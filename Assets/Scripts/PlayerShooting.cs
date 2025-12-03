using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

    [Header("Ammo Settings")]
    public int maxAmmo = 10;         // Maximum ammo the player can hold
    private int currentAmmo;         // Current ammo count

    void Start()
    {
        currentAmmo = maxAmmo;       // Fill ammo at start
        UIManager.Instance.UpdateAmmo(currentAmmo, maxAmmo);

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        // Optional: reload manually (press R)
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo! Press R to reload.");
            return;
        }

        currentAmmo--;  // Reduce ammo when shooting
        UIManager.Instance.UpdateAmmo(currentAmmo, maxAmmo); // Keep or remove based on your UI setup

        // 1. Get the mouse position in World space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure z is 0 for 2D consistency

        // 2. Calculate the direction vector from the fire point to the mouse
        Vector2 shootDirection = (mousePosition - firePoint.position).normalized;

        // 3. Calculate the rotation angle (optional, but good for visual direction)
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

        // 4. Instantiate the bullet, applying the rotation
        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.Euler(0, 0, angle) // Rotate the bullet to face the direction
        );

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // 5. Apply velocity in the calculated direction
            rb.velocity = shootDirection * bulletSpeed;

            // You can remove your original scaling logic as the rotation handles the visual direction:
            // Vector3 bulletScale = bullet.transform.localScale;
            // bulletScale.x *= direction;
            // bullet.transform.localScale = bulletScale;
        }
    }

    /*void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        if (currentAmmo <= 0)
        {
            Debug.Log("Out of ammo! Press R to reload.");
            return;
        }

        currentAmmo--;   // Reduce ammo when shooting
        UIManager.Instance.UpdateAmmo(currentAmmo, maxAmmo);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float direction = transform.localScale.x > 0 ? 1f : -1f;
            rb.velocity = new Vector2(direction * bulletSpeed, 0f);
            Vector3 bulletScale = bullet.transform.localScale;
            bulletScale.x *= direction;
            bullet.transform.localScale = bulletScale;
        }
    }*/

    void Reload()
    {
        currentAmmo = maxAmmo;
        UIManager.Instance.UpdateAmmo(currentAmmo, maxAmmo);
        Debug.Log("Reloaded!");
    }

    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo);
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public int GetMaxAmmo()
    {
        return maxAmmo;
    }
}
