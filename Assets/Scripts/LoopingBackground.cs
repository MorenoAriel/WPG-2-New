using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    public float backgroundSpeed; // Kecepatan pergerakan background
    public Renderer backgroundRenderer; // Renderer dari background
    // Update is called once per frame
    void Update()
    {
        backgroundRenderer.material.mainTextureOffset += new Vector2(backgroundSpeed * Time.deltaTime, 0f);
    }
}
