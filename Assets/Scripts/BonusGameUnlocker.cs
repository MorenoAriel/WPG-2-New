using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BonusGameUnlocker : MonoBehaviour
{
    [Header("References")]
    public AltitudeScoreSystem scoreSystem;
    public PlayerController    playerController;
    public PlayerPhysics       playerPhysics;

    [Header("Settings")]
    public float  scoreThreshold = 500f;
    public string bonusSceneName = "BonusGame";

    private bool hasTriggered = false;
    public Animator transition;
    public float transitionDuration = 1f;

    void Update()
    {
        if (hasTriggered)        return;
        if (scoreSystem == null) return;

        if (scoreSystem.GetCurrentScore() >= scoreThreshold)
        {
            hasTriggered = true;
            LoadBonusGame();
        }
    }

    IEnumerator PlayTransitionAndLoad()
    {
        if (transition != null)
            transition.SetTrigger("Start");

        yield return new WaitForSeconds(1); // sesuaikan dengan durasi animasi

        LoadBonusGame();
    }

    void LoadBonusGame()
    {
        // Simpan semua state
        GameStateBridge.HasSavedState  = true;
        GameStateBridge.MainSceneName  = SceneManager.GetActiveScene().name;
        GameStateBridge.Score          = scoreSystem.GetCurrentScore();
        GameStateBridge.PlayerPosition = playerController.transform.position;

        if (playerPhysics != null)
        {
            GameStateBridge.Velocity    = playerPhysics.velocity;
            GameStateBridge.FlightAngle = playerPhysics.flightAngle;
        }

        GameStateBridge.ControllerState   = playerController.currentState;
        GameStateBridge.RotationAngle     = playerController.CurrentRotationAngle;
        GameStateBridge.SavedTargetAngle  = playerController.TargetAngle;

        // Pause score
        scoreSystem.StopScore();

        Debug.Log($"[BonusGameUnlocker] State tersimpan. Score: {GameStateBridge.Score}");
        SceneManager.LoadScene(bonusSceneName);
    }
}