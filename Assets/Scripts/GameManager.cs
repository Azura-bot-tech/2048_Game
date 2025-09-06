using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TileBoard board;
    public CanvasGroup gameOverPanel;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;

    private AudioManager audioManager;
    private int score;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        SetScore(0);
        bestScoreText.text = LoadHisScore().ToString();

        gameOverPanel.alpha = 0f;
        gameOverPanel.interactable = false;

        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;

        audioManager.PlayInGameMusic();
        audioManager.StopPlayingSfx(audioManager.gameOverSound);
    }

    public void GameOver()
    {
        board.enabled = false;
        gameOverPanel.interactable = true;
        StartCoroutine(Fade(gameOverPanel, 1f, 1f));

        audioManager.PlaySfx(audioManager.gameOverSound);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private IEnumerator Fade(CanvasGroup group, float to, float delay)
    {
        yield return new WaitForSeconds(delay);

        float from = group.alpha;
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        group.alpha = to;
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString();
        SaveHisScore();
    }

    public void AddScore(int amount)
    {
        SetScore(score + amount);
    }

    private void SaveHisScore()
    {
        int hisScore = LoadHisScore();
        if (score > hisScore)
        {
            PlayerPrefs.SetInt("HisScore", score);
        }
    }
    
    private int LoadHisScore()
    {
        return PlayerPrefs.GetInt("HisScore", 0);
    }
}
