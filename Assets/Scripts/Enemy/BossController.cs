using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 3f;
    public float flySpeed = 5f;
    public float currentHealth;
    public float maxHealth = 100f;

    [Header("Attack Settings")]
    public float meleeRange = 1.5f;
    public float tongueRange = 6f;
    public int attacksToSwitchTarget = 3; // Switch target every 3 attacks

    [Header("Flight Settings")]
    public float hoverAmplitude = 0.5f;
    public float hoverFrequency = 2f;
    public float hoverSideOffset = 4f; // How far to the left/right it hovers

    [Header("References")]
    public Transform player;
    public Transform houseCenter;
    public LayerMask groundLayer;
    public Rigidbody2D rb;

    [Header("Status")]
    public bool isFlying = false;
    public bool isOnRoof = false;
    public float attackCooldown = 0f;
    public bool isAttacking = false;

    // AI Logic State
    public Transform currentTarget;
    private int attackCounter = 0;
    private float currentHoverSide = 1f; // 1 = Right, -1 = Left

    private enum State { Idle, Chasing, Attacking, Transitioning }
    private State currentState = State.Idle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        // Start by targeting the player
        currentTarget = player;

        // Pick a random side to start hovering
        PickRandomSide();
    }

    void Update()
    {
        if (currentHealth <= 0) return;

        // --- PHASE CHECK ---
        if (currentHealth <= maxHealth * 0.5f && !isFlying)
        {
            StartCoroutine(TransitionToFlight());
            return;
        }

        if (currentState == State.Transitioning) return;

        // Cooldown Timer
        if (attackCooldown > 0) attackCooldown -= Time.deltaTime;

        // Note: We removed SelectTarget() from here because we now 
        // strictly control target switching in the Attack Logic.

        if (isFlying)
        {
            HandleFlyingBehavior();
        }
        else
        {
            HandleGroundBehavior();
        }
    }

    void HandleGroundBehavior()
    {
        if (isAttacking)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // Safety check: if target is null (game start), default to player
        if (currentTarget == null) currentTarget = player;

        float distance = Vector2.Distance(transform.position, currentTarget.position);
        bool didAttack = false;

        // If Cooldown is ready, check for attacks
        if (attackCooldown <= 0)
        {
            if (isOnRoof && currentTarget == houseCenter)
            {
                StartCoroutine(PerformAttack("RoofSmash", 1.0f));
                didAttack = true;
            }
            else if (distance <= meleeRange)
            {
                StartCoroutine(PerformAttack("Claw", 0.5f));
                didAttack = true;
            }
            else if (distance <= tongueRange)
            {
                StartCoroutine(PerformAttack("Tongue", 0.7f));
                didAttack = true;
            }
        }

        // If not attacking, chase the current target
        if (!didAttack)
        {
            MoveTowardsGround(currentTarget.position, moveSpeed);
        }
    }

    void HandleFlyingBehavior()
    {
        if (isAttacking) return;

        // Safety check
        if (currentTarget == null) currentTarget = player;

        float distance = Vector2.Distance(transform.position, currentTarget.position);

        // 1. ATTACK RUN: If cooldown is ready, fly DIRECTLY at target
        if (attackCooldown <= 0)
        {
            if (distance <= meleeRange)
            {
                StartCoroutine(PerformAttack("SwoopClaw", 0.5f));
            }
            else
            {
                // Fly straight to target to close distance
                Vector2 newPos = Vector2.MoveTowards(transform.position, currentTarget.position, flySpeed * Time.deltaTime);
                rb.MovePosition(newPos);
                FlipSprite(currentTarget.position);
            }
        }
        // 2. FLANKING: If cooling down, hover to the SIDE
        else
        {
            Vector2 targetPos = currentTarget.position;

            // Calculate "Flank" Position: Target + (Side Offset) + (Up Offset)
            // This puts the boss to the Left or Right of the player
            float xOffset = currentHoverSide * hoverSideOffset;

            // Add the bobbing effect
            float yBob = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;

            Vector2 hoverTarget = new Vector2(targetPos.x + xOffset, targetPos.y + yBob + 2.0f);

            // Move smoothly to the flank position
            Vector2 newPos = Vector2.MoveTowards(transform.position, hoverTarget, flySpeed * Time.deltaTime);
            rb.MovePosition(newPos);

            // Still look at the player even while flying sideways
            FlipSprite(currentTarget.position);
        }
    }

    void MoveTowardsGround(Vector2 targetPos, float speed)
    {
        if (rb.gravityScale == 0) rb.gravityScale = 1;
        Vector2 direction = (targetPos.x > transform.position.x) ? Vector2.right : Vector2.left;
        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
        FlipSprite(targetPos);
    }

    IEnumerator PerformAttack(string attackType, float duration)
    {
        isAttacking = true;
        attackCooldown = 2.0f; // Reset cooldown

        Debug.Log("Attack: " + attackType + " | Target: " + currentTarget.name);

        // Wait for animation
        yield return new WaitForSeconds(duration);

        isAttacking = false;

        // --- LOGIC: Increment Counter and maybe Switch Target ---
        attackCounter++;

        // Pick a new side to hover for variety (Left or Right)
        PickRandomSide();

        if (attackCounter >= attacksToSwitchTarget)
        {
            SwitchTarget();
            attackCounter = 0;
        }
    }

    void SwitchTarget()
    {
        // Toggle between Player and House
        if (currentTarget == player)
        {
            currentTarget = houseCenter;
            Debug.Log("Boss switching target to: HOUSE");
        }
        else
        {
            currentTarget = player;
            Debug.Log("Boss switching target to: PLAYER");
        }
    }

    void PickRandomSide()
    {
        // Randomly choose -1 (Left) or 1 (Right)
        currentHoverSide = (Random.Range(0, 2) == 0) ? -1f : 1f;
    }

    IEnumerator TransitionToFlight()
    {
        currentState = State.Transitioning;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        Debug.Log("Phase 2 START");

        yield return new WaitForSeconds(1.5f);

        isFlying = true;
        currentState = State.Chasing;
    }

    void FlipSprite(Vector3 target)
    {
        if (target.x > transform.position.x) transform.localScale = new Vector3(1, 1, 1);
        else transform.localScale = new Vector3(-1, 1, 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Roof")) isOnRoof = true;
        else if (collision.gameObject.CompareTag("Ground")) isOnRoof = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            currentHealth -= 5;
        }
    }
}