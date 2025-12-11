using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
public class StoreManager : MonoBehaviour
{
    [Header("UI Announcement")]
    [SerializeField] private TMP_Text announcementText;
    [SerializeField] private float announcementDuration = 4f;
    [Header("Player References")]
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
    public int holyWaterPrice = 20; // Suggestion
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
        ShowAnnouncement("Shop", announcementDuration);
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
    }
    private void ShowAnnouncement(string message, float duration)
{
    if (announcementText == null) return;

    StopAllCoroutines();
    StartCoroutine(ShowAnnouncementRoutine(message, duration));
}

private IEnumerator ShowAnnouncementRoutine(string message, float duration)
{
    announcementText.text = message;
    announcementText.gameObject.SetActive(true);

    yield return new WaitForSeconds(duration);

    announcementText.gameObject.SetActive(false);
}
}
