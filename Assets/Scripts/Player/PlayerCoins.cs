using UnityEngine;

public class PlayerCoins : MonoBehaviour
{
    public int coins = 0;

    void Start()
    {
        // IMPORTANT: Load coins from GlobalData
        coins = GlobalData.Coins;

        // Update the UI
        UIManager.Instance.UpdateCoins(coins);
    }


    public void AddCoins(int amount)
    {
        coins += amount;
        Debug.Log("Total Coins: " + coins);
        UIManager.Instance.UpdateCoins(coins);
    }
}
