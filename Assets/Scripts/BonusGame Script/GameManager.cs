using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Health Settings")]
    public int maxHealth = 5;
    public int currentHealth;

    [Header("State")]
    public bool gameStarted = false;
    public bool gameOver = false;

    public delegate void OnGameStarted();
    public delegate void OnGameOver();
    public delegate void OnHealthChanged(int current, int max);

    public static event OnGameStarted   GameStartedEvent;
    public static event OnGameOver      GameOverEvent;
    public static event OnHealthChanged HealthChangedEvent;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentHealth = maxHealth;
        StartCoroutine(WaitForStartInput());
    }

    IEnumerator WaitForStartInput()
    {
        while (!Input.GetKeyDown(KeyCode.Space))
            yield return null;

        gameStarted = true;
        GameStartedEvent?.Invoke();
    }

    public void OnPlayerHit()
    {
        if (gameOver) return;

        currentHealth -= 1;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"[GameManager] Hit! Health → {currentHealth}/{maxHealth}");

        HealthChangedEvent?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("[GameManager] Health = 0. Game Over!");
            TriggerGameOver();
        }
    }

    // Public: dipanggil BonusGameTimer saat timer Stage 2 habis
    public void TriggerGameOverExternal()
    {
        if (gameOver) return;
        Debug.Log("[GameManager] TriggerGameOverExternal — Stage 2 timeout.");
        TriggerGameOver();
    }

    void TriggerGameOver()
    {
        gameOver = true;
        gameStarted = false;
        GameOverEvent?.Invoke(); // → GameOverUI_Bonus.ShowGameOver() terpanggil otomatis
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}