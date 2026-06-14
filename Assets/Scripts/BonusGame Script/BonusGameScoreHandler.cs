using UnityEngine;

/// <summary>
/// Mengelola score saat bonus game dimulai.
/// - Restore score dari GameStateBridge ke AltitudeScoreSystem (jika ada)
/// - Isi ScoreBridge agar GameOverUI_Bonus bisa membaca score
/// - Disable counting agar score tidak bertambah selama bonus game
/// </summary>
public class BonusGameScoreHandler : MonoBehaviour
{
    public AltitudeScoreSystem scoreSystem;

    void Start()
    {
        if (scoreSystem == null)
            scoreSystem = Object.FindAnyObjectByType<AltitudeScoreSystem>();

        if (!GameStateBridge.HasSavedState)
        {
            Debug.LogWarning("[BonusGameScoreHandler] Tidak ada saved state di GameStateBridge. " +
                             "Mungkin scene dijalankan langsung tanpa melalui main game.");

            // Coba ambil dari ScoreBridge jika sudah ada isinya
            if (!ScoreBridge.HasSavedScore)
            {
                Debug.LogWarning("[BonusGameScoreHandler] ScoreBridge juga kosong. Score akan 0.");
            }
            // Tidak lanjut ke bawah — tidak ada yang bisa di-restore
            return;
        }

        float savedScore = GameStateBridge.Score;
        Debug.Log($"[BonusGameScoreHandler] Score dari GameStateBridge: {savedScore}");

        // Selalu isi ScoreBridge agar GameOverUI_Bonus bisa membacanya
        ScoreBridge.SavedScore = savedScore;
        ScoreBridge.HasSavedScore = true;

        // Jika AltitudeScoreSystem ada di scene, restore dan stop counting
        if (scoreSystem != null)
        {
            scoreSystem.SetStartingScore(savedScore);
            scoreSystem.StopScore();
            Debug.Log($"[BonusGameScoreHandler] Score di-restore ke AltitudeScoreSystem ({savedScore}). Counting disabled.");
        }
        else
        {
            // Tidak apa-apa — GameOverUI_Bonus akan pakai ScoreBridge sebagai fallback
            Debug.Log("[BonusGameScoreHandler] AltitudeScoreSystem tidak ada di scene BonusGame. " +
                      "Score tetap tersimpan di ScoreBridge untuk UI.");
        }
    }
}