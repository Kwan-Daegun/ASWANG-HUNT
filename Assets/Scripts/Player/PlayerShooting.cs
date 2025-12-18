using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

    public int maxAmmo = 30;
    private int currentAmmo;

    private PlayerAnimation playerAnimation;

    // ▶ Audio
    public AudioSource shootingAudioSource; // Assign in Inspector
    public AudioClip shootClip;             // Assign shooting sound in Inspector

    void Start()
    {
        // Load ammo from GlobalData
        currentAmmo = GlobalData.Ammo;

        // Update the UI
        UIManager.Instance.UpdateAmmo(currentAmmo, maxAmmo);

        // Get PlayerAnimation component
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    void Update()
    {
        if (Time.timeScale == 0f)
            return;

        if (Input.GetMouseButtonDown(0))
            Shoot();

        if (Input.GetKeyDown(KeyCode.R))
            Reload();
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
            return;

        if (currentAmmo <= 0)
            return;

        currentAmmo--;
        UIManager.Instance.UpdateAmmo(currentAmmo, maxAmmo);

        // ▶ PLAY SHOOT ANIMATION
        if (playerAnimation != null)
            playerAnimation.PlayShootAnimation();

        // ▶ PLAY SHOOT SOUND
        if (shootingAudioSource != null && shootClip != null)
            shootingAudioSource.PlayOneShot(shootClip);

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector2 shootDirection = (mousePosition - firePoint.position).normalized;
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

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
    }

    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Min(currentAmmo + amount, maxAmmo);
        UIManager.Instance.UpdateAmmo(currentAmmo, maxAmmo);
    }

    public void SetAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(amount, 0, maxAmmo);
        UIManager.Instance.UpdateAmmo(currentAmmo, maxAmmo);
    }

    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;

    private void OnDisable()
    {
        GlobalData.Ammo = currentAmmo;
    }
}
