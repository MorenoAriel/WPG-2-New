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

    private PlayerController controller;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PlayerController>();

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
        velocity.y -= gravity * Time.fixedDeltaTime;

        float angleZ = flightAngle;

        if (angleZ > -10f)
        {
            float liftAmount = Mathf.Cos(angleZ * Mathf.Deg2Rad) * liftForceMultiplier;
            velocity.y += liftAmount * (velocity.x / 10f) * Time.fixedDeltaTime;
        }

        velocity.x -= (velocity.x * airResistance * 0.1f) * Time.fixedDeltaTime;

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