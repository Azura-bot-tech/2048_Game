using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using System.Collections;

public class TileBoard : MonoBehaviour
{
    public GameManager gameManager;
    public Tile tilePrefab;
    private TileGrid grid;

    public TileState[] tileStates;

    private GameControls controls;
    private Vector2 moveInput;

    private bool waiting;

    private List<Tile> tiles;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        grid = GetComponentInChildren<TileGrid>();
        tiles = new List<Tile>(16);

        controls = new GameControls();
    }

    public void ClearBoard()
    {
        foreach (var cell in grid.cells)
        {
            cell.tile = null;
        }

        foreach (var tile in tiles)
        {
            Destroy(tile.gameObject);
        }
        tiles.Clear();
    }

    public void CreateTile()
    {
        Tile tile = Instantiate(tilePrefab, grid.transform);
        tile.SetState(tileStates[0], 2);
        tile.Spawn(grid.GetRandomEmptyCell());
        tiles.Add(tile);
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Board.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Board.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnDisable()
    {
        controls.Disable();
        controls.Board.Move.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Board.Move.canceled -= ctx => moveInput = Vector2.zero;
    }

    private void Update()
    {
        if (waiting || moveInput == Vector2.zero)
        {
            return;
        }

        if (moveInput == Vector2.up)
        {
            MoveTiles(Vector2Int.up, 0, 1, 1, 1);
        }
        else if (moveInput == Vector2.down)
        {
            MoveTiles(Vector2Int.down, 0, 1, grid.high - 2, -1);
        }
        else if (moveInput == Vector2.left)
        {
            MoveTiles(Vector2Int.left, 1, 1, 0, 1);
        }
        else if (moveInput == Vector2.right)
        {
            MoveTiles(Vector2Int.right, grid.width - 2, -1, 0, 1);
        }
    }

    private void MoveTiles(Vector2Int direction, int startX, int xIncrement, int startY, int yIncrement)
    {
        // Logic to move tiles in the specified direction
        bool changed = false;
        for (int x = startX; x < grid.width && x >= 0; x += xIncrement)
        {
            for (int y = startY; y < grid.high && y >= 0; y += yIncrement)
            {
                TileCell cell = grid.GetCell(x, y);
                if (cell.occupied)
                {
                    changed |= MoveTile(cell.tile, direction);
                }
            }
        }

        if (changed)
        {
            StartCoroutine(WaitForChanges());
        }
    }

    private bool MoveTile(Tile tile, Vector2Int direction)
    {
        TileCell newCell = null;
        TileCell adjacentCell = grid.GetAdjancyCell(tile.cell, direction);

        while (adjacentCell != null)
        {
            if (adjacentCell.occupied)
            {
                if (CanMerge(tile, adjacentCell.tile))
                {
                    Merge(tile, adjacentCell.tile);
                    return true;
                }
                break;
            }
            newCell = adjacentCell;
            adjacentCell = grid.GetAdjancyCell(adjacentCell, direction);
        }

        if (newCell != null)
        {
            tile.MoveTo(newCell);
            return true;
        }
        return false;
    }

    private bool CanMerge(Tile a, Tile b)
    {
        return a.value == b.value;
    }

    private void Merge(Tile a, Tile b)
    {
        tiles.Remove(a);
        a.Merge(b.cell);

        int index = Mathf.Clamp(IndexOf(b.state) + 1, 0, tileStates.Length - 1);
        int newValue = b.value * 2;

        b.SetState(tileStates[index], newValue);

        gameManager.AddScore(newValue);
        audioManager.PlaySfx(audioManager.mergeSound);
    }

    private int IndexOf(TileState state)
    {
        for (int i = 0; i < tileStates.Length; i++)
        {
            if (state == tileStates[i])
            {
                return i;
            }
        }

        return -1;
    }

    private IEnumerator WaitForChanges()
    {
        waiting = true;
        yield return new WaitForSeconds(0.1f);
        waiting = false;

        if (tiles.Count != grid.size)
        {
            CreateTile();
        }

        if (CheckForGameOver())
        {
            gameManager.GameOver();
            audioManager.StopInGameMusic();
            // audioManager.PlaySfx(audioManager.gameOverSound);
        }
    }

    private bool CheckForGameOver()
    {
        if (tiles.Count != grid.size)
        {
            return false;
        }

        foreach (var tile in tiles)
        {
            TileCell up = grid.GetAdjancyCell(tile.cell, Vector2Int.up);
            TileCell down = grid.GetAdjancyCell(tile.cell, Vector2Int.down);
            TileCell left = grid.GetAdjancyCell(tile.cell, Vector2Int.left);
            TileCell right = grid.GetAdjancyCell(tile.cell, Vector2Int.right);

            if (up != null && CanMerge(tile, up.tile))
            {
                return false;
            }
            if (down != null && CanMerge(tile, down.tile))
            {
                return false;
            }
            if (left != null && CanMerge(tile, left.tile))
            {
                return false;
            }
            if (right != null && CanMerge(tile, right.tile))
            {
                return false;
            }
        }
        return true;
    }
}


