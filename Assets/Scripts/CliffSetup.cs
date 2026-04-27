using UnityEngine;

// ============================================================
// CliffSetup.cs
// 
// CARA PAKAI BARU (lebih mudah):
//   1. Pasang script ini ke GameObject "Cliff"
//   2. Drag GameObject "LaunchPoint" dan "CliffEdgePoint" ke Inspector
//   3. Di Scene view, kamu akan melihat:
//        - Bola HIJAU besar  = posisi karakter mulai (LaunchPoint)
//        - Bola MERAH besar  = ujung landasan tempat karakter melayang (CliffEdgePoint)
//   4. Klik dan drag bola tersebut langsung di Scene view untuk memindahkannya!
//      (Tidak perlu ubah angka di Inspector)
// ============================================================

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class CliffSetup : MonoBehaviour
{
    [Header("Titik Referensi — drag GameObject ke sini")]
    [Tooltip("Posisi karakter mulai meluncur (di atas lereng)")]
    public Transform launchPoint;

    [Tooltip("Ujung landasan datar — tempat karakter melayang ke udara")]
    public Transform cliffEdgePoint;

    [Header("Collider")]
    [Tooltip("Otomatis sesuaikan collider mengikuti bentuk sprite saat Play")]
    public bool autoFitCollider = true;

    // ============================================================
    // Start()
    // ============================================================
    void Start()
    {
        if (autoFitCollider)
            FitColliderToSprite();
    }

    // ============================================================
    // FitColliderToSprite()
    // ============================================================
    void FitColliderToSprite()
    {
        var sr   = GetComponent<SpriteRenderer>();
        var poly = GetComponent<PolygonCollider2D>();

        if (sr.sprite == null)
        {
            Debug.LogWarning("CliffSetup: Pasang sprite dulu!");
            return;
        }

        int shapeCount = sr.sprite.GetPhysicsShapeCount();
        if (shapeCount == 0)
        {
            Debug.LogWarning("CliffSetup: Buka Sprite Editor > Custom Physics Shape > Generate > Apply.");
            return;
        }

        poly.pathCount = shapeCount;
        var path = new System.Collections.Generic.List<Vector2>();
        for (int i = 0; i < shapeCount; i++)
        {
            path.Clear();
            sr.sprite.GetPhysicsShape(i, path);
            poly.SetPath(i, path.ToArray());
        }

        Debug.Log("CliffSetup: Collider berhasil dibuat dari sprite.");
    }

    // ============================================================
    // OnDrawGizmosSelected() — hanya tampil saat GameObject Cliff dipilih
    // ============================================================
    void OnDrawGizmosSelected()
    {
        float radius = 0.35f;

        if (launchPoint != null)
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.8f);
            Gizmos.DrawSphere(launchPoint.position, radius);
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(launchPoint.position, radius + 0.05f);

#if UNITY_EDITOR
            UnityEditor.Handles.Label(
                launchPoint.position + Vector3.up * (radius + 0.2f),
                "LAUNCH POINT (drag bola ini)"
            );
#endif
        }

        if (cliffEdgePoint != null)
        {
            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.8f);
            Gizmos.DrawSphere(cliffEdgePoint.position, radius);
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(cliffEdgePoint.position, radius + 0.05f);

#if UNITY_EDITOR
            UnityEditor.Handles.Label(
                cliffEdgePoint.position + Vector3.up * (radius + 0.2f),
                "CLIFF EDGE (drag bola ini)"
            );
#endif
        }

        // Garis kuning dari LaunchPoint ke CliffEdgePoint
        if (launchPoint != null && cliffEdgePoint != null)
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
            Gizmos.DrawLine(launchPoint.position, cliffEdgePoint.position);
        }
    }
}