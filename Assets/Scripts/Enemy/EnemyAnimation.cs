using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class EnemyAnimation : MonoBehaviour
{
    [Header("Walk Animation")]
    public Sprite[] walkSprites;     
    public float animationSpeed = 0.1f; 

    private SpriteRenderer spriteRenderer;
    private int currentFrame = 0;
    private float frameTimer = 0f;

    [Header("Movement")]
    public Rigidbody2D rb; 

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        AnimateWalk();
        FlipSprite();
    }

    void AnimateWalk()
    {
        if (walkSprites.Length == 0) return;

        
        if (rb.velocity.x != 0)
        {
            frameTimer += Time.deltaTime;
            if (frameTimer >= animationSpeed)
            {
                frameTimer = 0f;
                currentFrame = (currentFrame + 1) % walkSprites.Length;
                spriteRenderer.sprite = walkSprites[currentFrame];
            }
        }
        else
        {
            
            currentFrame = 0;
            spriteRenderer.sprite = walkSprites[currentFrame];
        }
    }

    void FlipSprite()
    {
        if (rb.velocity.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (rb.velocity.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }
}
