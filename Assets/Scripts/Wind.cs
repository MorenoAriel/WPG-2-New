using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Wind : MonoBehaviour
{
    public float speed = 5f;
    public float boostPower = 3f; // seberapa besar tambahan kecepatan

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // 🔥 DETEKSI DEPAN (sama seperti burung)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, 0.7f);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            ApplyBoost(hit.collider);
            Destroy(gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            ApplyBoost(col);
            Destroy(gameObject);
        }
    }

    void ApplyBoost(Collider2D col)
    {
        PlayerPhysics physics = col.GetComponent<PlayerPhysics>();

        if (physics != null)
        {
            physics.velocity.x += boostPower;
            Debug.Log("KENA ANGIN! Speed nambah 🚀");
        }
    }
}