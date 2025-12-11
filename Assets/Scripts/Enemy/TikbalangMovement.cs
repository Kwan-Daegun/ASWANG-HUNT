using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TikbalangMovement : MonoBehaviour
{
    [Header("Tikbalang Stats")]
    public float moveSpeed = 3f;
    public float jumpForce = 8f;   // How high it jumps
    public float stompForce = 12f; // Higher jump for attacking
    public int stompDamage = 20;   // Heavy damage

    [Header("AI Settings")]
    public float attackRange = 2.5f; // Distance to start stomping
    public float jumpInterval = 2f;  // Time between jumps

    [Header("Item Drop Settings")]
    public GameObject coinPrefab;
    public GameObject ammoPrefab;
    [Range(0f, 100f)] public float coinDropChance = 80f; // Higher chance for harder enemy
    [Range(0f, 100f)] public float ammoDropChance = 50f;

    private Vector2 target;
    private Rigidbody2D rb;
    private float jumpTimer;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        jumpTimer = jumpInterval;
    }

    public void SetTarget(Vector2 targetPos)
    {
        target = targetPos;
    }

    void Update()
    {
        jumpTimer -= Time.deltaTime;

        // Check distance to house/target
        float distanceToTarget = Vector2.Distance(transform.position, target);

        // Only do AI if we are on the ground (ready to jump)
        if (isGrounded && jumpTimer <= 0)
        {
            if (distanceToTarget <= attackRange)
            {
                // CLOSE RANGE: STOMP ATTACK (Jump straight up)
                PerformStomp();
            }
            else
            {
                // FAR RANGE: JUMP TOWARDS TARGET
                PerformMoveJump();
            }

            // Reset timer
            jumpTimer = jumpInterval;
            isGrounded = false; // We just jumped
        }
    }

    void PerformMoveJump()
    {
        // Calculate direction (-1 for left, 1 for right)
        float direction = (target.x < transform.position.x) ? -1f : 1f;

        // Face that direction
        transform.localScale = new Vector3(direction > 0 ? -1 : 1, 1, 1); // Adjust based on your sprite

        // Jump Forward (Up + Sideways)
        rb.velocity = new Vector2(direction * moveSpeed, jumpForce);
    }

    void PerformStomp()
    {
        // Zero out horizontal speed to jump straight up
        rb.velocity = new Vector2(0, stompForce);
        Debug.Log("Tikbalang performs HIGH JUMP STOMP!");
    }

    // --- COLLISION / LANDING LOGIC ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // --- LANDING CHECK ---
        if (collision.contactCount > 0)
        {
            ContactPoint2D contact = collision.contacts[0];

            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                rb.velocity = Vector2.zero;
            }
        }

        // --- DAMAGE CHECK ---
        if (collision.gameObject.CompareTag("House") || collision.gameObject.CompareTag("Player"))
        {
            HP hpScript = collision.gameObject.GetComponent<HP>();
            if (hpScript != null)
            {
                hpScript.SubHealth(stompDamage);
                Debug.Log("Tikbalang STOMPED target for " + stompDamage);
            }
        }
    }

    public void Die()
    {
        TryDropItems();
        Destroy(gameObject);
    }

    void TryDropItems()
    {
        if (coinPrefab != null && Random.Range(0f, 100f) < coinDropChance)
            Instantiate(coinPrefab, transform.position, Quaternion.identity);

        if (ammoPrefab != null && Random.Range(0f, 100f) < ammoDropChance)
            Instantiate(ammoPrefab, transform.position, Quaternion.identity);
    }
}
