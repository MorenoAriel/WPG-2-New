using UnityEngine;

/// <summary>
/// Simpan score game utama ke PlayerPrefs
/// sebelum scene berpindah ke BonusGame.
/// Attach ke GameObject yang sama dengan BonusGameUnlocker.
/// </summary>
public class MainGameScoreBridge : MonoBehaviour
{
    public AltitudeScoreSystem scoreSystem;
    private const string MainScoreKey = "MainGameScore";

    // Dipanggil otomatis saat scene akan diganti
    void OnDestroy()
    {
        if (scoreSystem != null)
            PlayerPrefs.SetFloat(MainScoreKey, scoreSystem.GetCurrentScore());
    }
}