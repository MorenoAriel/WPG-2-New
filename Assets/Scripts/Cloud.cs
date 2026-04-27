using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Cloud : MonoBehaviour
{
    public float speed = 5f;
    public float slowPower = 3f; // seberapa besar pengurangan speed

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // 🔥 SAMA PERSIS seperti Bird
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, 0.7f);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            ApplySlow(hit.collider);
            Destroy(gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            ApplySlow(col);
            Destroy(gameObject);
        }
    }

    void ApplySlow(Collider2D col)
    {
        PlayerPhysics physics = col.GetComponent<PlayerPhysics>();

        if (physics != null)
        {
            physics.velocity.x = Mathf.Max(physics.velocity.x - slowPower, 0f);
            Debug.Log("KENA AWAN! Speed berkurang 🌫");
        }
    }
}