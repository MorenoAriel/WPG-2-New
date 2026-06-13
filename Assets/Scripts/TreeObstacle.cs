using UnityEngine;

public class TreeObstacle : MonoBehaviour
{
    [Tooltip("Seberapa besar pohon ini mengurangi kecepatan karakter saat ditabrak")]
    public float speedPenalty = 5f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerPhysics physics = other.GetComponent<PlayerPhysics>();
        if (physics == null) return;

        // 🔥 Ganti check GameManager:
        // Anggap valid jika player masih bergerak (flying / sliding)
        // dan belum mati
        if (physics.velocity.magnitude <= 0.1f) return;

        // Kurangi kecepatan
        physics.velocity.x = Mathf.Max(physics.velocity.x - speedPenalty, 0f);
    }
}