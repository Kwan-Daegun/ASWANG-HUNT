using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Header("Loot Settings")]
    public int minAmmo = 2; // Minimum bullets to get
    public int maxAmmo = 5; // Maximum bullets to get
    public float rotationSpeed = 50f;

    private int ammoAmount;

    void Start()
    {
        // Randomize the amount when this object is created
        ammoAmount = Random.Range(minAmmo, maxAmmo + 1);
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerShooting playerShooting = other.GetComponent<PlayerShooting>();

            if (playerShooting != null)
            {
                // Check if player is already full
                if (playerShooting.GetCurrentAmmo() >= playerShooting.GetMaxAmmo())
                {
                    Debug.Log("Ammo Full! Cannot pick up.");
                    return; // Don't pick it up, leave it on ground for later
                }

                // Add the random amount
                playerShooting.AddAmmo(ammoAmount);
                Debug.Log("Picked up " + ammoAmount + " ammo!");

                Destroy(gameObject);
            }
        }
    }
}
