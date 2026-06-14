using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPhysics : MonoBehaviour
{
    public enum PhysicsState
    {
        Flying,
        Sliding,
        GameOver
    }

    public PhysicsState currentState = PhysicsState.Flying;

    [Header("Terbang & Gliding")]
    public float gravity = 9.8f;
    public float airResistance = 0.5f;
    public float liftForceMultiplier = 1.2f;
    public float maxSpeed = 50f;
    private bool physicsActive = true;

    [Header("Sliding & Bouncing")]
    public float groundFriction = 8f;
    public float bounceForce = 0.6f;
    public float maxBounceAngle = 20f;
    public LayerMask groundLayer;

    [HideInInspector] public Vector2 velocity;
    [HideInInspector] public float currentSpeed;
    [HideInInspector] public float flightAngle;

    private Rigidbody2D rb;
    private bool isDead = false;

    public AudioSource audioSource;
    public AudioClip bounceSound;
    public AudioClip bounceSound2;

    private PlayerController controller;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PlayerController>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        flightAngle = transform.eulerAngles.z > 180
            ? transform.eulerAngles.z - 360
            : transform.eulerAngles.z;
    }

    void FixedUpdate()
    {
        if (!physicsActive || isDead) return;

        // 🔥 Sinkron state dari PlayerController
        if (controller != null)
        {
            if (controller.currentState == PlayerController.PlayerState.Flying)
                currentState = PhysicsState.Flying;
        }

        if (currentState == PhysicsState.Flying)
        {
            ApplyFlightPhysics();
        }
        else if (currentState == PhysicsState.Sliding)
        {
            ApplySlidingPhysics();
        }

        rb.linearVelocity = velocity;
        currentSpeed = velocity.magnitude;
    }

    void ApplyFlightPhysics()
    {
        float dt = Time.fixedDeltaTime;

        float angleZ = transform.eulerAngles.z > 180
            ? transform.eulerAngles.z - 360
            : transform.eulerAngles.z;

        flightAngle = angleZ;
        float angleRad = angleZ * Mathf.Deg2Rad;
        float lift = Mathf.Sin(angleRad);
        float forward = Mathf.Cos(angleRad);

        // Gravity always applies downward.
        velocity.y -= gravity * dt;

        if (Mathf.Abs(angleZ) <= 10f)
        {
            // Near horizontal: glide slowly downward and lose forward speed.
            float glideLift = Mathf.Clamp01(forward) * liftForceMultiplier * 0.2f;
            velocity.y += glideLift * dt;
            velocity.x -= velocity.x * (airResistance * 0.3f) * dt;
        }
        else if (angleZ > 10f)
        {
            // Climbing: gain altitude while losing forward momentum.
            velocity.y += lift * liftForceMultiplier * dt;
            velocity.x -= Mathf.Abs(lift) * (airResistance + 0.3f) * dt;
        }
        else
        {
            // Diving: accelerate forward and fall faster.
            velocity.x += Mathf.Abs(lift) * liftForceMultiplier * dt;
            velocity.y -= Mathf.Abs(lift) * gravity * 0.2f * dt;
        }

        // Aerodynamic drag on horizontal speed.
        velocity.x -= (velocity.x * airResistance * 0.1f) * dt;
        velocity.x = Mathf.Clamp(velocity.x, 0f, maxSpeed);

        if (velocity.magnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
        }
    }

    void ApplySlidingPhysics()
    {
        velocity.x -= groundFriction * Time.fixedDeltaTime;
        velocity.y = 0;

        if (velocity.x <= 0.1f)
        {
            velocity = Vector2.zero;
            isDead = true;
            currentState = PhysicsState.GameOver;
            Debug.Log("MASUK GAME OVER DARI SLIDING");

            // 🔥 Stop score
           

            // 🔥 Beri tahu controller
            if (controller != null)
                controller.SetGameOver();
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (((1 << col.gameObject.layer) & groundLayer) != 0)
        {
            HandleLanding();
        }
    }

    void HandleLanding()
    {
        float impactAngle = Mathf.Abs(transform.eulerAngles.z);
        if (impactAngle > 180) impactAngle = Mathf.Abs(impactAngle - 360);

        if (impactAngle <= maxBounceAngle && velocity.x > 5f)
        {
            velocity.y = Mathf.Abs(velocity.y) * bounceForce;
            velocity.x *= 0.9f;
            Debug.Log("BOUNCE!");

            if (audioSource != null)
            {
                if (bounceSound != null)
                {
                    audioSource.PlayOneShot(bounceSound);
                }
                if (bounceSound2 != null)
                {
                    audioSource.PlayOneShot(bounceSound2);
                }
            }
        }
        else
        {
            if (velocity.x > 2f)
            {
                currentState = PhysicsState.Sliding;

                if (controller != null)
                    controller.currentState = PlayerController.PlayerState.Sliding;
            }
            else
            {
                currentState = PhysicsState.GameOver;
                Debug.Log("MASUK GAME OVER DARI LANDING");


                if (controller != null)
                    controller.SetGameOver();
            }
        }
    }
    

    public float GetCurrentAngle()
    {
        if (velocity.magnitude > 0.1f)
        {
            return Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        }

        return transform.eulerAngles.z > 180
            ? transform.eulerAngles.z - 360
            : transform.eulerAngles.z;
    }

    public void SetPhysicsActive(bool active)
    {
        physicsActive = active;
        rb.simulated = active;
    }
}