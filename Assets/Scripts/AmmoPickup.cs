using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 5;        // How much ammo this pickup gives
    public float rotationSpeed = 50f; // Optional spin for visibility

    void Update()
    {
        // Optional: make the ammo spin for visibility
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerShooting playerShooting = other.GetComponent<PlayerShooting>();

            if (playerShooting != null)
            {
                // Refill ammo
                playerShooting.AddAmmo(ammoAmount);
                Debug.Log("Picked up ammo +" + ammoAmount);

                //  Update the UI immediately
                UIManager.Instance.UpdateAmmo(playerShooting.GetCurrentAmmo(), playerShooting.GetMaxAmmo());

                Destroy(gameObject);
            }
        }
    }
}
