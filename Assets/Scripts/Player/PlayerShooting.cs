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
        // Load ammo from GlobalData if available
        currentAmmo = Mathf.Clamp(GlobalData.Ammo, 0, maxAmmo);
        UIManager.Instance.UpdateAmmo(currentAmmo, maxAmmo);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Shoot();

        // Optional: reload manually (press R)
        if (Input.GetKeyDown(KeyCode.R))
            Reload();
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
        UIManager.Instance.UpdateAmmo(currentAmmo, maxAmmo);

        // 1. Get the mouse position in World space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        // 2. Calculate the direction vector from the fire point to the mouse
        Vector2 shootDirection = (mousePosition - firePoint.position).normalized;

        // 3. Calculate rotation angle
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

        // 4. Instantiate the bullet with rotation
        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.Euler(0, 0, angle)
        );

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = shootDirection * bulletSpeed;
    }

    void Reload()
    {
        currentAmmo = maxAmmo;
        UIManager.Instance.UpdateAmmo(currentAmmo, maxAmmo);
        Debug.Log("Reloaded!");
    }

    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo);
        UIManager.Instance.UpdateAmmo(currentAmmo, maxAmmo);
    }

    // --- Persistence Methods ---
    public void SetAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(amount, 0, maxAmmo);
        UIManager.Instance.UpdateAmmo(currentAmmo, maxAmmo);
    }

    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;

    // Automatically save ammo to GlobalData whenever script is disabled (e.g., switching scenes)
    private void OnDisable()
    {
        GlobalData.Ammo = currentAmmo;
    }
}
