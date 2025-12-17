using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TikbalangAnimation : MonoBehaviour
{
   [Header("Walk Animation")]
    public Sprite[] walkFrames;          // Walking frames
    public float walkAnimationSpeed = 0.1f; // Seconds per frame

    [Header("Jump Animation")]
    public Sprite jumpFrame;             // Single jump frame

    [Header("References")]
    public Rigidbody2D rb;               // Assign Tikbalang's Rigidbody2D
    private SpriteRenderer spriteRenderer;

    // Internal variables
    private int currentWalkFrame = 0;
    private float walkFrameTimer = 0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Auto-assign Rigidbody if not set
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        AnimateTikbalang();
        FlipSprite();
    }

    void AnimateTikbalang()
    {
        if (rb == null) return;

        // Check if Tikbalang is in the air
        bool isJumping = Mathf.Abs(rb.velocity.y) > 0.01f;

        if (isJumping)
        {
            // Use jump frame while in air
            spriteRenderer.sprite = jumpFrame;
        }
        else
        {
            // Walk animation
            if (walkFrames.Length == 0) return;

            walkFrameTimer += Time.deltaTime;

            if (walkFrameTimer >= walkAnimationSpeed)
            {
                walkFrameTimer = 0f;
                currentWalkFrame = (currentWalkFrame + 1) % walkFrames.Length;
                spriteRenderer.sprite = walkFrames[currentWalkFrame];
            }
        }
    }

    void FlipSprite()
    {
        // Flip sprite based on horizontal velocity
        if (rb.velocity.x > 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (rb.velocity.x < 0)
            transform.localScale = new Vector3(1, 1, 1);
    }
}
