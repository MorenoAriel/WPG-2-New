using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance;

    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;
    public TMP_Text bestScoreText;
    public AltitudeScoreSystem scoreSystem;

    public AudioSource audioSource;
    public AudioClip buttonClickSound;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[GameOverUI] Instance dibuat dan DontDestroyOnLoad aktif");
        }
        else if (Instance != this)
        {
            Debug.Log("[GameOverUI] Instance sudah ada, menghapus duplicate");
            Destroy(gameObject);
            return;
        }

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        // Main-scene singleton: we only listen for scene loads to refresh scoreSystem reference.
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("[GameOverUI] Scene load listener terpasang (Main)");
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Debug.Log("[GameOverUI] Scene load listener terlepas (Main)");
    }

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameOverUI] Scene '{scene.name}' dimuat");
        
        // Setiap scene berubah, cari AltitudeScoreSystem yang baru (atau null jika tidak ada)
        scoreSystem = FindAnyObjectByType<AltitudeScoreSystem>();
        if (scoreSystem != null)
            Debug.Log("[GameOverUI] AltitudeScoreSystem ditemukan di scene ini");
        else
            Debug.Log("[GameOverUI] AltitudeScoreSystem TIDAK ditemukan di scene ini (akan gunakan GameStateBridge)");
        
        // Pastikan panel inactive saat scene baru dimulai
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            Debug.Log("[GameOverUI] Panel gameover di-nonaktifkan saat scene berubah");
        }
        else
        {
            Debug.LogWarning("[GameOverUI] gameOverPanel reference kosong saat scene berubah!");
        }
    }

    public void ShowGameOver()
    {
        Debug.Log("[GameOverUI] ShowGameOver() terpanggil!");
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Debug.Log("[GameOverUI] Panel gameover di-aktifkan");
        }
        else
        {
            Debug.LogWarning("[GameOverUI] gameOverPanel reference kosong!");
        }

        float finalScore = 0f;
        float bestScore = PlayerPrefs.GetFloat("BEST_SCORE", 0f);

        if (scoreSystem != null)
        {
            finalScore = scoreSystem.GetCurrentScore();
            bestScore = scoreSystem.GetBestScore();
            scoreSystem.StopScore();
            Debug.Log($"[GameOverUI] Score dari AltitudeScoreSystem: {finalScore}");
        }
        else if (GameStateBridge.HasSavedState)
        {
            finalScore = GameStateBridge.Score;
            Debug.Log($"[GameOverUI] Score dari GameStateBridge: {finalScore}");
        }
        else
        {
            Debug.LogWarning("[GameOverUI] Tidak ada scoreSystem dan GameStateBridge kosong! (Main)");
        }

        if (finalScoreText != null)
            finalScoreText.text = finalScore.ToString("F0");

        if (bestScoreText != null)
            bestScoreText.text = bestScore.ToString("F0");

        // ❗ jangan freeze dulu kalau debugging
        // Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        PlayButtonClickSound();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScenery");
    }
    public void Back_To_Menu()//ini untuk kemabali ke menu awwal biasanya di gunain ketika ui punya setting dan credit di menu awal (fungsi ini gunanaya untuk kembali ke menu awal)
    {
        PlayButtonClickSound();
        SceneManager.LoadScene("Main Menu");//ganti nama scene dengan nama scene menu yang akan di load pertama kali (namanya harus sama ya(huruf kapital dan yang lainya))
        Debug.Log("Back To Menu");
    }

    public void PlayButtonClickSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
}