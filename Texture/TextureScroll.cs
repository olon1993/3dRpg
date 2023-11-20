using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureScroll : MonoBehaviour
{
    [SerializeField] private float _scrollX = 0.5f;
    [SerializeField] private float _scrollY = 0.5f;

    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float OffsetX = Time.time * _scrollX;
        float OffsetY = Time.time * _scrollY;

        _renderer.material.mainTextureOffset = new Vector2(OffsetX, OffsetY);
    }
}
