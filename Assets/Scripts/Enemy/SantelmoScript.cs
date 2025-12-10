using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SantelmoScript : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float waveHeight = 2f;
    [SerializeField] private float waveFrequency = 5f;
    [SerializeField] private int burnDamagePerTick = 5;
    [SerializeField] private float burnTickSpeed = 1.5f;

    [Header("Item Drop Settings")]
    public GameObject coinPrefab;
    [Range(0f, 100f)] public float coinDropChance = 100f; // Reward for using Holy Water
    // Santelmos don't carry bullets, so no ammo drop

    private Rigidbody2D rb;
    private Vector2 target;
    private float timeAlive = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetTarget(Vector2 targetPos)
    {
        target = targetPos;
    }

    void Update()
    {
        timeAlive += Time.deltaTime;
        float xDir = (target.x > transform.position.x) ? 1 : -1;
        rb.velocity = new Vector2(xDir * speed, Mathf.Sin(timeAlive * waveFrequency) * waveHeight);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Touched House/Player: CURSE THEM (No loot, punishment)
        if (other.CompareTag("House") || other.CompareTag("Player"))
        {
            HP targetHP = other.GetComponent<HP>();
            if (targetHP != null) targetHP.ApplyBurn(burnDamagePerTick, burnTickSpeed);
            Destroy(gameObject);
        }

        // 2. Touched Holy Water: REWARD (Loot)
        if (other.CompareTag("HolyWater"))
        {
            ExtinguishSelf();
        }
    }

    public void ExtinguishSelf()
    {
        // Drop items ONLY if killed by player
        if (coinPrefab != null && Random.Range(0f, 100f) < coinDropChance)
            Instantiate(coinPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
