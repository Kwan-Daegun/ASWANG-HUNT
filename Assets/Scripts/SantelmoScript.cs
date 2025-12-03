using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SantelmoScript : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float waveHeight = 2f; // How high it goes up/down
    [SerializeField] private float waveFrequency = 5f; // How fast it bobs up/down

    [Header("Attack Settings")]
    [SerializeField] private float damageInterval = 1.5f; // Time between attacks
    [SerializeField] private int damageAmount = 10;

    [Header("Holy Water Settings")]
    [SerializeField] private string holyWaterTag = "HolyWater"; // Tag for your water explosion

    private Rigidbody2D rb;
    private Vector2 target;
    private bool isAttacking = false;
    private float timeAlive = 0f;
    private HP houseHP; // Reference to your existing HP script

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Automatically find the House's HP script so we can damage it
        GameObject houseObj = GameObject.Find("House");
        if (houseObj != null)
        {
            houseHP = houseObj.GetComponent<HP>();
        }
    }

    // Called by your Spawner
    public void SetTarget(Vector2 targetPos)
    {
        target = targetPos;
    }

    void Update()
    {
        // If we are attacking the house, stop moving
        if (isAttacking) return;

        timeAlive += Time.deltaTime;

        // 1. Calculate Horizontal Direction (Left or Right)
        float xDir = (target.x > transform.position.x) ? 1 : -1;
        float xVelocity = xDir * speed;

        // 2. Calculate Wave Movement (Up and Down)
        // We set Y velocity based on a Sine wave calculation
        float yVelocity = Mathf.Sin(timeAlive * waveFrequency) * waveHeight;

        // Apply movement
        rb.velocity = new Vector2(xVelocity, yVelocity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Reached the House
        if (other.CompareTag("House"))
        {
            StartCoroutine(AttackHouseRoutine());
        }

        // 2. Touched Holy Water / Water Bottle Explosion
        if (other.CompareTag(holyWaterTag))
        {
            Extinguish();
        }
    }

    // Logic to deal damage every X seconds
    IEnumerator AttackHouseRoutine()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero; // Stop moving completely
        Debug.Log("Fireball reached house! Starting burn damage...");

        // Keep damaging as long as this enemy and the house exist
        while (this != null && houseHP != null)
        {
            houseHP.SubHealth(damageAmount);
            yield return new WaitForSeconds(damageInterval);
        }
    }

    // Call this when touched by water
    public void Extinguish()
    {
        Debug.Log("Fireball extinguished by Holy Water!");
        // Optional: Spawn steam particle effect here
        Destroy(gameObject);
    }
}
