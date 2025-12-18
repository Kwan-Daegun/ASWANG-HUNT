using UnityEngine;
using UnityEngine.SceneManagement;

public class StoreManager : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private HP playerHP;
    [SerializeField] private HP houseHP;
    [SerializeField] private PlayerCoins playerCoinsScript;
    [SerializeField] private PlayerShooting playerShooting;
    [SerializeField] private PlayerThrowing playerThrowing;

    [Header("Shop Buttons")]
    [SerializeField] private GameObject ammoButton;
    [SerializeField] private GameObject healingButton;
    [SerializeField] private GameObject holyWaterButton;
    [SerializeField] private GameObject buffButton;

    [Header("Prices")]
    public int ammoPrice = 10;
    public int healingPrice = 10;
    public int holyWaterPrice = 10;
    public int buffPrice = 30;

    private void Start()
    {
        Time.timeScale = 1f;

        // --- 1. Load Persistent Stats ---
        // USE SetHealth so the UI bar updates immediately!
        if (GlobalData.PlayerHealth > 0)
            playerHP.SetHealth(GlobalData.PlayerHealth);

        if (GlobalData.HouseHealth > 0)
            houseHP.SetHealth(GlobalData.HouseHealth);

        if (playerCoinsScript != null)
            playerCoinsScript.coins = GlobalData.Coins;

        if (playerShooting != null)
            playerShooting.SetAmmo(GlobalData.Ammo);

        if (playerThrowing != null)
            playerThrowing.holyWaterAmmo = GlobalData.HolyWaterAmmo;

        // --- 2. Update UI ---
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateAmmo(playerShooting.GetCurrentAmmo(), playerShooting.GetMaxAmmo());
            UIManager.Instance.UpdateCoins(playerCoinsScript.coins);
        }

        // --- 3. Shop Locking Logic ---
        SetupShopForDay();
    }

    private void SetupShopForDay()
    {
        // (Keep your existing logic here, no changes needed)
        int currentDay = DayandNightData.Instance.currentDay;

        if (currentDay == 1)
        {
            if (ammoButton) ammoButton.SetActive(true);
            if (healingButton) healingButton.SetActive(false);
            if (holyWaterButton) holyWaterButton.SetActive(false);
            if (buffButton) buffButton.SetActive(false);
        }
        else if (currentDay == 2)
        {
            if (ammoButton) ammoButton.SetActive(true);
            if (healingButton) healingButton.SetActive(true);
            if (holyWaterButton) holyWaterButton.SetActive(true);
            if (buffButton) buffButton.SetActive(false);
        }
        else if (currentDay >= 3)
        {
            if (ammoButton) ammoButton.SetActive(true);
            if (healingButton) healingButton.SetActive(true);
            if (holyWaterButton) holyWaterButton.SetActive(true);
            if (buffButton) buffButton.SetActive(true);
        }
    }

    // --- BUYING FUNCTIONS ---

    public void BuyAmmo()
    {
        if (TrySpendCoins(ammoPrice))
        {
            playerShooting.AddAmmo(10);
            Debug.Log("Bought 10 Ammo");
            // Update UI immediately if possible
            if (UIManager.Instance != null)
                UIManager.Instance.UpdateAmmo(playerShooting.GetCurrentAmmo(), playerShooting.GetMaxAmmo());
        }
    }

    public void BuyHealing()
    {
        // Logic: Heal Player 25 AND House 15
        // Check if either one actually needs healing to avoid wasting money (Optional)
        bool playerNeedsHealth = playerHP.currentBarValue < 100;
        bool houseNeedsHealth = houseHP.currentBarValue < 100;

        if (playerNeedsHealth || houseNeedsHealth)
        {
            if (TrySpendCoins(healingPrice))
            {
                playerHP.AddHealth(25);
                houseHP.AddHealth(15); // Added House Healing
                Debug.Log("Bought Healing: Player +25, House +15");
            }
        }
        else
        {
            Debug.Log("Health is already full!");
        }
    }

    public void BuyHolyWater()
    {
        if (TrySpendCoins(holyWaterPrice))
        {
            if (playerThrowing != null)
            {
                playerThrowing.AddAmmo(3);
                Debug.Log("Bought 3 Holy Water");
            }
        }
    }

    public void BuyBuff()
    {
        if (GlobalData.DamageMultiplier > 1.0f)
        {
            Debug.Log("Already bought buffs!");
            return;
        }

        if (TrySpendCoins(buffPrice))
        {
            GlobalData.DamageMultiplier = 1.5f;
            GlobalData.SpeedMultiplier = 1.3f;
            Debug.Log("PLAYER BUFFED!");
        }
    }

    private bool TrySpendCoins(int cost)
    {
        if (playerCoinsScript.coins >= cost)
        {
            playerCoinsScript.coins -= cost;
            if (UIManager.Instance != null)
                UIManager.Instance.UpdateCoins(playerCoinsScript.coins);
            return true;
        }
        else
        {
            Debug.Log("Not enough coins!");
            return false;
        }
    }

    // --- SCENE MANAGEMENT ---

    public void ContinueToGame()
    {
        Time.timeScale = 1f;

        // 1. SAVE THE DATA BEFORE LEAVING STORE
        GlobalData.PlayerHealth = playerHP.currentBarValue;
        GlobalData.HouseHealth = houseHP.currentBarValue;
        GlobalData.Coins = playerCoinsScript.coins;
        GlobalData.Ammo = playerShooting.GetCurrentAmmo();

        /*if (playerThrowing != null)
            GlobalData.HolyWaterAmmo = playerThrowing.holyWaterAmmo;*/

        // 2. LOAD NIGHT SCENE
        DayandNightData.Instance.StartNight();
    }


    /*[Header("Player References")]
    [SerializeField] private HP playerHP;
    [SerializeField] private HP houseHP;
    [SerializeField] private PlayerCoins playerCoinsScript;
    [SerializeField] private PlayerShooting playerShooting;
    [SerializeField] private PlayerThrowing playerThrowing;

    [Header("Shop Buttons (Drag UI Objects here)")]
    [SerializeField] private GameObject ammoButton;
    [SerializeField] private GameObject healingButton;
    [SerializeField] private GameObject holyWaterButton;
    [SerializeField] private GameObject buffButton;

    [Header("Prices")]
    public int ammoPrice = 10;
    public int healingPrice = 20;   // Suggestion
    public int holyWaterPrice = 10; // Suggestion
    public int buffPrice = 30;      // Suggestion

    private void Start()
    {
        Time.timeScale = 1f;

        // --- 1. Load Persistent Stats ---
        playerHP.currentBarValue = GlobalData.PlayerHealth;
        houseHP.currentBarValue = GlobalData.HouseHealth;

        if (playerCoinsScript != null)
            playerCoinsScript.coins = GlobalData.Coins;

        if (playerShooting != null)
            playerShooting.SetAmmo(GlobalData.Ammo);

        if (playerThrowing != null)
            playerThrowing.holyWaterAmmo = GlobalData.HolyWaterAmmo;

        // --- 2. Update UI ---
        UIManager.Instance.UpdateAmmo(playerShooting.GetCurrentAmmo(), playerShooting.GetMaxAmmo());
        UIManager.Instance.UpdateCoins(playerCoinsScript.coins);

        // --- 3. Shop Locking Logic ---
        SetupShopForDay();
    }

    private void SetupShopForDay()
    {
        int currentDay = DayandNightData.Instance.currentDay;

        // Day 1: Only Ammo
        if (currentDay == 1)
        {
            if (ammoButton) ammoButton.SetActive(true);
            if (healingButton) healingButton.SetActive(false);
            if (holyWaterButton) holyWaterButton.SetActive(false);
            if (buffButton) buffButton.SetActive(false);
        }
        // Day 2: Ammo + Healing + Holy Water
        else if (currentDay == 2)
        {
            if (ammoButton) ammoButton.SetActive(true);
            if (healingButton) healingButton.SetActive(true);
            if (holyWaterButton) holyWaterButton.SetActive(true);
            if (buffButton) buffButton.SetActive(false);
        }
        // Day 3: Everything unlocked
        else if (currentDay >= 3)
        {
            if (ammoButton) ammoButton.SetActive(true);
            if (healingButton) healingButton.SetActive(true);
            if (holyWaterButton) holyWaterButton.SetActive(true);
            if (buffButton) buffButton.SetActive(true);
        }
    }

    // --- BUYING FUNCTIONS ---

    public void BuyAmmo()
    {
        if (TrySpendCoins(ammoPrice))
        {
            playerShooting.AddAmmo(10);
            Debug.Log("Bought 10 Ammo");
        }
    }

    public void BuyHealing()
    {
        if (TrySpendCoins(healingPrice))
        {
            playerHP.AddHealth(25); // Heals 25 HP
            Debug.Log("Bought Healing");
        }
    }

    public void BuyHolyWater()
    {
        if (TrySpendCoins(holyWaterPrice))
        {
            if (playerThrowing != null)
            {
                playerThrowing.AddAmmo(3); // Gives 3 Holy Water bottles
                Debug.Log("Bought 3 Holy Water");
            }
        }
    }

    public void BuyBuff()
    {
        // Only allow buying the buff once (Optional check)
        if (GlobalData.DamageMultiplier > 1.0f)
        {
            Debug.Log("Already bought buffs!");
            return;
        }

        if (TrySpendCoins(buffPrice))
        {
            // Increase Stats in Global Data
            GlobalData.DamageMultiplier = 1.5f; // 50% more damage
            GlobalData.SpeedMultiplier = 1.3f;  // 30% faster
            Debug.Log("PLAYER BUFFED!");
        }
    }

    // Helper function to check money and subtract it
    private bool TrySpendCoins(int cost)
    {
        if (playerCoinsScript.coins >= cost)
        {
            playerCoinsScript.coins -= cost;
            UIManager.Instance.UpdateCoins(playerCoinsScript.coins);
            return true;
        }
        else
        {
            Debug.Log("Not enough coins!");
            return false;
        }
    }

    // --- SCENE MANAGEMENT ---

    private void SaveCurrentStats()
    {
        GlobalData.PlayerHealth = playerHP.currentBarValue;
        GlobalData.HouseHealth = houseHP.currentBarValue;
        GlobalData.Coins = playerCoinsScript != null ? playerCoinsScript.coins : GlobalData.Coins;
        GlobalData.Ammo = playerShooting != null ? playerShooting.GetCurrentAmmo() : GlobalData.Ammo;
        GlobalData.HolyWaterAmmo = playerThrowing != null ? playerThrowing.holyWaterAmmo : 0;
    }

    public void ContinueToGame()
    {
        Time.timeScale = 1f; // Unpause time

        // 1. SAVE THE DATA
        GlobalData.PlayerHealth = playerHP.currentBarValue;
        GlobalData.HouseHealth = houseHP.currentBarValue;
        GlobalData.Coins = playerCoinsScript.coins;
        GlobalData.Ammo = playerShooting.GetCurrentAmmo();

        if (playerThrowing != null)
            GlobalData.HolyWaterAmmo = playerThrowing.holyWaterAmmo;

        // 2. NOW SWITCH SCENES
        DayandNightData.Instance.StartNight();
    }*/
}
