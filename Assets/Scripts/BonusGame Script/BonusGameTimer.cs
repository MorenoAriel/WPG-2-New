using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class BonusGameTimer : MonoBehaviour
{
    public static BonusGameTimer Instance;

    public enum BonusGameStage
    {
        StageOne,
        StageTwo,
        Completed
    }

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
    public GameObject gameOverPanel;

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

        if (timerPanel != null)
            timerPanel.SetActive(false);

        if (healthUIPanel != null)
            healthUIPanel.SetActive(true);

        if (stageTwoController == null)
            stageTwoController = FindAnyObjectByType<BonusGameStageTwo>();

        if (stageTwoController != null && stageTwoController.objectCountPanel != null)
            stageTwoController.objectCountPanel.SetActive(false);

        StartCoroutine(BonusGameSequence());
    }

    void OnEnable()
    {
        GameManager.GameOverEvent += OnGameOver;
    }

    void OnDisable()
    {
        GameManager.GameOverEvent -= OnGameOver;
    }

    void OnGameOver()
    {
        Debug.Log("[BonusGameTimer] Game Over terdeteksi! Menghentikan timer...");
        shouldStopTimer = true;
        timerRunning = false;
        SetPlayerInvulnerable(false);

        if (stageTwoController != null)
            stageTwoController.StopStageTwo();

        ShowGameOverUI();
    }

    IEnumerator BonusGameSequence()
    {
        yield return new WaitForSeconds(infoPanelDuration);

        if (infoPanel != null)
            infoPanel.SetActive(false);

        if (timerPanel != null)
            timerPanel.SetActive(true);

        yield return StartCoroutine(RunTimer(bonusGameDuration, false));

        if (shouldStopTimer)
            yield break;

        StartStageTwoTransition();
        yield return new WaitForSeconds(infoPanelDuration);

        if (infoPanel != null)
            infoPanel.SetActive(false);

        if (timerPanel != null)
            timerPanel.SetActive(true);

        if (stageTwoController != null)
            stageTwoController.StartStageTwo();

        yield return StartCoroutine(RunTimer(bonusGameStageTwoDuration, true));

        if (!shouldStopTimer && !stageTwoCompleted)
        {
            Debug.Log("[BonusGameTimer] Waktu bonus game 2 habis tetapi objektif belum tercapai. Game over.");
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
                timerText.text = Mathf.CeilToInt(timeRemaining).ToString();

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
            if (infoPanelText != null)
                infoPanelText.text = stageTwoInfoText;
        }

        if (timerPanel != null)
            timerPanel.SetActive(false);

        if (healthUIPanel != null)
            healthUIPanel.SetActive(false);

        SetPlayerInvulnerable(true);
        StopStageOneSpawners();
        DestroyStageOneObstacles();
    }

    void StopStageOneSpawners()
    {
        ObstacleSpawner[] spawners = FindObjectsByType<ObstacleSpawner>(FindObjectsSortMode.None);
        foreach (ObstacleSpawner spawner in spawners)
            spawner.StopSpawning();
    }

    void DestroyStageOneObstacles()
    {
        ObstacleMover[] movers = FindObjectsByType<ObstacleMover>(FindObjectsSortMode.None);
        foreach (ObstacleMover mover in movers)
            Destroy(mover.gameObject);
    }

    public void NotifyStageTwoComplete()
    {
        if (stageTwoCompleted)
            return;

        stageTwoCompleted = true;
        timerRunning = false;
        SetPlayerInvulnerable(false);

        if (stageTwoController != null)
            stageTwoController.StopStageTwo();

        if (timerText != null)
            timerText.text = "0";

        Debug.Log("[BonusGameTimer] Objektif bonus game 2 tercapai. Timer dihentikan.");
    }

    void TriggerStageTwoGameOver()
    {
        shouldStopTimer = true;
        SetPlayerInvulnerable(false);

        if (stageTwoController != null)
            stageTwoController.StopStageTwo();

        ShowGameOverUI();
    }

    void SetPlayerInvulnerable(bool value)
    {
        ShipController ship = FindAnyObjectByType<ShipController>();
        if (ship != null)
            ship.SetInvulnerable(value);
    }

    void ShowGameOverUI()
    {
        GameOverUI_Bonus ui = FindAnyObjectByType<GameOverUI_Bonus>();
        if (ui != null)
        {
            ui.ShowGameOver();
            return;
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
}
