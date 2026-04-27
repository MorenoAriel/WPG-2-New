using UnityEngine;
using System.Collections.Generic;

// ============================================================
// TreeSpawner.cs
// Pasang script ini pada GameObject kosong bernama "TreeSpawner".
//
// CARA SETUP:
//   1. Buat prefab pohon:
//      - Buat sprite pohon (atau pakai aset kamu)
//      - Add Component -> Collider2D (Box/Capsule), centang "Is Trigger"
//      - Add Component -> TreeObstacle
//      - Drag ke folder Prefabs -> jadikan Prefab
//   2. Buat Empty GameObject -> rename "TreeSpawner"
//   3. Add Component -> TreeSpawner
//   4. Drag prefab pohon ke slot "Tree Prefab"
//   5. Drag GameObject "Ground" ke slot "Ground Object"
//      (untuk deteksi posisi Y permukaan tanah)
// ============================================================

public class TreeSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [Tooltip("Prefab pohon yang akan di-spawn")]
    public GameObject treePrefab;

    [Header("Pengaturan Spawn")]
    [Tooltip("Jarak minimum antar pohon")]
    public float minSpacing = 5f;

    [Tooltip("Jarak maksimum antar pohon")]
    public float maxSpacing = 15f;

    [Tooltip("Seberapa jauh ke depan pohon di-spawn dari posisi kamera")]
    public float spawnAheadDistance = 25f;

    [Tooltip("Seberapa jauh di belakang kamera pohon dihapus (untuk hemat memori)")]
    public float despawnBehindDistance = 20f;

    [Header("Posisi Y Pohon")]
    [Tooltip("Posisi Y permukaan tanah. Pohon akan diletakkan tepat di atasnya.")]
    public float groundY = 0f;

    [Tooltip("Offset Y pohon dari permukaan tanah (sesuaikan dengan tinggi sprite pohon)")]
    public float treeYOffset = 1f;

    [Header("Kapan Mulai Spawn")]
    [Tooltip("Posisi X minimum sebelum pohon mulai muncul " +
             "(beri jarak dari ujung tebing agar tidak spawn di tebing)")]
    public float startSpawnX = 10f;

    [Header("Variasi Pohon")]
    [Tooltip("Variasi ukuran pohon. 0 = semua sama, 0.3 = bervariasi 30%")]
    [Range(0f, 0.5f)]
    public float sizeVariation = 0.2f;

    // ============================================================
    // VARIABEL INTERNAL
    // ============================================================

    private Transform playerTransform;
    private UnityEngine.Camera mainCamera;
    private float nextSpawnX;                          // posisi X pohon berikutnya
    private List<GameObject> activeTrees = new List<GameObject>(); // pohon yang aktif

    // ============================================================
    // Start()
    // ============================================================
    void Start()
    {
        // Cari referensi
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        mainCamera = FindFirstObjectByType<UnityEngine.Camera>();

        // Tentukan posisi X pertama untuk spawn
        nextSpawnX = startSpawnX + Random.Range(minSpacing, maxSpacing);
    }

    // ============================================================
    // Update() — cek setiap frame apakah perlu spawn/despawn
    // ============================================================
    void Update()
    {
        if (playerTransform == null || treePrefab == null) return;

        float playerX = playerTransform.position.x;

        // Cek sendiri: belum melewati titik awal = belum perlu spawn
        if (playerX < startSpawnX) return;

        float cameraRightEdge = GetCameraRightEdge();

        while (nextSpawnX < cameraRightEdge + spawnAheadDistance)
        {
            if (nextSpawnX >= startSpawnX)
                SpawnTree(nextSpawnX);

            nextSpawnX += Random.Range(minSpacing, maxSpacing);
        }

        DespawnOldTrees(playerX);
    }

    // ============================================================
    // SpawnTree() — buat satu pohon di posisi X yang ditentukan
    // ============================================================
    void SpawnTree(float posX)
    {
        // Hitung posisi Y: permukaan tanah + offset tinggi pohon
        float posY = groundY + treeYOffset;

        Vector3 spawnPos = new Vector3(posX, posY, 0f);

        // Buat pohon dari prefab
        GameObject tree = Instantiate(treePrefab, spawnPos, Quaternion.identity);

        // Variasi ukuran agar tidak semua pohon terlihat sama
        if (sizeVariation > 0f)
        {
            float randomScale = 1f + Random.Range(-sizeVariation, sizeVariation);
            tree.transform.localScale *= randomScale;
        }

        // Simpan ke list agar bisa dihapus nanti
        activeTrees.Add(tree);

        Debug.Log($"Pohon spawn di X: {posX:F1}");
    }

    // ============================================================
    // DespawnOldTrees() — hapus pohon yang sudah tidak terlihat
    // ============================================================
    void DespawnOldTrees(float playerX)
    {
        // Cari pohon yang sudah jauh di belakang
        for (int i = activeTrees.Count - 1; i >= 0; i--)
        {
            if (activeTrees[i] == null)
            {
                activeTrees.RemoveAt(i);
                continue;
            }

            float treeX = activeTrees[i].transform.position.x;

            if (treeX < playerX - despawnBehindDistance)
            {
                Destroy(activeTrees[i]);
                activeTrees.RemoveAt(i);
            }
        }
    }

    // ============================================================
    // GetCameraRightEdge() — hitung tepi kanan kamera dalam world space
    // ============================================================
    float GetCameraRightEdge()
    {
        if (mainCamera == null) return playerTransform.position.x;

        // Konversi sudut kanan atas layar ke world position
        Vector3 rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1f, 0.5f, 0f));
        return rightEdge.x;
    }

    // ============================================================
    // OnDrawGizmos() — visualisasi area spawn di Scene view
    // ============================================================
    void OnDrawGizmos()
    {
        // Garis merah = batas mulai spawn
        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            new Vector3(startSpawnX, groundY - 2f, 0),
            new Vector3(startSpawnX, groundY + 4f, 0)
        );

#if UNITY_EDITOR
        UnityEditor.Handles.Label(
            new Vector3(startSpawnX, groundY + 4.5f, 0),
            "Mulai Spawn"
        );
#endif

        // Garis hijau = posisi Y tanah
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawLine(
            new Vector3(startSpawnX - 2f, groundY, 0),
            new Vector3(startSpawnX + 30f, groundY, 0)
        );
    }
}