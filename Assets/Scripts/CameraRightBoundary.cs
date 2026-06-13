using UnityEngine;
using Cinemachine;

public class CameraRightBoundary : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    [Tooltip("Posisi X maksimum kamera (batas kanan)")]
    public float maxX = 30f;

    void LateUpdate()
    {
        if (vcam == null) return;

        // Ambil posisi vcam saat ini
        Vector3 pos = vcam.transform.position;

        // Batasi hanya X (sisi kanan)
        if (pos.x > maxX)
        {
            pos.x = maxX;
            vcam.transform.position = pos;
        }
    }
}