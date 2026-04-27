using UnityEngine;
using TMPro;

public class AltitudeScoreSystem : MonoBehaviour
{
    public float GetCurrentScore()
{
    return currentScore;
}

public float GetBestScore()
{
    return bestScore;
}
    [Header("Reference")]
    public Transform player;
    public float groundY = 0f;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text bestText;

    [Header("Score Settings")]
    public float scoreMultiplier = 5f;     // dibuat lebih kecil biar halus
    public float airTimeBonus = 0.5f;

    [Header("Start Delay")]
    public float startDelay = 1.5f;         // ⏳ jeda setelah SPACE

    private float currentScore = 0f;
    private float bestScore = 0f;

    private bool scoreStarted = false;
    private bool countingEnabled = false;
    private float startTimer = 0f;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        bestScore = PlayerPrefs.GetFloat("BEST_SCORE", 0f);
        UpdateBestUI();
    }

    void Update()
    {
        if (player == null) return;

        HandleStartSequence();

        if (!countingEnabled) return;

        float height = player.position.y - groundY;

        if (height > 0.5f)
        {
            // 🔥 SCORE SMOOTH (tidak langsung naik besar)
            float targetGain = (height * scoreMultiplier + airTimeBonus);

            currentScore = Mathf.Lerp(
                currentScore,
                currentScore + targetGain,
                Time.deltaTime * 2f
            );
        }

        UpdateUI();
    }

    // =========================================================
    // START SYSTEM (SPACE + DELAY)
    // =========================================================
    void HandleStartSequence()
    {
        if (!scoreStarted)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                scoreStarted = true;
                startTimer = startDelay;
            }
            return;
        }

        if (startTimer > 0f)
        {
            startTimer -= Time.deltaTime;

            if (startTimer <= 0f)
            {
                countingEnabled = true;
            }
        }
    }

    // =========================================================
    // UI
    // =========================================================
    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + currentScore.ToString("F0");
    }

    void UpdateBestUI()
    {
        if (bestText != null)
            bestText.text = "Best: " + bestScore.ToString("F0");
    }

    // =========================================================
    // SAVE
    // =========================================================
    public void StopScore()
    {
        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetFloat("BEST_SCORE", bestScore);
            PlayerPrefs.Save();
        }
    }

    public void ResetScore()
    {
        currentScore = 0f;
        scoreStarted = false;
        countingEnabled = false;
        startTimer = 0f;

        UpdateUI();
    }
}