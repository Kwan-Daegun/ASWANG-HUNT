using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BossAnimationFrames : MonoBehaviour
{
    [Header("Animation Frames")]
    public Sprite[] idleFrames;
    public Sprite[] walkFrames;
    public Sprite[] jumpFrames;
    public Sprite[] flyingFrames;
    public Sprite[] clawAttackFrames;
    public Sprite[] tongueAttackFrames;
    public Sprite[] roofSmashFrames;
    public Sprite[] swoopClawFrames;

    [Header("Animation Settings")]
    public float frameRate = 0.1f; // seconds per frame

    [Header("References")]
    public BossController boss;

    private SpriteRenderer spriteRenderer;

    // Internal tracking
    private float frameTimer = 0f;
    private int currentFrame = 0;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (boss == null)
            boss = GetComponent<BossController>();
    }

    void Update()
    {
        if (boss == null) return;

        AnimateBoss();
        FlipSprite();
    }

    void AnimateBoss()
    {
        Sprite[] currentAnimation = GetCurrentAnimation();

        if (currentAnimation.Length == 0) return;

        frameTimer += Time.deltaTime;

        if (frameTimer >= frameRate)
        {
            frameTimer = 0f;
            currentFrame = (currentFrame + 1) % currentAnimation.Length;
            spriteRenderer.sprite = currentAnimation[currentFrame];
        }
    }

    Sprite[] GetCurrentAnimation()
    {
        if (boss.isAttacking)
        {
            string attackType = GetCurrentAttackType();
            switch (attackType)
            {
                case "Claw": return clawAttackFrames;
                case "Tongue": return tongueAttackFrames;
                case "RoofSmash": return roofSmashFrames;
                case "SwoopClaw": return swoopClawFrames;
            }
        }
        else if (boss.isFlying)
        {
            return flyingFrames;
        }
        else
        {
            if (!boss.isFlying && Mathf.Abs(boss.rb.velocity.y) > 0.1f)
                return jumpFrames;
            if (Mathf.Abs(boss.rb.velocity.x) > 0.1f)
                return walkFrames;
            return idleFrames;
        }

        return idleFrames;
    }

    string GetCurrentAttackType()
    {
        // Simplest way: use BossController's isFlying for SwoopClaw
        if (boss.isAttacking)
        {
            return boss.isFlying ? "SwoopClaw" : "Claw"; // fallback
        }
        return "";
    }

    void FlipSprite()
    {
        if (boss.currentTarget != null)
        {
            if (boss.currentTarget.position.x > transform.position.x)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
