using UnityEngine;

public class BackgroundCrossfade : MonoBehaviour
{
    [Header("References")]
    public Transform playerTransform;
    public Transform cameraTransform;

    [Header("Background Layers")]
    public Renderer layer1Renderer;
    public Renderer layer2Renderer;

    [Header("Transition Settings")]
    public float heightThreshold = 20f;
    public float fadeDuration = 1.0f;

    [Header("Parallax Scrolling")]
    public float parallaxFactor = 0.5f;

    private bool isTransitioning = false;
    private float fadeTimer = 0f;
    private Vector3 lastCameraPosition;
    private float texOffsetLayer1 = 0f;
    private float texOffsetLayer2 = 0f;
    private bool isLayer2Active = false;

    void Start()
    {
        if (cameraTransform == null)
        {
            Camera cam = FindFirstObjectByType<Camera>();
            if (cam != null) cameraTransform = cam.transform;
        }

        if (playerTransform == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) playerTransform = p.transform;
        }

        lastCameraPosition = cameraTransform.position;

        transform.position = new Vector3(
            cameraTransform.position.x,
            cameraTransform.position.y,
            transform.position.z
        );

        // BG1 fully visible, BG2 fully transparent
        SetAlpha(layer1Renderer, 1f);
        SetAlpha(layer2Renderer, 0f);

        if (layer1Renderer != null) layer1Renderer.gameObject.SetActive(true);
        if (layer2Renderer != null) layer2Renderer.gameObject.SetActive(true);
        isLayer2Active = false;
    }

    void Update()
    {
        FollowCamera();
        HandleParallax();
        HandleBackgroundSwitch();
    }

    void HandleBackgroundSwitch()
    {
        if (isTransitioning || playerTransform == null) return;

        bool playerAboveThreshold = playerTransform.position.y >= heightThreshold;

        // Transition to layer 2
        if (playerAboveThreshold && !isLayer2Active)
        {
            isLayer2Active = true;
            isTransitioning = true;
            fadeTimer = 0f;
            if (layer2Renderer != null) layer2Renderer.gameObject.SetActive(true);
        }

        // Transition back to layer 1
        if (!playerAboveThreshold && isLayer2Active)
        {
            isLayer2Active = false;
            isTransitioning = true;
            fadeTimer = 0f;
            if (layer1Renderer != null) layer1Renderer.gameObject.SetActive(true);
        }
    }

    void HandleParallax()
    {
        if (cameraTransform == null) return;

        Vector3 delta = cameraTransform.position - lastCameraPosition;
        float speed = (Time.deltaTime > 0f) ? delta.x / Time.deltaTime : 0f;

        float parallax = speed * parallaxFactor * Time.deltaTime;

        texOffsetLayer1 += parallax;
        texOffsetLayer2 += parallax;

        if (layer1Renderer != null)
            layer1Renderer.material.mainTextureOffset = new Vector2(texOffsetLayer1, 0f);

        if (layer2Renderer != null)
            layer2Renderer.material.mainTextureOffset = new Vector2(texOffsetLayer2, 0f);

        lastCameraPosition = cameraTransform.position;

        if (isTransitioning)
        {
            fadeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(fadeTimer / fadeDuration);

            if (isLayer2Active)
            {
                // BG1 fade out, BG2 fade in simultaneously
                SetAlpha(layer1Renderer, Mathf.Lerp(1f, 0f, t));
                SetAlpha(layer2Renderer, Mathf.Lerp(0f, 1f, t));
            }
            else
            {
                // BG2 fade out, BG1 fade in simultaneously
                SetAlpha(layer1Renderer, Mathf.Lerp(0f, 1f, t));
                SetAlpha(layer2Renderer, Mathf.Lerp(1f, 0f, t));
            }

            if (t >= 1f)
            {
                isTransitioning = false;

                if (layer1Renderer != null) layer1Renderer.gameObject.SetActive(true);
                if (layer2Renderer != null) layer2Renderer.gameObject.SetActive(true);
            }
        }
    }

    void FollowCamera()
    {
        if (cameraTransform == null) return;

        transform.position = new Vector3(
            cameraTransform.position.x,
            cameraTransform.position.y,
            transform.position.z
        );
    }

    /// <summary>
    /// Set alpha on a Renderer. Supports Unlit/Transparent, Standard (Fade/Transparent),
    /// and any shader that exposes a _Color or _BaseColor property.
    /// </summary>
    void SetAlpha(Renderer r, float alpha)
    {
        if (r == null) return;

        Material mat = r.material;

        // Try _Color first (Unlit/Transparent, Standard, Sprites/Default, etc.)
        if (mat.HasProperty("_Color"))
        {
            Color c = mat.GetColor("_Color");
            c.a = alpha;
            mat.SetColor("_Color", c);
            return;
        }

        // Fallback: URP/HDRP Lit uses _BaseColor
        if (mat.HasProperty("_BaseColor"))
        {
            Color c = mat.GetColor("_BaseColor");
            c.a = alpha;
            mat.SetColor("_BaseColor", c);
        }
    }
}