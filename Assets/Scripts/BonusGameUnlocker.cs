using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BonusGameUnlocker : MonoBehaviour
{
    [Header("References")]
    public GameObject playerObject;     // assign Player GameObject di Inspector
    public GameObject scoreObject;      // assign object yang punya AltitudeScoreSystem
    public Animator   transition;

    [Header("Settings")]
    public string bonusSceneName    = "BonusGame";
    public string playerTag         = "Player";
    public float  transitionDuration = 1f;

    private AltitudeScoreSystem scoreSystem;
    private PlayerController    playerController;
    private PlayerPhysics       playerPhysics;
    private bool hasTriggered = false;

    void Awake()
    {
        scoreSystem      = scoreObject.GetComponent<AltitudeScoreSystem>();
        playerController = playerObject.GetComponent<PlayerController>();
        playerPhysics    = playerObject.GetComponent<PlayerPhysics>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered) return;

        if (collision.CompareTag(playerTag))
        {
            hasTriggered = true;

            if (transition != null)
                StartCoroutine(PlayTransitionAndLoad());
            else
                LoadBonusGame();
        }
    }

    IEnumerator PlayTransitionAndLoad()
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionDuration);
        LoadBonusGame();
    }

    void LoadBonusGame()
    {
        GameStateBridge.HasSavedState  = true;
        GameStateBridge.MainSceneName  = SceneManager.GetActiveScene().name;
        GameStateBridge.Score          = scoreSystem.GetCurrentScore();
        GameStateBridge.PlayerPosition = playerController.transform.position;

        if (playerPhysics != null)
        {
            GameStateBridge.Velocity    = playerPhysics.velocity;
            GameStateBridge.FlightAngle = playerPhysics.flightAngle;
        }

        GameStateBridge.ControllerState  = playerController.currentState;
        GameStateBridge.RotationAngle    = playerController.CurrentRotationAngle;
        GameStateBridge.SavedTargetAngle = playerController.TargetAngle;

        scoreSystem.StopScore();

        Debug.Log($"[BonusGameUnlocker] State tersimpan. Score: {GameStateBridge.Score}");
        SceneManager.LoadScene(bonusSceneName);
    }
}