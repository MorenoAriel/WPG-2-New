using UnityEngine;

/// <summary>
/// Ship mechanic: Hold = fly up, Release = fall down.
/// Player stays at fixed X; only Y changes.
/// When speed reaches 0, the "pig falls" (gravity takes over, no input).
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class ShipController : MonoBehaviour
{
    [Header("Flight Settings")]
    public float thrustForce = 8f;
    public float maxUpSpeed = 6f;
    public float maxDownSpeed = 6f;
    public float fixedX = -5f; // Posisi X tetap di sebelah kiri layar
    public float rotationSpeed = 10f;
    public float maxUpRotation = 45f;
    public float maxDownRotation = -45f;

    [Header("Boundaries")]
    public UnityEngine.Camera mainCamera;
    public float boundaryOffset = 0.5f; // Offset dari edge kamera

    [Header("Fall (Game Over)")]
    public float fallGravityScale = 4f;   // stronger gravity when dead
    public float fallTorque = -30f;        // spin when falling

    private Rigidbody2D rb;
    private bool canFly = false;
    private bool isDead = false;
    private bool isInvulnerable = false;
    private float topLimit;
    private float bottomLimit;
    private float verticalVelocity = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        // Pastikan player berada di posisi X tetap di sebelah kiri
        Vector3 pos = transform.position;
        pos.x = fixedX;
        transform.position = pos;

        // Setup kamera dan batas
        if (mainCamera == null)
            mainCamera = UnityEngine.Camera.main;

        UpdateCameraBoundaries();
    }

    void UpdateCameraBoundaries()
    {
        if (mainCamera == null) return;

        // Hitung batas atas dan bawah berdasarkan kamera orthographic
        float cameraHeight = mainCamera.orthographicSize * 2f;
        float cameraTop = mainCamera.transform.position.y + mainCamera.orthographicSize - boundaryOffset;
        float cameraBottom = mainCamera.transform.position.y - mainCamera.orthographicSize + boundaryOffset;

        topLimit = cameraTop;
        bottomLimit = cameraBottom;
    }

    void OnEnable()
    {
        GameManager.GameStartedEvent += OnGameStarted;
        GameManager.GameOverEvent += OnGameOver;
    }

    void OnDisable()
    {
        GameManager.GameStartedEvent -= OnGameStarted;
        GameManager.GameOverEvent -= OnGameOver;
    }

    void OnGameStarted()
    {
        canFly = true;
        verticalVelocity = 0f;
        rb.gravityScale = 0f;
    }

    void OnGameOver()
    {
        isDead = true;
        canFly = false;

        // Release all constraints so pig can fall and spin
        rb.constraints = RigidbodyConstraints2D.None;
        rb.gravityScale = fallGravityScale;
        rb.AddTorque(fallTorque);
    }

    void Update()
    {
        if (!canFly || isDead) return;

        // Update boundaries setiap frame jika kamera bergerak
        UpdateCameraBoundaries();

        bool pressing = Input.GetKey(KeyCode.Space);

        // Ramping vertical velocity
        float delta = thrustForce * Time.deltaTime;
        verticalVelocity += pressing ? delta : -delta;
        verticalVelocity = Mathf.Clamp(verticalVelocity, -maxDownSpeed, maxUpSpeed);

        Vector3 pos = transform.position;
        pos.y += verticalVelocity * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, bottomLimit, topLimit);
        pos.x = fixedX;
        transform.position = pos;

        if ((pos.y <= bottomLimit && verticalVelocity < 0f) || (pos.y >= topLimit && verticalVelocity > 0f))
        {
            verticalVelocity = 0f;
        }

        rb.linearVelocity = new Vector2(0f, verticalVelocity);

        float normalizedVelocity = Mathf.InverseLerp(-maxDownSpeed, maxUpSpeed, verticalVelocity);
        float targetAngle = Mathf.Lerp(maxDownRotation, maxUpRotation, normalizedVelocity);
        float zAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, zAngle);
    }

    void HandleObstacleHit(GameObject other)
    {
        if (isDead) return;
        if (isInvulnerable) return;
        if (GameManager.Instance == null) return;

        GameObject obstacle = FindObstacleRoot(other);
        if (obstacle == null) return;

        GameManager.Instance.OnPlayerHit();
        verticalVelocity = 0f;
    }

    GameObject FindObstacleRoot(GameObject other)
    {
        if (other.CompareTag("Obstacle"))
            return other;

        Transform parent = other.transform.parent;
        while (parent != null)
        {
            if (parent.CompareTag("Obstacle"))
                return parent.gameObject;
            parent = parent.parent;
        }

        return null;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[ShipController] OnTriggerEnter2D with '{other.gameObject.name}' tag='{other.gameObject.tag}' trigger={other.isTrigger}");
        HandleObstacleHit(other.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"[ShipController] OnCollisionEnter2D with '{collision.gameObject.name}' tag='{collision.gameObject.tag}'");
        HandleObstacleHit(collision.gameObject);
    }

    // Called externally after speed hits 0 to make pig ragdoll-fall
    public void FallDown()
    {
        OnGameOver();
    }

    // Set invulnerability for bonus game 2
    public void SetInvulnerable(bool value)
    {
        isInvulnerable = value;
        Debug.Log($"[ShipController] Invulnerable set to {value}");
    }
}