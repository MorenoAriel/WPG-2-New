using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WarningManager : MonoBehaviour
{
    public static WarningManager Instance;

    [Header("References")]
    public Canvas     canvas;
    public UnityEngine.Camera mainCamera;
    
    [Header("Warning Icon")]
    public Sprite     warningSprite;   // assign sprite tanda seru di Inspector
    public Color      warningColor = Color.red;
    public Vector2    iconSize = new Vector2(60f, 60f);

    [Header("Settings")]
    public float blinkRate      = 0.15f;  // kecepatan kedip
    public float edgeOffsetX    = 40f;    // jarak dari tepi kanan layar

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Tampilkan warning di tepi kanan kamera pada posisi Y obstacle.
    /// duration = berapa lama warning tampil sebelum obstacle spawn.
    /// </summary>
    public void ShowWarning(float worldY, float duration)
    {
        StartCoroutine(WarningRoutine(worldY, duration));
    }

    IEnumerator WarningRoutine(float worldY, float duration)
    {
        // Buat icon warning
        GameObject obj = new GameObject("WarningIcon");
        obj.transform.SetParent(canvas.transform, false);

        Image img       = obj.AddComponent<Image>();
        img.sprite      = warningSprite;
        img.color       = warningColor;
        img.raycastTarget = false;

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.sizeDelta     = iconSize;

        // Posisi: tepi kanan kamera, Y sesuai obstacle
        PositionAtRightEdge(rt, worldY);

        // Kedip selama duration
        float elapsed = 0f;
        while (elapsed < duration)
        {
            img.enabled = !img.enabled;
            yield return new WaitForSeconds(blinkRate);
            elapsed += blinkRate;
        }

        Destroy(obj);
    }

    void PositionAtRightEdge(RectTransform rt, float worldY)
    {
        if (mainCamera == null) return;

        float rightEdgeX = mainCamera.transform.position.x
                        + mainCamera.orthographicSize * mainCamera.aspect;

        Vector3 worldPos  = new Vector3(rightEdgeX, worldY, 0f);
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
            out Vector2 localPos
        );

        localPos.x -= edgeOffsetX;
        rt.localPosition = localPos;
    }
}