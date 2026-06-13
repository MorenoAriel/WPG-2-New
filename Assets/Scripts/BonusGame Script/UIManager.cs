using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [Header("Panels")]
    public GameObject startPanel;
    public GameObject gameOverPanel;

    [Header("Auto Assign")]
    public string gameOverPanelSearchText = "Game Over";

    [Header("Health Bar")]
    public Slider healthBar;
    public Image  healthBarFill;
    public TextMeshProUGUI healthText;
    public int defaultMaxHealth = 5;

    [Header("Health Bar Colors")]
    public Color colorFull = Color.green;
    public Color colorMid  = Color.yellow;
    public Color colorLow  = Color.red;

    [Header("Game Over UI")]
    public TextMeshProUGUI finalHealthText;
    public Button restartButton;

    [Header("Animation")]
    public float panelFadeInDuration = 0.4f;
    private CanvasGroup gameOverCanvasGroup;

    void Awake()
    {
        Instance = this;
        if (gameOverPanel == null)
            gameOverPanel = FindPanelByName(gameOverPanelSearchText);

        if (gameOverPanel != null)
        {
            gameOverCanvasGroup = gameOverPanel.GetComponent<CanvasGroup>();
            if (gameOverCanvasGroup == null)
                gameOverCanvasGroup = gameOverPanel.AddComponent<CanvasGroup>();

            gameOverPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("[UIManager] gameOverPanel belum diassign dan tidak ditemukan otomatis. Pastikan GameObject panel Game Over ada di scene.");
        }

        if (startPanel != null)
            startPanel.SetActive(true);

        // Init health bar saat Awake agar tidak kosong di Editor
        int max = GameManager.Instance != null
            ? GameManager.Instance.maxHealth
            : defaultMaxHealth;

        SetHealthBar(max, max);
    }

    GameObject FindPanelByName(string searchText)
    {
        if (string.IsNullOrEmpty(searchText))
            return null;

        string normalizedSearch = NormalizeName(searchText);

        RectTransform[] rects = Resources.FindObjectsOfTypeAll<RectTransform>();
        foreach (RectTransform rect in rects)
        {
            if (rect.gameObject.scene.isLoaded == false)
                continue;

            string normalizedName = NormalizeName(rect.gameObject.name);
            if (normalizedName.Contains(normalizedSearch))
                return rect.gameObject;
        }

        return null;
    }

    string NormalizeName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return "";

        return name.Replace(" ", "").Replace("_", "").ToLowerInvariant();
    }

    void OnEnable()
    {
        GameManager.GameStartedEvent   += OnGameStarted;
        GameManager.GameOverEvent      += OnGameOver;
        GameManager.HealthChangedEvent += OnHealthChanged;
    }

    void OnDisable()
    {
        GameManager.GameStartedEvent   -= OnGameStarted;
        GameManager.GameOverEvent      -= OnGameOver;
        GameManager.HealthChangedEvent -= OnHealthChanged;
    }

    void Start()
    {
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);

        // Pastikan health bar terisi penuh saat Start
        int max = GameManager.Instance != null
            ? GameManager.Instance.maxHealth
            : defaultMaxHealth;

        SetHealthBar(max, max);
    }

    void OnGameStarted()
    {
        if (startPanel != null)
            startPanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Reset health bar saat game mulai
        int max = GameManager.Instance != null
            ? GameManager.Instance.maxHealth
            : defaultMaxHealth;

        SetHealthBar(max, max);
    }

    void OnHealthChanged(int current, int max)
    {
        SetHealthBar(current, max);
    }

    void SetHealthBar(int current, int max)
    {
        if (healthBar != null)
        {
            healthBar.minValue = 0;
            healthBar.maxValue = max;
            healthBar.value    = current;
        }

        if (healthText != null)
            healthText.text = $"{current} / {max}";

        if (healthBarFill != null)
        {
            float ratio = (float)current / max;
            if      (ratio > 0.6f) healthBarFill.color = colorFull;
            else if (ratio > 0.3f) healthBarFill.color = colorMid;
            else                   healthBarFill.color = colorLow;
        }
    }

    void OnGameOver()
    {
        if (finalHealthText != null && GameManager.Instance != null)
            finalHealthText.text = $"Sisa HP: {GameManager.Instance.currentHealth}";

        StartCoroutine(ShowGameOverPanel());
    }

    public IEnumerator ShowGameOverPanel()
    {
        if (gameOverPanel == null)
        {
            Debug.LogWarning("[UIManager] ShowGameOverPanel dipanggil tetapi gameOverPanel tidak tersedia.");
            yield break;
        }

        yield return new WaitForSeconds(0.8f);

        gameOverPanel.SetActive(true);
        gameOverCanvasGroup.alpha = 0f;

        float elapsed = 0f;
        while (elapsed < panelFadeInDuration)
        {
            elapsed += Time.deltaTime;
            gameOverCanvasGroup.alpha = Mathf.Clamp01(elapsed / panelFadeInDuration);
            yield return null;
        }

        gameOverCanvasGroup.alpha = 1f;
    }

    void OnRestartClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RestartGame();
    }
}