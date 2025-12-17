using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [Header("Settings")]
    public int coinValue = 5; // Set this to 5 in Inspector
    public float rotationSpeed = 50f;

    [Header("Performance")]
    [SerializeField] private float lifetime = 15f; // Disappear after 15 seconds

    void Start()
    {
        // Destroy self automatically after X seconds
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Try to find the PlayerCoins script
            PlayerCoins playerCoins = other.GetComponent<PlayerCoins>();

            if (playerCoins != null)
            {
                playerCoins.AddCoins(coinValue);
                Debug.Log("Picked up " + coinValue + " coins!");

                // Optional: Play a "Ching!" sound here

                Destroy(gameObject);
            }
        }
    }



    /*public int coinValue = 3;         // How much the coin is worth
    public float rotationSpeed = 100f; // Optional spin for visibility

    void Update()
    {
        // Optional spin (for 2D, use Vector3.forward instead of Vector3.up)
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCoins playerCoins = other.GetComponent<PlayerCoins>();

            if (playerCoins != null)
            {
                playerCoins.AddCoins(coinValue);
                Debug.Log("Picked up coin +" + coinValue);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("No PlayerCoins script found on Player!");
            }
        }
    }*/
}
