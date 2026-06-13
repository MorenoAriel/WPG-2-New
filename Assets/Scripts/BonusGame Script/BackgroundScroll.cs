using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundScroll : MonoBehaviour
{
    public float scrollSpeed;

    [SerializeField] private Renderer backgroundRenderer;
    void Update()
    {
        backgroundRenderer.material.mainTextureOffset += new Vector2(scrollSpeed * Time.deltaTime, 0f);
    }
}
