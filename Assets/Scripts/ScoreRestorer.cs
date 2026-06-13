using UnityEngine;

/// <summary>
/// Saat scene game utama di-load kembali, 
/// restore score ke AltitudeScoreSystem.
/// Attach ke GameObject yang sama dengan BonusGameUnlocker.
/// </summary>
public class ScoreRestorer : MonoBehaviour
{
    public AltitudeScoreSystem scoreSystem;

    void Start()
    {
        if (!ScoreBridge.HasSavedScore) return;
        if (scoreSystem == null) return;

        Debug.Log($"[ScoreRestorer] Melanjutkan score: {ScoreBridge.SavedScore}");

        // Inject score tersimpan lalu lanjutkan
        scoreSystem.SetStartingScore(ScoreBridge.SavedScore);
        scoreSystem.StartScore(); // atau method apapun untuk mulai score

        // Reset bridge
        ScoreBridge.HasSavedScore = false;
        ScoreBridge.SavedScore    = 0f;
    }
}