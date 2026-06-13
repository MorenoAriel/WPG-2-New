using UnityEngine;

/// <summary>
/// Attach to each obstacle prefab.
/// Moves left at its own independent speed.
/// </summary>
public class ObstacleMover : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float destroyX = -15f;

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (!GameManager.Instance.gameStarted) return;

        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.World);

        if (transform.position.x < destroyX)
            Destroy(gameObject);
    }
}