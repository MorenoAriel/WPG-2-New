using UnityEngine;
using TMPro;

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

        // FIX: subscribe di Awake, bukan OnEnable
        // Jika GameObject ini disabled saat Start, OnEnable tidak pernah dipanggil
        // sehingga event tidak pernah terdaftar
        GameManager.GameOverEvent += ShowGameOver;
        Debug.Log("[GameOverUI_Bonus] Subscribed ke GameOverEvent di Awake.");

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        else
            Debug.LogError("[GameOverUI_Bonus] gameOverPanel BELUM DI-ASSIGN di Inspector!");
    }

    void OnDestroy()
    {
        GameManager.GameOverEvent -= ShowGameOver;
    }

    // Hapus OnEnable/OnDisable — sudah dipindah ke Awake/OnDestroy
    // agar tidak double-subscribe jika GameObject di-enable ulang

    void Start()
    {
        if (scoreSystem == null)
            scoreSystem = Object.FindAnyObjectByType<AltitudeScoreSystem>();
    }

    public void ShowGameOver()
    {
        Debug.Log("[GameOverUI_Bonus] ShowGameOver() dipanggil!");

        if (gameOverPanel == null)
        {
            Debug.LogError("[GameOverUI_Bonus] gameOverPanel null — drag panel ke Inspector!");
            return;
        }

        gameOverPanel.SetActive(true);
        Debug.Log("[GameOverUI_Bonus] gameOverPanel.SetActive(true) berhasil.");

        float finalScore = 0f;
        float bestScore = PlayerPrefs.GetFloat("BEST_SCORE", 0f);

        if (scoreSystem != null)
        {
            finalScore = scoreSystem.GetCurrentScore();
            bestScore = scoreSystem.GetBestScore();
        }
        else if (ScoreBridge.HasSavedScore)
        {
            finalScore = ScoreBridge.SavedScore;
        }
        else if (GameStateBridge.HasSavedState)
        {
            finalScore = GameStateBridge.Score;
        }
        else
        {
            Debug.LogWarning("[GameOverUI_Bonus] Tidak ada sumber score tersedia.");
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
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScenery");
    }

    public void PlayButtonClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
            audioSource.PlayOneShot(buttonClickSound);
    }
}