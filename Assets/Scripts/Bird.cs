using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bird : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        // Tetap boleh pakai ini karena kita bantu dengan manual detection
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // 🔥 ANTI TEMBUS (deteksi manual ke depan)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, 0.7f);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}