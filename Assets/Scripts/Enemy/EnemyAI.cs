using UnityEngine;
using System.Collections; // Required for IEnumerators

public class EnemyAI : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 2f;
    public int enemyDmg = 10;
    public float attackCooldown = 1.5f;

    [Header("Item Drop Settings")]
    public GameObject coinPrefab;
    public GameObject ammoPrefab;
    [Range(0f, 100f)] public float coinDropChance = 50f;
    [Range(0f, 100f)] public float ammoDropChance = 10f;

    [Header("Slash Visuals")] // --- NEW VISUAL SETTINGS ---
    public Sprite[] slashSprites; // Drag your scratch/bite sprites here
    public float frameSpeed = 0.04f;
    public Vector3 slashOffset = new Vector3(0.5f, 0f, 0f); // Adjusts where the slash appears
    public int sortOrder = 10; // Higher number ensures it draws on top of the enemy

    private Vector2 target;
    private Rigidbody2D rb;
    private float attackTimer = 0f;
    private SpriteRenderer slashRenderer; // The hidden renderer we create

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        attackTimer = 0f;

        // --- NEW: CREATE THE SLASH OBJECT AUTOMATICALLY ---
        // This creates a child object on the Enemy to hold the slash sprite
        GameObject slashObj = new GameObject("EnemySlashEffect");
        slashObj.transform.parent = transform;
        slashObj.transform.localPosition = slashOffset;
        slashObj.transform.localScale = Vector3.one;

        slashRenderer = slashObj.AddComponent<SpriteRenderer>();
        slashRenderer.sortingOrder = sortOrder;
        slashRenderer.enabled = false; // Hide it initially
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
        // Simple logic to face left or right
        Vector2 direction = ((Vector2)transform.position - target).x > 0 ? Vector2.left : Vector2.right;

        // This scale flip AUTOMATICALLY flips the child Slash Object too!
        transform.localScale = new Vector3(direction.x > 0 ? -1 : 1, 1, 1);

        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
    }

    // --- SHARED ATTACK LOGIC ---
    private void AttemptAttack(GameObject hitObject)
    {
        if (hitObject.CompareTag("House") || hitObject.CompareTag("Player"))
        {
            rb.velocity = Vector2.zero;

            if (attackTimer <= 0)
            {
                HP hpScript = hitObject.GetComponent<HP>();
                if (hpScript == null) hpScript = hitObject.GetComponentInParent<HP>();

                if (hpScript != null)
                {
                    // 1. PLAY THE VISUAL SLASH
                    StartCoroutine(PlaySlashEffect());

                    // 2. DEAL DAMAGE
                    hpScript.SubHealth(enemyDmg);
                    Debug.Log("Tiyanak attacked " + hitObject.name);

                    attackTimer = attackCooldown;
                }
            }
        }
    }

    // --- NEW: COROUTINE FOR ANIMATION ---
    private IEnumerator PlaySlashEffect()
    {
        // Safety check: Do we have sprites?
        if (slashRenderer != null && slashSprites.Length > 0)
        {
            slashRenderer.enabled = true; // Show it

            // Loop through the animation frames
            for (int i = 0; i < slashSprites.Length; i++)
            {
                slashRenderer.sprite = slashSprites[i];
                yield return new WaitForSeconds(frameSpeed);
            }

            slashRenderer.enabled = false; // Hide it again
        }
    }

    // --- LISTENERS ---
    private void OnCollisionStay2D(Collision2D collision)
    {
        AttemptAttack(collision.gameObject);
    }

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

    // --- NEW: GIZMOS TO HELP YOU POSITION THE SLASH ---
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        // Calculate where the slash will appear
        Vector3 visualPos = transform.position + slashOffset;

        // Flip gizmo calculation if enemy is facing left (approximate)
        if (transform.localScale.x < 0)
            visualPos = transform.position + new Vector3(-slashOffset.x, slashOffset.y, slashOffset.z);

        Gizmos.DrawWireCube(visualPos, new Vector3(0.5f, 0.5f, 0.1f));
        Gizmos.DrawLine(transform.position, visualPos);
    }
}





/*using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 2f;
    public int enemyDmg = 10;
    public float attackCooldown = 1.5f;

    [Header("Item Drop Settings")]
    public GameObject coinPrefab;
    public GameObject ammoPrefab;
    [Range(0f, 100f)] public float coinDropChance = 50f;
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
*/