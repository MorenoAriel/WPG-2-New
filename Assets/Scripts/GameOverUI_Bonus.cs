using UnityEngine;
using TMPro;

/// <summary>
/// GameOver UI used in the BonusGame scene.
/// Listens to the BonusGame `GameManager.GameOverEvent` and shows a local panel.
/// Reads score from `AltitudeScoreSystem` if present, else from `ScoreBridge`.
/// </summary>
public class GameOverUI_Bonus : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;
    public TMP_Text bestScoreText;
    public AltitudeScoreSystem scoreSystem;

    public AudioSource audioSource;
    public AudioClip buttonClickSound;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        GameManager.GameOverEvent += ShowGameOver;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    void OnDisable()
    {
        GameManager.GameOverEvent -= ShowGameOver;
    }

    void Start()
    {
        if (scoreSystem == null)
            scoreSystem = Object.FindAnyObjectByType<AltitudeScoreSystem>();
    }

    public void ShowGameOver()
    {
        Debug.Log("[GameOverUI_Bonus] ShowGameOver() terpanggil");

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        float finalScore = 0f;
        float bestScore = PlayerPrefs.GetFloat("BEST_SCORE", 0f);

        if (scoreSystem != null)
        {
            finalScore = scoreSystem.GetCurrentScore();
            bestScore = scoreSystem.GetBestScore();
            Debug.Log($"[GameOverUI_Bonus] Score dari AltitudeScoreSystem: {finalScore}");
        }
        else if (ScoreBridge.HasSavedScore)
        {
            finalScore = ScoreBridge.SavedScore;
            Debug.Log($"[GameOverUI_Bonus] Score dari ScoreBridge: {finalScore}");
        }
        else if (GameStateBridge.HasSavedState)
        {
            finalScore = GameStateBridge.Score;
            Debug.Log($"[GameOverUI_Bonus] Score dari GameStateBridge: {finalScore}");
        }
        else
        {
            Debug.LogWarning("[GameOverUI_Bonus] Tidak ada sumber score tersedia!");
        }

        if (finalScoreText != null)
            finalScoreText.text = finalScore.ToString("F0");

        if (bestScoreText != null)
            bestScoreText.text = bestScore.ToString("F0");
    }

    public void RestartGame()
    {
        PlayButtonClickSound();
        Time.timeScale = 1f;
        // Kembali ke MainScenery
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScenery");
    }

    public void PlayButtonClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
}
