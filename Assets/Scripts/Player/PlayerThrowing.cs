using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowing : MonoBehaviour
{
    [Header("Throwing Settings")]
    [SerializeField] private GameObject holyWaterPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float throwForce = 15f;

    [Header("Ammo")]
    public int holyWaterAmmo = 0;
    [SerializeField] private float throwCooldown = 2f;

    private float nextThrowTime = 0f;
    private Collider2D playerCollider;

    void Start()
    {
        playerCollider = GetComponent<Collider2D>();
        // Load from Global Data
        holyWaterAmmo = GlobalData.HolyWaterAmmo;
    }

    // Save to Global Data when scene changes
    private void OnDisable()
    {
        GlobalData.HolyWaterAmmo = holyWaterAmmo;
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
        if (holyWaterAmmo <= 0) return;

        if (Time.time < nextThrowTime) return;

        ThrowAtMouse();

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

        Collider2D bottleCollider = bottle.GetComponent<Collider2D>();
        if (playerCollider != null && bottleCollider != null)
        {
            Physics2D.IgnoreCollision(playerCollider, bottleCollider, true);
        }

        Rigidbody2D rb = bottle.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(direction * throwForce, ForceMode2D.Impulse);
            rb.AddTorque(-5f, ForceMode2D.Impulse);
        }
    }

    public void AddAmmo(int amount)
    {
        holyWaterAmmo += amount;
    }
}
