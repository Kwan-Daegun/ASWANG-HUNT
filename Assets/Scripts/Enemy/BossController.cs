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
    public int attacksToSwitchTarget = 3;

    [Header("Flight Settings")]
    public float hoverAmplitude = 0.5f;
    public float hoverFrequency = 2f;
    public float hoverSideOffset = 4f;

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

    public Transform currentTarget;
    private int attackCounter = 0;
    private float currentHoverSide = 1f;

    private enum State { Idle, Chasing, Attacking, Transitioning }
    private State currentState = State.Idle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        currentTarget = player;
        PickRandomSide();
    }

    void Update()
    {
        if (currentHealth <= 0) return;

        if (currentHealth <= maxHealth * 0.5f && !isFlying)
        {
            StartCoroutine(TransitionToFlight());
            return;
        }

        if (currentState == State.Transitioning) return;

        if (attackCooldown > 0) attackCooldown -= Time.deltaTime;

        if (isFlying)
            HandleFlyingBehavior();
        else
            HandleGroundBehavior();
    }

    void HandleGroundBehavior()
    {
        if (isAttacking)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (currentTarget == null) currentTarget = player;

        float distance = Vector2.Distance(transform.position, currentTarget.position);
        bool didAttack = false;

        if (attackCooldown <= 0)
        {
            if (isOnRoof && currentTarget == houseCenter)
            {
                if (Vector2.Distance(transform.position, houseCenter.position) <= meleeRange + 0.5f) // strict range
                {
                    StartCoroutine(PerformAttack("RoofSmash", 1.0f));
                    didAttack = true;
                }
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

        if (!didAttack)
        {
            MoveTowardsGround(currentTarget.position, moveSpeed);
        }
    }

    void HandleFlyingBehavior()
    {
        if (isAttacking) return;

        if (currentTarget == null) currentTarget = player;

        float distance = Vector2.Distance(transform.position, currentTarget.position);

        if (attackCooldown <= 0)
        {
            if (distance <= meleeRange)
            {
                StartCoroutine(PerformAttack("SwoopClaw", 0.5f));
            }
            else
            {
                Vector2 newPos = Vector2.MoveTowards(transform.position, currentTarget.position, flySpeed * Time.deltaTime);
                rb.MovePosition(newPos);
                FlipSprite(currentTarget.position);
            }
        }
        else
        {
            float xOffset = currentHoverSide * hoverSideOffset;
            float yBob = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
            Vector2 hoverTarget = new Vector2(currentTarget.position.x + xOffset, currentTarget.position.y + yBob + 2.0f);

            Vector2 newPos = Vector2.MoveTowards(transform.position, hoverTarget, flySpeed * Time.deltaTime);
            rb.MovePosition(newPos);
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
        attackCooldown = 2.0f;

        yield return new WaitForSeconds(duration * 0.5f); // wait until hit frame

        // --- APPLY DAMAGE STRICTLY WITHIN RANGE ---
        if (currentTarget != null)
        {
            float distance = Vector2.Distance(transform.position, currentTarget.position);
            HP targetHP = currentTarget.GetComponent<HP>();

            if (targetHP != null)
            {
                int damage = 0;

                switch (attackType)
                {
                    case "Claw":
                        if (distance <= meleeRange) damage = 10;
                        break;
                    case "Tongue":
                        if (distance <= tongueRange) damage = 7;
                        break;
                    case "SwoopClaw":
                        if (distance <= meleeRange) damage = 10;
                        break;
                    case "RoofSmash":
                        if (currentTarget == houseCenter && distance <= meleeRange) damage = 15;
                        break;
                }

                if (damage > 0)
                {
                    targetHP.SubHealth(damage);
                    Debug.Log("Boss dealt " + damage + " damage to " + currentTarget.name);
                }
            }
        }

        yield return new WaitForSeconds(duration * 0.5f);
        isAttacking = false;
        attackCounter++;
        PickRandomSide();

        if (attackCounter >= attacksToSwitchTarget)
        {
            SwitchTarget();
            attackCounter = 0;
        }
    }

    void SwitchTarget()
    {
        currentTarget = (currentTarget == player) ? houseCenter : player;
        Debug.Log("Boss switching target to: " + currentTarget.name);
    }

    void PickRandomSide()
    {
        currentHoverSide = (Random.Range(0, 2) == 0) ? -1f : 1f;
    }

    IEnumerator TransitionToFlight()
    {
        currentState = State.Transitioning;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(1.5f);

        isFlying = true;
        currentState = State.Chasing;
    }

    void FlipSprite(Vector3 target)
    {
        transform.localScale = (target.x > transform.position.x) ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
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
