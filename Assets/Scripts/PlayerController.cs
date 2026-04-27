using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerPhysics))]
public class PlayerController : MonoBehaviour
{
    public GameOverUI gameOverUI; // taruh di atas (di dalam class)

    public enum PlayerState
    {
        WaitingToLaunch,
        Launching,
        Flying,
        Sliding,
        GameOver
    }

    public PlayerState currentState = PlayerState.WaitingToLaunch;

    [Header("Waypoints Lereng Tebing")]
    public Transform[] cliffWaypoints;
    private int currentWaypointIndex = 0;

    [Header("Fisika Meluncur (Slope)")]
    public float initialSpeed = 5f;
    public float slopeAcceleration = 25f;

    [Header("Kontrol Terbang")]
    public float rotationSpeed = 100f;
    public float boostForce = 15f;
    public float boostCooldown = 1.5f;

    private float lastBoostTime;
    private PlayerPhysics physics;

    private float currentRotationAngle;
    private float targetAngle;

    void Start()
    {
        physics = GetComponent<PlayerPhysics>();

        if (cliffWaypoints != null && cliffWaypoints.Length > 0)
        {
            transform.position = cliffWaypoints[0].position;

            if (cliffWaypoints.Length > 1)
            {
                Vector2 dir = (cliffWaypoints[1].position - cliffWaypoints[0].position).normalized;
                currentRotationAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                physics.flightAngle = currentRotationAngle;
                transform.rotation = Quaternion.Euler(0, 0, currentRotationAngle);
            }
        }

        targetAngle = currentRotationAngle;
    }

    void Update()
    {
        switch (currentState)
        {
            case PlayerState.WaitingToLaunch:
                HandleLaunchInput();
                break;

            case PlayerState.Launching:
                UpdateSlideAlongCliff();
                break;

            case PlayerState.Flying:
                HandleAngleInput();
                HandleBoostInput();
                break;

            case PlayerState.Sliding:
                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    Quaternion.Euler(0, 0, 0),
                    5f * Time.deltaTime
                );
                break;
        }
    }

    void FixedUpdate()
    {
        if (currentState == PlayerState.Flying)
        {
            ApplySmoothRotation();
        }
    }

    void HandleLaunchInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            physics.velocity = new Vector2(initialSpeed, -0.5f);
            currentWaypointIndex = 0;

            physics.SetPhysicsActive(false);

            StartCoroutine(SlowMoRoutine(0.3f, 1f));

            currentState = PlayerState.Launching;
        }
    }

    void UpdateSlideAlongCliff()
    {
        if (cliffWaypoints == null || currentWaypointIndex >= cliffWaypoints.Length) return;

        Transform targetWP = cliffWaypoints[currentWaypointIndex];
        Vector2 direction = ((Vector2)targetWP.position - (Vector2)transform.position).normalized;

        float speedFactor = Mathf.Max(physics.velocity.magnitude, 2f);
        physics.velocity += direction * slopeAcceleration * speedFactor * Time.deltaTime;

        float moveStep = physics.velocity.magnitude * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, targetWP.position, moveStep);

        float targetAngleLocal = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion targetRot = Quaternion.Euler(0, 0, targetAngleLocal);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 300f * Time.deltaTime);

        currentRotationAngle = targetAngleLocal;
        physics.flightAngle = currentRotationAngle;

        float distanceToTarget = Vector2.Distance(transform.position, targetWP.position);
        float tolerance = Mathf.Max(0.3f, moveStep * 1.2f);

        if (distanceToTarget < tolerance)
        {
            currentWaypointIndex++;

            if (currentWaypointIndex >= cliffWaypoints.Length)
            {
                LaunchIntoAir(targetAngleLocal);
            }
        }
    }

    void LaunchIntoAir(float finalAngle)
    {
        currentRotationAngle = finalAngle;
        targetAngle = finalAngle;
        physics.flightAngle = currentRotationAngle;

        physics.SetPhysicsActive(true);

        currentState = PlayerState.Flying;

        physics.velocity.y += 2f;

       
    }

    void HandleAngleInput()
    {
        float input = 0;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) input = 1;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) input = -1;

        targetAngle += input * rotationSpeed * Time.deltaTime;
        targetAngle = Mathf.Clamp(targetAngle, -60f, 60f);
    }

    void ApplySmoothRotation()
    {
        currentRotationAngle = Mathf.Lerp(currentRotationAngle, targetAngle, 8f * Time.fixedDeltaTime);

        physics.flightAngle = currentRotationAngle;

        Quaternion targetRot = Quaternion.Euler(0, 0, currentRotationAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 300f * Time.fixedDeltaTime);
    }

    void HandleBoostInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > lastBoostTime + boostCooldown)
        {
            Vector2 boostDirection = transform.right;
            physics.velocity += boostDirection * boostForce;

            lastBoostTime = Time.time;
        }
    }
    public void SetGameOver()
{
    Debug.Log("GAME OVER TRIGGERED");

    currentState = PlayerState.GameOver;

    if (gameOverUI != null)
    {
        gameOverUI.ShowGameOver();
    }
}

    IEnumerator SlowMoRoutine(float scale, float duration)
    {
        Time.timeScale = scale;
        Time.fixedDeltaTime = 0.02f * scale;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}