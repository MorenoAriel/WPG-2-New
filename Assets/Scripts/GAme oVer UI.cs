using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;
    public TMP_Text bestScoreText;
    public AltitudeScoreSystem scoreSystem;

    void Start()
    {
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver()
{
    gameOverPanel.SetActive(true);

    float finalScore = 0f;
    float bestScore = 0f;

    if (scoreSystem != null)
    {
        finalScore = scoreSystem.GetCurrentScore();
        bestScore = scoreSystem.GetBestScore();
        scoreSystem.StopScore();
    }

    if (finalScoreText != null)
        finalScoreText.text = "Score: " + finalScore.ToString("F0");

    if (bestScoreText != null)
        bestScoreText.text = "Best: " + bestScore.ToString("F0");

    // ❗ jangan freeze dulu kalau debugging
    // Time.timeScale = 0f;
}

    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }
    public void Back_To_Menu()//ini untuk kemabali ke menu awwal biasanya di gunain ketika ui punya setting dan credit di menu awal (fungsi ini gunanaya untuk kembali ke menu awal)
    {
        SceneManager.LoadScene("Coba-Coba");//ganti nama scene dengan nama scene menu yang akan di load pertama kali (namanya harus sama ya(huruf kapital dan yang lainya))
        Debug.Log("Back To Menu");
    }
}