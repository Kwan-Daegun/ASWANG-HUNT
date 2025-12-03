using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 2f;
    private Vector2 target;
    private Rigidbody2D rb;
    private HP house;
    public int enemyDmg;

    [Header("Item Drop Settings")]
    public GameObject coinPrefab;
    public GameObject ammoPrefab;
    [Range(0f, 100f)] public float coinDropChance = 100f;
    [Range(0f, 100f)] public float ammoDropChance = 100f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        house = GameObject.Find("House").GetComponent<HP>();
    }

    public void SetTarget(Vector2 targetPos)
    {
        target = targetPos;
    }

    void Update()
    {
        Vector2 direction = ((Vector2)transform.position - target).x > 0 ? Vector2.left : Vector2.right;
        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("House"))
        {
            Debug.Log("Enemy reached the house!");
            Destroy(gameObject);
        }
    }

    public void Die()
    {
        Debug.Log("Enemy died — dropping items...");
        TryDropItems();
        Destroy(gameObject);
    }

    void TryDropItems()
    {
        bool dropped = false;

        if (coinPrefab != null && Random.Range(0f, 100f) < coinDropChance)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
            Debug.Log("Dropped coin!");
            dropped = true;
        }

        if (ammoPrefab != null && Random.Range(0f, 100f) < ammoDropChance)
        {
            Instantiate(ammoPrefab, transform.position, Quaternion.identity);
            Debug.Log("Dropped ammo!");
            dropped = true;
        }

        if (!dropped)
        {
            Debug.Log("No items dropped this time.");
        }
    }
}
