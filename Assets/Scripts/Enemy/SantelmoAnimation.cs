using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SantelmoAnimation : MonoBehaviour
{
    [Header("Animation Frames")]
    public Sprite[] frames;             // Assign your Santelmo frames here
    public float animationSpeed = 0.1f; // Seconds per frame

    private SpriteRenderer spriteRenderer;
    private int currentFrame = 0;
    private float frameTimer = 0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        AnimateSantelmo();
    }

    void AnimateSantelmo()
    {
        if (frames.Length == 0) return;

        frameTimer += Time.deltaTime;

        if (frameTimer >= animationSpeed)
        {
            frameTimer = 0f;
            currentFrame = (currentFrame + 1) % frames.Length;
            spriteRenderer.sprite = frames[currentFrame];
        }
    }
}
