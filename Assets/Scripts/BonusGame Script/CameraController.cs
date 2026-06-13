using UnityEngine;

/// <summary>
/// Camera stays at fixed X offset to the left of the player's start position.
/// Only tracks player on Y axis (smoothed).
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    public bool lockCameraPosition = true; // jika true, kamera diam di tempat

    void Start()
    {
        if (!lockCameraPosition) return;
        // Jangan mengikuti player. Kamera tetap di posisi awal.
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    void LateUpdate()
    {
        if (lockCameraPosition)
            return;
    }
}