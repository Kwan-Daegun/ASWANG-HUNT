using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Set this to 'Player', 'House', or 'Enemy'")]
    [SerializeField] private string type;

    [Header("UI References")]
    [SerializeField] private GameObject HPGO;
    [SerializeField] private GameObject BarGO;
    [SerializeField] private Image BarMask;

    [Header("Stats")]
    [SerializeField] public float currentBarValue;
    float maxBarValue = 100;

    // Bullet damage logic should usually be on the Bullet, but we can keep it here for enemies
    [SerializeField] private int bulletDamage = 10;

    private Vector3 originalBarScale;

    [Header("Status Effects")]
    public bool isBurning = false;
    private Coroutine burnCoroutine;
    [SerializeField] private GameObject fireParticleEffect;

    void Start()
    {
        if (BarGO != null)
        {
            BarGO.SetActive(true);
            originalBarScale = BarGO.transform.localScale;
        }

        // Initialize health fill
        if (BarMask != null)
        {
            float fill = currentBarValue / maxBarValue;
            BarMask.fillAmount = fill;
        }
    }

    public void AddHealth(float number)
    {
        if (number <= 0) return;
        currentBarValue = Mathf.Clamp(currentBarValue + number, 0, maxBarValue);
        UpdateUI();
    }

    public void SubHealth(float number)
    {
        if (number <= 0) return;
        currentBarValue = Mathf.Clamp(currentBarValue - number, 0, maxBarValue);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (BarMask != null)
        {
            float fill = currentBarValue / maxBarValue;
            BarMask.fillAmount = fill;
        }
    }

    private void Update()
    {
        // --- BAR FLIPPING LOGIC ---
        if (BarGO != null)
        {
            // If the object flips (scale.x is negative), flip the bar back so it looks normal
            float direction = (transform.localScale.x < 0) ? -1 : 1;

            // We only modify the X scale, keeping the original size
            Vector3 newScale = originalBarScale;
            newScale.x = Mathf.Abs(originalBarScale.x) * (direction == -1 ? -1 : 1);
            // Note: Depending on your hierarchy, you might just want newScale.x = Mathf.Abs... 
            // usually bars shouldn't flip with the character, but this logic tries to correct it.
            if (transform.localScale.x < 0)
            {
                newScale.x = -Mathf.Abs(originalBarScale.x);
            }
            else
            {
                newScale.x = Mathf.Abs(originalBarScale.x);
            }
            BarGO.transform.localScale = newScale;
        }

        // --- DEATH LOGIC ---
        if (currentBarValue <= 0)
        {
            if (type == "Enemy")
            {
                // Check if it has the AI script to handle drops
                EnemyAI enemyAI = GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.Die();
                    return;
                }

                TikbalangMovement tikbalang = GetComponent<TikbalangMovement>();
                if (tikbalang != null)
                {
                    tikbalang.Die();
                    return;
                }

                // Santelmo check
                SantelmoScript santelmo = GetComponent<SantelmoScript>();
                if (santelmo != null)
                {
                    santelmo.ExtinguishSelf(); // Or a Die function
                    return;
                }

                Destroy(gameObject);
            }

            else if (type == "Player" || type == "House")
            {
                NightManager manager = FindObjectOfType<NightManager>();

                if (manager != null)
                {
                    manager.ShowGameOver();
                }
                else
                {
                    Debug.LogError("NightManager not found!");
                    if (DayandNightData.Instance != null)
                        DayandNightData.Instance.ResetGame();
                }

                gameObject.SetActive(false);
            }
        }
    }

    // ---------------- FIRE SYSTEM ---------------- //
    public void ApplyBurn(int damagePerTick, float tickRate)
    {
        if (isBurning) return;
        isBurning = true;
        burnCoroutine = StartCoroutine(BurnProcess(damagePerTick, tickRate));
        if (fireParticleEffect != null) fireParticleEffect.SetActive(true);
    }

    public void ExtinguishFire()
    {
        if (!isBurning) return;
        isBurning = false;
        if (burnCoroutine != null) StopCoroutine(burnCoroutine);
        if (fireParticleEffect != null) fireParticleEffect.SetActive(false);
    }

    private IEnumerator BurnProcess(int damage, float rate)
    {
        while (isBurning)
        {
            yield return new WaitForSeconds(rate);
            SubHealth(damage);
            if (currentBarValue <= 0) isBurning = false;
        }
    }

    // ---------------- COLLISIONS ---------------- //

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Holy Water (Cure Fire)
        if (collision.CompareTag("HolyWater"))
        {
            ExtinguishFire();
        }

        // 2. Bullets hitting Enemy
        // (We keep this here so your shooting still works)
        if (type == "Enemy")
        {
            if (collision.CompareTag("Bullet"))
            {
                SubHealth(bulletDamage);
                // Destroy bullet so it doesn't go through enemies
                Destroy(collision.gameObject);
            }
        }
    }

}
