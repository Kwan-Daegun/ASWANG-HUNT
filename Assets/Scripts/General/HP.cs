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
    [SerializeField] private int bulletDamage = 10; // Defaulted to 10 so bullets actually hurt
    float maxBarValue = 100;

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

        if (BarMask != null)
        {
            float fill = currentBarValue / maxBarValue;
            BarMask.fillAmount = fill;
        }
    }

    public void SubHealth(float number)
    {
        if (number <= 0) return;
        currentBarValue = Mathf.Clamp(currentBarValue - number, 0, maxBarValue);

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
            if (transform.localScale.x < 0)
            {
                Vector3 newScale = originalBarScale;
                newScale.x = -Mathf.Abs(originalBarScale.x);
                BarGO.transform.localScale = newScale;
            }
            else
            {
                Vector3 newScale = originalBarScale;
                newScale.x = Mathf.Abs(originalBarScale.x);
                BarGO.transform.localScale = newScale;
            }
        }

        // --- DEATH LOGIC ---
        if (currentBarValue <= 0)
        {
            // 1. If it is an ENEMY
            if (type == "Enemy")
            {
                EnemyAI enemyAI = GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.Die();
                    return; // Stop here so we don't trigger the player death logic
                }

                // Fallback if no AI script:
                Destroy(gameObject);
            }

            // 2. If it is the PLAYER or the HOUSE
            else if (type == "Player" || type == "House")
            {
                // Trigger the Reset Game function from your Singleton
                if (DayandNightData.Instance != null)
                {
                    DayandNightData.Instance.ResetGame();
                }
                else
                {
                    Debug.LogError("DayandNightData Instance not found! Can't reset game.");
                }

                // Destroy object (optional, since scene will reload anyway, but looks cleaner)
                Destroy(gameObject);
                if (BarGO != null) BarGO.SetActive(false);
            }
        }
    }

    // ---------------- FIRE SYSTEM START ---------------- //

    public void ApplyBurn(int damagePerTick, float tickRate)
    {
        if (isBurning) return;

        isBurning = true;
        burnCoroutine = StartCoroutine(BurnProcess(damagePerTick, tickRate));

        Debug.Log(gameObject.name + " CAUGHT FIRE! Find Holy Water!");

        if (fireParticleEffect != null) fireParticleEffect.SetActive(true);
    }

    public void ExtinguishFire()
    {
        if (!isBurning) return;

        isBurning = false;
        if (burnCoroutine != null) StopCoroutine(burnCoroutine);

        Debug.Log(gameObject.name + " was cleansed with Holy Water!");

        if (fireParticleEffect != null) fireParticleEffect.SetActive(false);
    }

    private IEnumerator BurnProcess(int damage, float rate)
    {
        while (isBurning)
        {
            yield return new WaitForSeconds(rate);

            SubHealth(damage);
            Debug.Log(gameObject.name + " took burn damage...");

            if (currentBarValue <= 0) isBurning = false;
        }
    }
    // ---------------- FIRE SYSTEM END ---------------- //


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // --- HOLY WATER CHECK ---
        if (collision.CompareTag("HolyWater"))
        {
            ExtinguishFire();
        }

        // --- DAMAGE LOGIC ---
        if (type == "House")
        {
            if (collision.CompareTag("Enemy"))
            {
                SubHealth(10);
            }
        }

        if (type == "Enemy")
        {
            if (collision.CompareTag("Bullet"))
            {
                SubHealth(bulletDamage);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (type == "Player")
        {
            if (collision.collider.CompareTag("Enemy"))
            {
                SubHealth(5);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (type == "Player")
        {
            if (collision.collider.CompareTag("Enemy"))
            {
                SubHealth(1);
            }
        }
    }


}
