using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnimation : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite[] idleSprites;
    [SerializeField] private Sprite[] walkSprites;
    [SerializeField] private Sprite[] shootSprites;
    [SerializeField] private Sprite[] meleeSprites;

    [Header("Speeds")]
    [SerializeField] private float animationSpeed = 0.12f;
    [SerializeField] private float shootAnimSpeed = 0.06f;
    [SerializeField] private float meleeAnimSpeed = 0.07f;

    private SpriteRenderer sr;
    private Rigidbody2D rb;

    private float timer;
    private int frame;

    private bool isShooting;
    private bool isMelee;
    private float actionTimer;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isMelee)
        {
            PlayMelee();
            return;
        }

        if (isShooting)
        {
            PlayShoot();
            return;
        }

        PlayMovement();
    }

    private void PlayMovement()
    {
        bool isWalking = Mathf.Abs(rb.velocity.x) > 0.05f;

        timer += Time.deltaTime;
        if (timer >= animationSpeed)
        {
            timer = 0f;
            frame++;
        }

        if (isWalking && walkSprites.Length > 0)
        {
            frame %= walkSprites.Length;
            sr.sprite = walkSprites[frame];
        }
        else if (idleSprites.Length > 0)
        {
            frame %= idleSprites.Length;
            sr.sprite = idleSprites[frame];
        }
    }

    private void PlayShoot()
    {
        actionTimer += Time.deltaTime;
        if (actionTimer >= shootAnimSpeed)
        {
            actionTimer = 0f;
            frame++;
        }

        if (shootSprites.Length == 0 || frame >= shootSprites.Length)
        {
            isShooting = false;
            frame = 0;
            return;
        }

        sr.sprite = shootSprites[frame];
    }

    private void PlayMelee()
    {
        actionTimer += Time.deltaTime;
        if (actionTimer >= meleeAnimSpeed)
        {
            actionTimer = 0f;
            frame++;
        }

        if (meleeSprites.Length == 0 || frame >= meleeSprites.Length)
        {
            isMelee = false;
            frame = 0;
            return;
        }

        sr.sprite = meleeSprites[frame];
    }

    public void PlayShootAnimation()
    {
        if (shootSprites.Length == 0) return;

        isShooting = true;
        isMelee = false;
        frame = 0;
        timer = 0f;
        actionTimer = 0f;
    }

    public void PlayMeleeAnimation()
    {
        if (meleeSprites.Length == 0) return;

        isMelee = true;
        isShooting = false;
        frame = 0;
        timer = 0f;
        actionTimer = 0f;
    }
}
