using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class BonusGameTimer : MonoBehaviour
{
    public static BonusGameTimer Instance;

    public enum BonusGameStage { StageOne, StageTwo, Completed }

    [Header("Info Panel")]
    public GameObject infoPanel;
    public TMP_Text infoPanelText;
    public float infoPanelDuration = 3f;
    public string stageOneInfoText = "Bonus Game 1 dimulai! Hindari semua rintangan sampai waktunya habis.";
    public string stageTwoInfoText = "Bonus Game 2 dimulai! Kumpulkan 2 objek masing-masing 5x dalam 30 detik.";

    [Header("Timer")]
    public float bonusGameDuration = 20f;
    public float bonusGameStageTwoDuration = 30f;
    public TMP_Text timerText;
    public GameObject timerPanel;
    public string fallbackMainScene = "MainScenery";

    [Header("UI References")]
    public GameObject healthUIPanel;
    public BonusGameStageTwo stageTwoController;

    [Header("Stage Control")]
    public BonusGameStage currentStage = BonusGameStage.StageOne;

    private float timeRemaining;
    private bool timerRunning = false;
    private bool shouldStopTimer = false;
    private bool stageTwoCompleted = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentStage = BonusGameStage.StageOne;
        stageTwoCompleted = false;
        shouldStopTimer = false;
        timeRemaining = bonusGameDuration;

        if (infoPanel != null)
        {
            infoPanel.SetActive(true);
            if (infoPanelText != null)
                infoPanelText.text = stageOneInfoText;
        }

        if (timerPanel != null) timerPanel.SetActive(false);
        if (healthUIPanel != null) healthUIPanel.SetActive(true);

        if (stageTwoController == null)
            stageTwoController = FindAnyObjectByType<BonusGameStageTwo>();

        if (stageTwoController?.objectCountPanel != null)
            stageTwoController.objectCountPanel.SetActive(false);

        StartCoroutine(BonusGameSequence());
    }

    void OnEnable()  { GameManager.GameOverEvent += OnGameOver; }
    void OnDisable() { GameManager.GameOverEvent -= OnGameOver; }

    void OnGameOver()
    {
        if (shouldStopTimer) return;

        Debug.Log("[BonusGameTimer] OnGameOver() — stop semua coroutine.");
        shouldStopTimer = true;
        timerRunning = false;
        StopAllCoroutines();

        SetPlayerInvulnerable(false);
        if (stageTwoController != null) stageTwoController.StopStageTwo();

        // Panggil langsung sebagai safety net jika event di GameOverUI_Bonus
        // belum terdaftar karena timing issue
        ForceShowGameOverUI();
    }

    IEnumerator BonusGameSequence()
    {
        yield return new WaitForSeconds(infoPanelDuration);
        if (infoPanel != null) infoPanel.SetActive(false);
        if (timerPanel != null) timerPanel.SetActive(true);

        yield return StartCoroutine(RunTimer(bonusGameDuration, false));
        if (shouldStopTimer) yield break;

        StartStageTwoTransition();
        yield return new WaitForSeconds(infoPanelDuration);
        if (infoPanel != null) infoPanel.SetActive(false);
        if (timerPanel != null) timerPanel.SetActive(true);

        if (stageTwoController != null) stageTwoController.StartStageTwo();

        yield return StartCoroutine(RunTimer(bonusGameStageTwoDuration, true));

        if (!shouldStopTimer && !stageTwoCompleted)
        {
            Debug.Log("[BonusGameTimer] Waktu Stage 2 habis, target belum tercapai.");
            TriggerStageTwoGameOver();
        }
    }

    IEnumerator RunTimer(float duration, bool isStageTwo)
    {
        timerRunning = true;
        timeRemaining = duration;

        while (timeRemaining > 0f && !shouldStopTimer && (!isStageTwo || !stageTwoCompleted))
        {
            timeRemaining -= Time.deltaTime;
            if (timerText != null)
                timerText.text = Mathf.CeilToInt(Mathf.Max(timeRemaining, 0f)).ToString();
            yield return null;
        }

        timerRunning = false;
    }

    void StartStageTwoTransition()
    {
        currentStage = BonusGameStage.StageTwo;

        if (infoPanel != null)
        {
            infoPanel.SetActive(true);
            if (infoPanelText != null) infoPanelText.text = stageTwoInfoText;
        }

        if (timerPanel != null) timerPanel.SetActive(false);
        if (healthUIPanel != null) healthUIPanel.SetActive(false);

        SetPlayerInvulnerable(true);

        foreach (var s in FindObjectsByType<ObstacleSpawner>(FindObjectsSortMode.None))
            s.StopSpawning();
        foreach (var m in FindObjectsByType<ObstacleMover>(FindObjectsSortMode.None))
            Destroy(m.gameObject);
    }

    public void NotifyStageTwoComplete()
    {
        if (stageTwoCompleted) return;

        stageTwoCompleted = true;
        timerRunning = false;
        SetPlayerInvulnerable(false);
        if (stageTwoController != null) stageTwoController.StopStageTwo();
        if (timerText != null) timerText.text = "0";

        Debug.Log("[BonusGameTimer] Stage 2 selesai!");
        // TODO: tampilkan layar Victory
    }

    void TriggerStageTwoGameOver()
    {
        shouldStopTimer = true;
        SetPlayerInvulnerable(false);
        if (stageTwoController != null) stageTwoController.StopStageTwo();

        // Picu lewat GameManager agar GameOverEvent di-broadcast ke semua subscriber
        if (GameManager.Instance != null && !GameManager.Instance.gameOver)
        {
            Debug.Log("[BonusGameTimer] Memanggil GameManager.TriggerGameOverExternal().");
            GameManager.Instance.TriggerGameOverExternal();
        }
        else
        {
            // Fallback: GameManager tidak ada atau sudah game over
            Debug.LogWarning("[BonusGameTimer] GameManager tidak tersedia, tampilkan UI langsung.");
            ForceShowGameOverUI();
        }
    }

    // Safety net: panggil ShowGameOver() langsung jika event tidak tertangkap
    void ForceShowGameOverUI()
    {
        GameOverUI_Bonus ui = FindAnyObjectByType<GameOverUI_Bonus>(FindObjectsInactive.Include);
        if (ui != null)
        {
            Debug.Log("[BonusGameTimer] ForceShowGameOverUI — memanggil ShowGameOver() langsung.");
            ui.ShowGameOver();
        }
        else
        {
            Debug.LogError("[BonusGameTimer] GameOverUI_Bonus tidak ditemukan di scene!");
        }
    }

    void SetPlayerInvulnerable(bool value)
    {
        ShipController ship = FindAnyObjectByType<ShipController>();
        if (ship != null) ship.SetInvulnerable(value);
    }
}