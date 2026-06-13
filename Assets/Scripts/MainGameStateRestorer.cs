using UnityEngine;
using System.Collections;

public class MainGameStateRestorer : MonoBehaviour
{
    [Header("References")]
    public PlayerController    playerController;
    public PlayerPhysics       playerPhysics;
    public AltitudeScoreSystem scoreSystem;

    IEnumerator Start()
    {
        if (!GameStateBridge.HasSavedState) yield break;

        // Tunggu 1 frame agar PlayerController.Start() selesai duluan
        yield return null;

        if (playerController == null)
        {
            playerController = Object.FindAnyObjectByType<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("[MainGameStateRestorer] playerController belum diassign di Inspector dan tidak ditemukan secara otomatis. State tidak bisa di-restore.");
                yield break;
            }
            Debug.LogWarning("[MainGameStateRestorer] playerController tidak diassign, tapi ditemukan otomatis di scene.");
        }

        if (playerPhysics == null)
        {
            playerPhysics = Object.FindAnyObjectByType<PlayerPhysics>();
            if (playerPhysics == null)
            {
                Debug.LogWarning("[MainGameStateRestorer] playerPhysics tidak diassign dan tidak ditemukan secara otomatis. Physics tidak akan di-restore.");
            }
            else
            {
                Debug.LogWarning("[MainGameStateRestorer] playerPhysics tidak diassign, tapi ditemukan otomatis di scene.");
            }
        }

        if (scoreSystem == null)
        {
            scoreSystem = Object.FindAnyObjectByType<AltitudeScoreSystem>();
            if (scoreSystem == null)
            {
                Debug.LogWarning("[MainGameStateRestorer] scoreSystem tidak diassign dan tidak ditemukan secara otomatis. Score tidak akan di-restore.");
            }
            else
            {
                Debug.LogWarning("[MainGameStateRestorer] scoreSystem tidak diassign, tapi ditemukan otomatis di scene.");
            }
        }

        // Restore posisi
        playerController.transform.position = GameStateBridge.PlayerPosition;

        // Restore physics
        if (playerPhysics != null)
        {
            playerPhysics.velocity    = GameStateBridge.Velocity;
            playerPhysics.flightAngle = GameStateBridge.FlightAngle;
            playerPhysics.SetPhysicsActive(true);
        }

        // Restore controller state & rotasi
        playerController.currentState = GameStateBridge.ControllerState;
        playerController.TargetAngle  = GameStateBridge.SavedTargetAngle;

        playerController.transform.rotation = Quaternion.Euler(
            0f, 0f, GameStateBridge.RotationAngle
        );

        // Restore score
        if (scoreSystem != null)
        {
            scoreSystem.SetStartingScore(GameStateBridge.Score);
            scoreSystem.StartScore();
        }

        Debug.Log($"[Restorer] State berhasil di-restore. Score: {GameStateBridge.Score}");

        // Bersihkan bridge
        GameStateBridge.Clear();
    }
}