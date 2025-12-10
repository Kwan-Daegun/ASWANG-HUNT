using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // Singleton access

    [Header("TextMeshPro References")]
    public TMP_Text ammoText;
    public TMP_Text coinText;

    private void Awake()
    {
        // Simple singleton pattern (optional)
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        if (ammoText != null)
            ammoText.text = $"Ammo: {currentAmmo}/{maxAmmo}";
    }

    public void UpdateCoins(int coins)
    {
        if (coinText != null)
            coinText.text = $"Coins: {coins}";
    }
}
