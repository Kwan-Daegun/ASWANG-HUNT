using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    public int coinValue = 1;         // How much the coin is worth
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
    }
}
