using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Attach this script to any scene GameObject. No dedicated UI manager is required.
public class ProgressionBarUI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Image distanceBar;
    public Image heightBar;
    public Slider distanceSlider;
    public Slider heightSlider;
    public TMP_Text distanceLabel;
    public TMP_Text heightLabel;

    [Header("Progress Settings")]
    public float groundY = 0f;
    public float maxDistance = 100f;
    public float maxHeight = 50f;
    public bool useStartPositionAsZero = true;
    public float startX = 0f;

    private float initialPlayerX;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        initialPlayerX = player != null ? player.position.x : 0f;
        if (!useStartPositionAsZero)
            initialPlayerX = startX;

        ResetBars();
    }

    void Update()
    {
        if (player == null) return;

        float distanceProgress = Mathf.Clamp01(GetDistanceProgress());
        float heightProgress = Mathf.Clamp01(GetHeightProgress());

        if (distanceBar != null)
            distanceBar.fillAmount = distanceProgress;
        else if (distanceSlider != null)
            distanceSlider.value = distanceProgress;

        if (heightBar != null)
            heightBar.fillAmount = heightProgress;
        else if (heightSlider != null)
            heightSlider.value = heightProgress;

        UpdateLabels(distanceProgress, heightProgress);
    }

    float GetDistanceProgress()
    {
        float traveled = Mathf.Max(0f, player.position.x - initialPlayerX);
        return maxDistance > 0f ? traveled / maxDistance : 0f;
    }

    float GetHeightProgress()
    {
        float height = Mathf.Max(0f, player.position.y - groundY);
        return maxHeight > 0f ? height / maxHeight : 0f;
    }

    void UpdateLabels(float distanceProgress, float heightProgress)
    {
        if (distanceLabel != null)
        {
            float traveled = Mathf.Max(0f, player.position.x - initialPlayerX);
            distanceLabel.text = $"Distance: {traveled:F0} / {maxDistance:F0}";
        }

        if (heightLabel != null)
        {
            float height = Mathf.Max(0f, player.position.y - groundY);
            heightLabel.text = $"Height: {height:F0} / {maxHeight:F0}";
        }
    }

    public void ResetBars()
    {
        if (player != null && useStartPositionAsZero)
            initialPlayerX = player.position.x;

        if (distanceBar != null)
            distanceBar.fillAmount = 0f;
        if (distanceSlider != null)
            distanceSlider.value = 0f;

        if (heightBar != null)
            heightBar.fillAmount = 0f;
        if (heightSlider != null)
            heightSlider.value = 0f;
    }
}
