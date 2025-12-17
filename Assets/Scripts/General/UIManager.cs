using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("TextMeshPro References")]
    public TMP_Text ammoText;
    public TMP_Text coinText;

    // 1. Drag your Day/Night Text object here in the Inspector
    public TMP_Text dayNightText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // 2. THIS RUNS AUTOMATICALLY WHEN SCENE LOADS
    private void Start()
    {
        UpdateDayNightLabel();
    }

    private void UpdateDayNightLabel()
    {
        // Safety check: make sure Data exists
        if (DayandNightData.Instance == null)
        {
            return;
        }

        int day = DayandNightData.Instance.currentDay;
        string currentScene = SceneManager.GetActiveScene().name;
        string shopSceneName = DayandNightData.Instance.daySceneName;

        // Check if we are in the Shop Scene
        if (currentScene == shopSceneName)
        {
            if (dayNightText != null) dayNightText.text = "Day " + day;
        }
        // Otherwise, we are in a Night Scene
        else
        {
            if (dayNightText != null) dayNightText.text = "Night " + day;
        }
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



    /*public static UIManager Instance; // Singleton access

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
    }*/
}
