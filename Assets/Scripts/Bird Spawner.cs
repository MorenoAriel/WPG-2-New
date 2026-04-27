using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    [Header("Referensi")]
    public Transform player;
    public GameObject birdPrefab;
    public GameObject windPrefab;
    public GameObject cloudPrefab;

    [Header("Pengaturan Spawn")]
    public float spawnOffsetX = 10f;
    public float minY = -2f;
    public float maxY = 2f;

    [Header("Kontrol Jumlah")]
    public float spawnDistance = 5f;

    private float lastSpawnX;
    private bool isSpawning = false;

    void Start()
    {
        if (player != null)
        {
            lastSpawnX = player.position.x;
        }
    }

    void Update()
    {
        if (player == null) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isSpawning = true;
        }

        if (!isSpawning) return;

        float distanceMoved = player.position.x - lastSpawnX;

        if (distanceMoved >= spawnDistance)
        {
            SpawnRandomObject();
            lastSpawnX = player.position.x;
        }
    }

    void SpawnRandomObject()
    {
        float spawnX = player.position.x + spawnOffsetX;
        float spawnY = player.position.y + Random.Range(minY, maxY);

        Vector2 spawnPos = new Vector2(spawnX, spawnY);

        int rand = Random.Range(0, 3); // 0,1,2

        switch (rand)
        {
            case 0:
                Instantiate(birdPrefab, spawnPos, Quaternion.identity);
                Debug.Log("Spawn Bird");
                break;

            case 1:
                Instantiate(windPrefab, spawnPos, Quaternion.identity);
                Debug.Log("Spawn Wind");
                break;

            case 2:
                Instantiate(cloudPrefab, spawnPos, Quaternion.identity);
                Debug.Log("Spawn Cloud");
                break;
        }
    }
}