using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tile : MonoBehaviour
{
    public TileState state { get; private set; }

    public TileCell cell { get; private set; }

    public int value { get; private set; }

    private Image backgroundImage;

    private TextMeshProUGUI text;

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();

    }

    public void SetState(TileState state, int value)
    {
        this.state = state;
        this.value = value;

        backgroundImage.color = state.backgroundColor;
        text.color = state.textColor;
        text.text = value.ToString();
    }

    public void Spawn(TileCell cell)
    {
        if (this.cell != null)
        {
            Debug.Log("Tile is already in a cell");
            this.cell.tile = null;
        }

        this.cell = cell;
        this.cell.tile = this;

        transform.position = cell.transform.position;
    }

    public void Merge(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = null;

        StartCoroutine(Animate(cell.transform.position, true));
    }

    public void MoveTo(TileCell newCell)
    {
        if (cell != null)
        {
            cell.tile = null;
        }

        cell = newCell;
        cell.tile = this;

        StartCoroutine(Animate(newCell.transform.position, false));
    }

    private IEnumerator Animate(Vector3 to, bool isMerging)
    {
        float duration = 0.1f;
        float elapsed = 0f;
        Vector3 from = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = to;

        if (isMerging)
        {
            Destroy(gameObject);
        }
    }
}
