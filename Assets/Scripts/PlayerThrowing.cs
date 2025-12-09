using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowing : MonoBehaviour
{
    [Header("Throwing Settings")]
    [SerializeField] private GameObject holyWaterPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float throwForce = 15f;

    [Header("Ammo & Cooldown")]
    // Serialized so you can adjust it in Inspector for testing (initially 0)
    [SerializeField] public int holyWaterAmmo = 0;
    [SerializeField] private float throwCooldown = 5f; // 5 seconds wait time

    private float nextThrowTime = 0f; // Tracks when we are allowed to throw again
    private Collider2D playerCollider;

    void Start()
    {
        playerCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            AttemptThrow();
        }
    }

    void AttemptThrow()
    {
        // 1. Check Ammo
        if (holyWaterAmmo <= 0)
        {
            Debug.Log("Out of Holy Water! Visit the Shop.");
            return;
        }

        // 2. Check Cooldown
        if (Time.time < nextThrowTime)
        {
            float timeRemaining = nextThrowTime - Time.time;
            Debug.Log("Holy Water on Cooldown: " + timeRemaining.ToString("F1") + "s");
            return;
        }

        // 3. Throw is Valid
        ThrowAtMouse();

        // Deduct Ammo and Reset Timer
        holyWaterAmmo--;
        nextThrowTime = Time.time + throwCooldown;
    }

    void ThrowAtMouse()
    {
        if (holyWaterPrefab == null || throwPoint == null) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector2 direction = (mousePos - throwPoint.position).normalized;

        GameObject bottle = Instantiate(holyWaterPrefab, throwPoint.position, Quaternion.identity);

        // Ignore Player Collision
        Collider2D bottleCollider = bottle.GetComponent<Collider2D>();
        if (playerCollider != null && bottleCollider != null)
        {
            Physics2D.IgnoreCollision(playerCollider, bottleCollider, true);
        }

        // Apply Force
        Rigidbody2D rb = bottle.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(direction * throwForce, ForceMode2D.Impulse);
            rb.AddTorque(-5f, ForceMode2D.Impulse);
        }
    }

    // Helper function for the Shop later
    public void AddAmmo(int amount)
    {
        holyWaterAmmo += amount;
    }
}
