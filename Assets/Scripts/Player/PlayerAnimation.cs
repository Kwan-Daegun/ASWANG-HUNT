using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] idleSprites;
    [SerializeField] private Sprite[] walkSprites;
    [SerializeField] private Sprite[] shootSprites;
    [SerializeField] private float animationSpeed = 0.12f;
    [SerializeField] private float shootAnimSpeed = 0.06f;

    private SpriteRenderer sr;
    private Rigidbody2D rb;

    private float timer;
    private int frame;

    private bool isShooting;
    private float shootTimer;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isShooting)
        {
            PlayShoot();
            return;
        }

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
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootAnimSpeed)
        {
            shootTimer = 0f;
            frame++;
        }

        if (shootSprites.Length == 0)
        {
            isShooting = false;
            return;
        }

        if (frame >= shootSprites.Length)
        {
            isShooting = false;
            frame = 0;
            return;
        }

        sr.sprite = shootSprites[frame];
    }

    public void PlayShootAnimation()
    {
        if (shootSprites.Length == 0) return;

        isShooting = true;
        frame = 0;
        timer = 0f;
        shootTimer = 0f;
    }
}
