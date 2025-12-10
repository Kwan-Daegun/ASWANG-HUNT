using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 2f;
    public int enemyDmg = 10;
    public float attackCooldown = 1.5f;

    [Header("Item Drop Settings")]
    public GameObject coinPrefab;
    public GameObject ammoPrefab;
    [Range(0f, 100f)] public float coinDropChance = 30f;
    [Range(0f, 100f)] public float ammoDropChance = 10f;

    private Vector2 target;
    private Rigidbody2D rb;
    private float attackTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        attackTimer = 0f;
    }

    public void SetTarget(Vector2 targetPos)
    {
        target = targetPos;
    }

    void Update()
    {
        if (attackTimer > 0) attackTimer -= Time.deltaTime;

        MoveTowardsTarget();
    }

    void MoveTowardsTarget()
    {
        Vector2 direction = ((Vector2)transform.position - target).x > 0 ? Vector2.left : Vector2.right;
        transform.localScale = new Vector3(direction.x > 0 ? -1 : 1, 1, 1);
        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
    }

    // --- SHARED ATTACK LOGIC ---
    // We call this function regardless of how we hit the object (Trigger or Collision)
    private void AttemptAttack(GameObject hitObject)
    {
        // Check Tags
        if (hitObject.CompareTag("House") || hitObject.CompareTag("Player"))
        {
            // Stop moving if we hit the target
            rb.velocity = Vector2.zero;

            // Check Cooldown
            if (attackTimer <= 0)
            {
                // Find HP Script (Check object first, then parent)
                HP hpScript = hitObject.GetComponent<HP>();
                if (hpScript == null) hpScript = hitObject.GetComponentInParent<HP>();

                if (hpScript != null)
                {
                    hpScript.SubHealth(enemyDmg);
                    Debug.Log("Tiyanak attacked " + hitObject.name);
                    attackTimer = attackCooldown; // Reset timer
                }
            }
        }
    }

    // --- LISTENER 1: PHYSICAL COLLISIONS (For Enemies bumping into each other or walls) ---
    private void OnCollisionStay2D(Collision2D collision)
    {
        AttemptAttack(collision.gameObject);
    }

    // --- LISTENER 2: TRIGGER COLLISIONS (For your House set to 'Is Trigger') ---
    private void OnTriggerStay2D(Collider2D other)
    {
        AttemptAttack(other.gameObject);
    }

    // --- DEATH & DROPS ---
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
