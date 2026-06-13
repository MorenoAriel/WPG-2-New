using UnityEngine;
using System.Collections;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Prefabs")]
    public GameObject stalactitePrefab;
    public GameObject stalagmitePrefab;

    [Header("Spawn Settings")]
    public float spawnIntervalMin = 2f;
    public float spawnIntervalMax = 4f;
    public float spawnX           = 12f;

    [Header("Position Settings")]
    public float stalactiteY  = 4f;
    public float stalagmiteY  = -4f;

    [Header("Warning Settings")]
    public float warningDuration = 1.2f;  // berapa detik warning tampil sebelum spawn

    private bool spawning    = false;
    private int  nextObstacle = 0;

    void OnEnable()
    {
        GameManager.GameStartedEvent += StartSpawning;
        GameManager.GameOverEvent    += StopSpawning;
    }

    void OnDisable()
    {
        GameManager.GameStartedEvent -= StartSpawning;
        GameManager.GameOverEvent    -= StopSpawning;
    }

    void StartSpawning() => StartCoroutine(SpawnLoop());

    public void StopSpawning()
    {
        spawning = false;
        StopAllCoroutines();
    }

    IEnumerator SpawnLoop()
    {
        spawning = true;
        while (spawning)
        {
            float wait = Random.Range(spawnIntervalMin, spawnIntervalMax);

            // Pastikan wait lebih panjang dari warningDuration
            float waitBeforeWarning = Mathf.Max(0f, wait - warningDuration);
            yield return new WaitForSeconds(waitBeforeWarning);

            // Tentukan Y obstacle berikutnya untuk posisi warning
            float nextY = nextObstacle == 0 ? stalactiteY : stalagmiteY;

            // Tampilkan warning
            if (WarningManager.Instance != null)
                WarningManager.Instance.ShowWarning(nextY, warningDuration);

            // Tunggu lalu spawn
            yield return new WaitForSeconds(warningDuration);

            if (nextObstacle == 0) SpawnStalactite();
            else                   SpawnStalagmite();

            nextObstacle = 1 - nextObstacle;
        }
    }

    void SpawnStalactite()
    {
        if (stalactitePrefab == null) return;
        Instantiate(stalactitePrefab,
            new Vector3(spawnX, stalactiteY, 0f),
            Quaternion.Euler(0f, 0f, -90f));
    }

    void SpawnStalagmite()
    {
        if (stalagmitePrefab == null) return;
        Instantiate(stalagmitePrefab,
            new Vector3(spawnX, stalagmiteY, 0f),
            Quaternion.Euler(0f, 0f, 90f));
    }
}