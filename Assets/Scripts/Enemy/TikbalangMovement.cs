using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TikbalangMovement : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private int enemyDmg;

    private Vector2 target;
    private Rigidbody2D rb;
    //private HP house;
    

    [Header("Item Drop Settings")]
    public GameObject coinPrefab;
    public GameObject ammoPrefab;
    [Range(0f, 100f)] public float coinDropChance = 100f;
    [Range(0f, 100f)] public float ammoDropChance = 100f;

    //NEW
    public float jumpForce = 7f;
    public float cycleDuration = 2f;

    private enum EnemyState { Walking, Jumping }
    private EnemyState currentState = EnemyState.Walking;
    private float stateTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //house = GameObject.Find("House").GetComponent<HP>();

        //NEW
        stateTimer = cycleDuration;
    }

    public void SetTarget(Vector2 targetPos)
    {
        target = targetPos;
    }

    void Update()
    {
        Vector2 direction = ((Vector2)transform.position - target).x > 0 ? Vector2.left : Vector2.right;
        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);

        //NEW
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0)
        {
            stateTimer = cycleDuration;

            if (currentState == EnemyState.Walking)
            {
                currentState = EnemyState.Jumping;
            }
            else
            {
                currentState = EnemyState.Walking;
            }
        }
        if (currentState == EnemyState.Walking)
        {
            HandleWalking();
        }
        else if (currentState == EnemyState.Jumping)
        {
            HandleJumping();
        }

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

    void HandleWalking()
    {
        Vector2 direction = ((Vector2)transform.position - target).x > 0 ? Vector2.left : Vector2.right;

        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
    }

    //NEW
    void HandleJumping()
    {
        rb.velocity = new Vector2(0f, jumpForce);

        currentState = EnemyState.Walking;
    }
}
