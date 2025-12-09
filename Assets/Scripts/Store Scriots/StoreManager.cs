using UnityEngine;
using UnityEngine.SceneManagement;

public class StoreManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HP playerHP;
    [SerializeField] private HP houseHP;
    [SerializeField] private PlayerCoins playerCoinsScript;
    [SerializeField] private PlayerShooting playerShooting;

    private void Start()
    {
        Time.timeScale = 1f;

        // Load persistent stats
        playerHP.currentBarValue = GlobalData.PlayerHealth;
        houseHP.currentBarValue = GlobalData.HouseHealth;

        if (playerCoinsScript != null)
            playerCoinsScript.coins = GlobalData.Coins;

        if (playerShooting != null)
            playerShooting.SetAmmo(GlobalData.Ammo);

        UIManager.Instance.UpdateAmmo(playerShooting.GetCurrentAmmo(), playerShooting.GetMaxAmmo());
        UIManager.Instance.UpdateCoins(playerCoinsScript.coins);
    }

    private void SaveCurrentStats()
    {
        GlobalData.PlayerHealth = playerHP.currentBarValue;
        GlobalData.HouseHealth = houseHP.currentBarValue;
        GlobalData.Coins = playerCoinsScript != null ? playerCoinsScript.coins : GlobalData.Coins;
        GlobalData.Ammo = playerShooting != null ? playerShooting.GetCurrentAmmo() : GlobalData.Ammo;
    }

    public void HealPlayer(int amount = 10) => playerHP.AddHealth(amount);
    public void RepairHouse(int amount = 10) => houseHP.AddHealth(amount);

    public void BuyItem(int cost)
    {
        if (playerCoinsScript.coins >= cost)
            playerCoinsScript.coins -= cost;
        else
            Debug.Log("Not enough coins!");

        UIManager.Instance.UpdateCoins(playerCoinsScript.coins);
    }

    public void ContinueToGame()
    {
        SaveCurrentStats();
        SceneManager.LoadScene("LevelOne");
    }

    public void Home()
    {
        SaveCurrentStats();
        Time.timeScale = 1f;
        SceneManager.LoadScene("newMenu");
    }

    public void ExitGame() => Application.Quit();
}
