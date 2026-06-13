using UnityEngine;

/// <summary>
/// Mengelola score saat bonus game dimulai.
/// - Restore score dari GameStateBridge
/// - Disable counting agar score tidak bertambah
/// - Menjaga score tetap sama sepanjang bonus game
/// </summary>
public class BonusGameScoreHandler : MonoBehaviour
{
    public AltitudeScoreSystem scoreSystem;

    void Start()
    {
        if (scoreSystem == null)
        {
            scoreSystem = Object.FindAnyObjectByType<AltitudeScoreSystem>();
        }

        if (scoreSystem != null && GameStateBridge.HasSavedState)
        {
            // Restore score dari main game
            float savedScore = GameStateBridge.Score;
            scoreSystem.SetStartingScore(savedScore);
            scoreSystem.StopScore();

            // Also forward saved score to ScoreBridge so bonus-scene UI can read it
            ScoreBridge.SavedScore = savedScore;
            ScoreBridge.HasSavedScore = true;

            Debug.Log($"[BonusGameScoreHandler] Score di-restore ke {savedScore}. Counting disabled. ScoreBridge diisi.");
        }
        else
        {
            Debug.LogWarning("[BonusGameScoreHandler] scoreSystem tidak ditemukan atau tidak ada saved state!");
        }
    }
}
