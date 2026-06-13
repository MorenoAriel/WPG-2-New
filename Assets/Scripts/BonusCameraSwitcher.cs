using UnityEngine;
using Cinemachine;

// BonusCameraSwitcher.cs
// - Tujuan: Gunakan dua Cinemachine Virtual Cameras:
//   * normalCam = kamera utama selama gameplay
//   * boundCam  = vcam yang memiliki Cinemachine Confiner (bounding shape sudah diset di Inspector)
// Saat skor >= threshold, script menaikkan prioritas boundCam sehingga kamera berganti ke versi terbatas.

public class BonusCameraSwitcher : MonoBehaviour
{
    [Header("Virtual Cameras")]
    public CinemachineVirtualCamera normalCam;
    public CinemachineVirtualCamera boundCam; // should have a Confiner component configured in-editor

    [Header("Score")]
    public int scoreThreshold = 500;

    [Header("Optional Score Source")]
    // Jika kamu punya AltitudeScoreSystem, drag referensinya di Inspector.
    public MonoBehaviour scoreSource; // expects a method float GetCurrentScore() if set

    bool boundActive = false;

    void Start()
    {
        // Pastikan prioritas awal: normalCam lebih tinggi
        if (normalCam != null) normalCam.Priority = 20;
        if (boundCam != null) boundCam.Priority = 0;
    }

    void Update()
    {
        if (scoreSource != null)
        {
            // Try call GetCurrentScore() reflectively to avoid hard dependency
            var method = scoreSource.GetType().GetMethod("GetCurrentScore");
            if (method != null)
            {
                var val = method.Invoke(scoreSource, null);
                if (val != null && float.TryParse(val.ToString(), out float score))
                {
                    CheckScore(Mathf.FloorToInt(score));
                }
            }
        }
    }

    // Panggil dari code lain ketika score berubah, atau biarkan script polling Update()
    public void CheckScore(int score)
    {
        if (score >= scoreThreshold && !boundActive)
        {
            SetBound(true);
        }
        else if (score < scoreThreshold && boundActive)
        {
            SetBound(false);
        }
    }

    public void SetBound(bool on)
    {
        boundActive = on;
        if (normalCam == null || boundCam == null) return;

        if (on)
        {
            normalCam.Priority = 0;
            boundCam.Priority = 20;
        }
        else
        {
            normalCam.Priority = 20;
            boundCam.Priority = 0;
        }
    }
}
