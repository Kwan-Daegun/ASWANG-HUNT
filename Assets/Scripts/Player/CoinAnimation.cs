using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinAnimation : MonoBehaviour
{
    [Header("Coin Animation Frames")]
    public Sprite[] frames;           
    public float animationSpeed = 0.1f; 

    private SpriteRenderer spriteRenderer;
    private int currentFrame = 0;
    private float frameTimer = 0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        AnimateCoin();
    }

    void AnimateCoin()
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
