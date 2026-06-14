using UnityEngine;
using TMPro;
using System.Collections;

public class BonusGameStageTwo : MonoBehaviour
{
    [Header("Collectable Prefabs")]
    public GameObject collectableTypeA;
    public GameObject collectableTypeB;

    [Header("Spawn Settings")]
    public float spawnIntervalMin = 1.5f;
    public float spawnIntervalMax = 3.0f;
    public float spawnX = 12f;

    [Header("Y Bounds")]
    public float spawnYMin = -4.5f;
    public float spawnYMax = 4.5f;

    [Header("Collectable UI")]
    public GameObject objectCountPanel;
    public TMP_Text typeAText;
    public TMP_Text typeBText;

    [Header("Collectable Targets")]
    public int targetTypeA = 5;
    public int targetTypeB = 5;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float destroyX = -15f;

    [Header("Win UI")]
    public GameObject winPanel;

    private int collectedTypeA = 0;
    private int collectedTypeB = 0;
    private bool stageActive = false;
    private bool spawning = false;
    private int nextCollectable = 0;

    void Start()
    {
        UpdateUI();

        if (objectCountPanel != null)
            objectCountPanel.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(false);
    }

    public void StartStageTwo()
    {
        if (stageActive) return;

        stageActive = true;
        collectedTypeA = 0;
        collectedTypeB = 0;
        UpdateUI();

        if (objectCountPanel != null)
            objectCountPanel.SetActive(true);

        spawning = true;
        StartCoroutine(SpawnLoop());
    }

    public void StopStageTwo()
    {
        stageActive = false;
        spawning = false;
        StopAllCoroutines();
    }

    IEnumerator SpawnLoop()
    {
        while (spawning)
        {
            float wait = Random.Range(spawnIntervalMin, spawnIntervalMax);
            yield return new WaitForSeconds(wait);
            SpawnNextCollectable();
            nextCollectable = 1 - nextCollectable;
        }
    }

    void SpawnNextCollectable()
    {
        GameObject prefab = nextCollectable == 0 ? collectableTypeA : collectableTypeB;
        if (prefab == null) return;

        float spawnY = Random.Range(spawnYMin, spawnYMax);
        GameObject spawned = Instantiate(prefab, new Vector3(spawnX, spawnY, 0f), Quaternion.identity);

        CollectableMover mover = spawned.GetComponent<CollectableMover>();
        if (mover == null) mover = spawned.AddComponent<CollectableMover>();
        mover.moveSpeed = moveSpeed;
        mover.destroyX = destroyX;

        CollectableItem item = spawned.GetComponent<CollectableItem>();
        if (item == null) item = spawned.AddComponent<CollectableItem>();
        item.SetType(nextCollectable == 0 ? CollectableItem.CollectableType.TypeA : CollectableItem.CollectableType.TypeB);
        item.stageTwoController = this;
    }

    public void RegisterCollectable(CollectableItem.CollectableType type)
    {
        if (!stageActive) return;

        if (type == CollectableItem.CollectableType.TypeA)
            collectedTypeA = Mathf.Min(collectedTypeA + 1, targetTypeA);
        else
            collectedTypeB = Mathf.Min(collectedTypeB + 1, targetTypeB);

        UpdateUI();

        if (collectedTypeA >= targetTypeA && collectedTypeB >= targetTypeB)
            CompleteStageTwo();
    }

    void UpdateUI()
    {
        if (typeAText != null)
            typeAText.text = $"{collectedTypeA} / {targetTypeA}";
        if (typeBText != null)
            typeBText.text = $"{collectedTypeB} / {targetTypeB}";
    }

    void CompleteStageTwo()
    {
        if (!stageActive) return;

        stageActive = false;
        spawning = false;
        StopAllCoroutines();

        Debug.Log("[BonusGameStageTwo] Stage 2 selesai! Menampilkan panel menang.");

        if (winPanel != null)
            winPanel.SetActive(true);
        else
            Debug.LogWarning("[BonusGameStageTwo] winPanel belum di-assign di Inspector!");

        BonusGameTimer.Instance?.NotifyStageTwoComplete();
    }
}