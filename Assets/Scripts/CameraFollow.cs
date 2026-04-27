using UnityEngine;

// ============================================================
// CameraFollow.cs
// Tugas: Kamera mengikuti karakter dengan smooth (halus).
// Pasang script ini pada GameObject Camera utama.
//
// Tips: Jika kamera terlalu "gelisah", naikkan nilai smoothSpeed.
//        Jika kamera terlalu "lambat", turunkan nilai smoothSpeed.
// ============================================================

public class CameraFollow : MonoBehaviour
{
    // ============================================================
    // PENGATURAN
    // ============================================================

    [Header("--- Target ---")]
    [Tooltip("Karakter yang diikuti kamera. Drag Player dari Hierarchy.")]
    public Transform target;

    [Header("--- Kecepatan Mengikuti ---")]
    [Tooltip("Seberapa halus kamera mengikuti. 0.1 = sangat halus, 1.0 = langsung.")]
    [Range(0.05f, 1f)]
    public float smoothSpeed = 0.125f;

    [Header("--- Offset (Jarak Kamera dari Karakter) ---")]
    [Tooltip("Geser kamera dari karakter. z harus negatif (misalnya -10) agar kamera 'melihat' scene.")]
    public Vector3 offset = new Vector3(3f, 1f, -10f);

    [Header("--- Batas Kamera ---")]
    [Tooltip("Apakah kamera memiliki batas bawah (tidak boleh turun lebih rendah dari tanah)?")]
    public bool useBoundary = true;

    [Tooltip("Posisi Y minimum kamera (supaya tidak turun ke bawah tanah).")]
    public float minY = -2f;

    // ============================================================
    // VARIABEL INTERNAL
    // ============================================================

    private Vector3 velocity = Vector3.zero; // Untuk SmoothDamp

    // ============================================================
    // LateUpdate() - Untuk kamera, gunakan LateUpdate.
    // LateUpdate dipanggil SETELAH Update(), jadi kamera mengikuti
    // posisi karakter yang sudah diperbarui.
    // ============================================================
    void LateUpdate()
    {
        if (target == null) return;

        // Hitung posisi yang diinginkan (posisi target + offset)
        Vector3 desiredPosition = target.position + offset;

        // Terapkan batas Y jika aktif
        if (useBoundary)
        {
            desiredPosition.y = Mathf.Max(desiredPosition.y, minY);
        }

        // Smooth movement menggunakan SmoothDamp
        // SmoothDamp bergerak perlahan ke target secara otomatis
        Vector3 smoothedPosition = Vector3.SmoothDamp(
            transform.position,    // Posisi saat ini
            desiredPosition,       // Posisi tujuan
            ref velocity,          // Referensi kecepatan (diisi otomatis)
            smoothSpeed            // Waktu untuk mencapai tujuan
        );

        // Terapkan posisi baru ke kamera
        transform.position = smoothedPosition;
    }

    // ============================================================
    // SetTarget() - Ubah target kamera dari script lain
    // ============================================================
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
