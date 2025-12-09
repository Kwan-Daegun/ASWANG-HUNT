using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SantelmoScript : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float waveHeight = 2f;
    [SerializeField] private float waveFrequency = 5f;

    [Header("Burn Settings")]
    [SerializeField] private int burnDamagePerTick = 5;
    [SerializeField] private float burnTickSpeed = 1.5f; // User requested 1.5s interval

    [Header("Holy Water Settings")]
    [SerializeField] private string holyWaterTag = "HolyWater";

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

        // Standard Santelmo Movement
        float xDir = (target.x > transform.position.x) ? 1 : -1;
        float xVelocity = xDir * speed;
        float yVelocity = Mathf.Sin(timeAlive * waveFrequency) * waveHeight;

        rb.velocity = new Vector2(xVelocity, yVelocity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Touched House OR Player -> Set them on FIRE
        if (other.CompareTag("House") || other.CompareTag("Player"))
        {
            HP targetHP = other.GetComponent<HP>();
            if (targetHP != null)
            {
                // We pass the damage and the speed, but NOT a duration.
                // The fire is now infinite until cured.
                targetHP.ApplyBurn(burnDamagePerTick, burnTickSpeed);
            }

            // DESTROY SANTELMO IMMEDIATELY
            // It has passed its "curse" to the target, so it is no longer needed.
            Destroy(gameObject);
        }

        // 2. Touched Holy Water -> Extinguish SELF (The Santelmo dies before hitting player)
        if (other.CompareTag(holyWaterTag))
        {
            ExtinguishSelf();
        }
    }

    public void ExtinguishSelf()
    {
        Debug.Log("Santelmo extinguished by water before it could hit anything!");
        // Spawn steam particles?
        Destroy(gameObject);
    }
}
