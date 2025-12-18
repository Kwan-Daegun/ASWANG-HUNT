using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NightManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel; // Drag your "Game Over Canvas" or Panel here

    [Header("Player References")]
    [SerializeField] private PlayerCoins playerCoins;
    [SerializeField] private PlayerShooting playerShooting;
    [SerializeField] private PlayerThrowing playerThrowing;
    [SerializeField] private HP playerHP;
    [SerializeField] private HP houseHP;

    void Start()
    {
        // 1. Check if we should load data (Day 2, Day 3, etc.)
        // If it's Day 1, we let the default values stay (Full HP, 0 Coins, Default Ammo)
        if (DayandNightData.Instance != null && DayandNightData.Instance.currentDay > 1)
        {
            LoadSavedStats();
        }

        // 2. Update the UI immediately so the numbers are correct on screen
        if (UIManager.Instance != null)
        {
            // Update Coin UI
            if (playerCoins != null)
                UIManager.Instance.UpdateCoins(playerCoins.coins);

            // Update Ammo UI
            if (playerShooting != null)
                UIManager.Instance.UpdateAmmo(playerShooting.GetCurrentAmmo(), playerShooting.GetMaxAmmo());
        }
    }

    void LoadSavedStats()
    {
        Debug.Log("Loading Saved Stats from GlobalData...");

        // --- HEALTH ---
        if (GlobalData.PlayerHealth > 0) playerHP.SetHealth(GlobalData.PlayerHealth);
        if (GlobalData.HouseHealth > 0) houseHP.SetHealth(GlobalData.HouseHealth);

        // --- COINS ---
        // Directly set the coins variable
        if (playerCoins != null)
        {
            playerCoins.coins = GlobalData.Coins;
        }

        // --- AMMO ---
        if (playerShooting != null)
        {
            playerShooting.SetAmmo(GlobalData.Ammo);
        }

        // --- HOLY WATER ---
        if (playerThrowing != null)
        {
            playerThrowing.holyWaterAmmo = GlobalData.HolyWaterAmmo;
        }
    }

    /*private void Start()
    {
        // Ensure the Game Over screen is hidden when the night starts
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        Time.timeScale = 1f; // Ensure game is running
    }*/

    // --- CALLED BY HP SCRIPT ---
    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f; // PAUSE the game so enemies stop attacking
        }
    }

    // --- BUTTON FUNCTIONS ---

    public void RetryGame()
    {
        // 1. Unpause the game immediately
        Time.timeScale = 1f;

        // 2. Reset stats
        GlobalData.ResetData();

        // 3. Try to use the Main System
        if (DayandNightData.Instance != null)
        {
            DayandNightData.Instance.ResetGame();
        }
        else
        {
            // FALLBACK: If we are testing Night 1 directly, DayandNightData won't exist.
            // So we manually load the shop scene.
            Debug.Log("DayandNightData was missing (Testing mode?), forcing load of Shop.");

            // IMPORTANT: Make sure this string "Shop" matches your actual scene name exactly!
            SceneManager.LoadScene("Shop");
        }
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f; // Unpause time
        SceneManager.LoadScene("NewMenu"); // Make sure this matches your Menu scene name exactly
    }

    // --- EXISTING WIN LOGIC ---
    public void GoToStore()
    {
        SaveStats();
        DayandNightData.Instance.CompleteNight();
    }

    public void GoToNextLevel()
    {
        SaveStats();
        DayandNightData.Instance.SkipToNextNight();
    }

    private void SaveStats()
    {
        if (playerCoins != null) GlobalData.Coins = playerCoins.coins;
        if (playerShooting != null) GlobalData.Ammo = playerShooting.GetCurrentAmmo();
        if (playerThrowing != null) GlobalData.HolyWaterAmmo = playerThrowing.holyWaterAmmo;
        if (playerHP != null) GlobalData.PlayerHealth = playerHP.currentBarValue;
        if (houseHP != null) GlobalData.HouseHealth = houseHP.currentBarValue;
    }
}
