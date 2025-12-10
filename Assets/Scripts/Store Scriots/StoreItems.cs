using UnityEngine;
using TMPro;
using System.Collections;

public class StoreItems : MonoBehaviour
{
    [Header("References")]
    public PlayerCoins playerCoins;
    public PlayerShooting playerShooting;
    public HP playerHP;

    [Header("Item Prices")]
    public int addAmmoPrice = 5;
    public int healPrice = 5;

    [Header("Item Amounts")]
    public int ammoAmount = 5;
    public int healAmount = 10;

    [Header("UI Feedback")]
    public TMP_Text feedbackText;
    public float feedbackDuration = 2f;

    private Coroutine hideCoroutine;

    private void Start()
    {
        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
    }

    public void BuyAddAmmo()
    {
        if (playerCoins.coins >= addAmmoPrice)
        {
            playerCoins.coins -= addAmmoPrice;
            playerCoins.AddCoins(0);
            playerShooting.AddAmmo(ammoAmount);
        }
        else
        {
            ShowFeedback("You don’t have enough coins!");
        }
    }

    public void BuyHealingPotion()
    {
        if (playerCoins.coins >= healPrice)
        {
            playerCoins.coins -= healPrice;
            playerCoins.AddCoins(0);
            playerHP.AddHealth(healAmount);
        }
        else
        {
            ShowFeedback("You don’t have enough coins!");
        }
    }

    private void ShowFeedback(string message)
    {
        if (feedbackText == null) return;

        feedbackText.text = message;
        feedbackText.gameObject.SetActive(true);

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(HideFeedbackAfterDelay());
    }

    private IEnumerator HideFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(feedbackDuration);
        feedbackText.gameObject.SetActive(false);
    }
}
