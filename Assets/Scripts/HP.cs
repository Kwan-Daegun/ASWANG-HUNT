using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour
{
    [SerializeField] private string type;
    [SerializeField] private GameObject HPGO;
    [SerializeField] private GameObject BarGO;
    [SerializeField] private Image BarMask;
    [SerializeField] public float currentBarValue;
    [SerializeField] private int bulletDamage = 0;
    float maxBarValue = 100;

    private Vector3 originalBarScale;

    [Header("Status Effects")]
    public bool isBurning = false; // Is this object currently on fire?
    private Coroutine burnCoroutine;
    [SerializeField] private GameObject fireParticleEffect; // Optional: Drag a fire prefab here

    void Start()
    {
        BarGO.SetActive(true);

        if (BarGO != null)
        {
            originalBarScale = BarGO.transform.localScale;
        }
    }

    public void AddHealth(float number)
    {
        if (number <= 0) return;
        currentBarValue = Mathf.Clamp(currentBarValue + number, 0, maxBarValue);

        float fill = currentBarValue / maxBarValue;
        BarMask.fillAmount = fill;
    }

    public void SubHealth(float number)
    {
        if (number <= 0) return;
        currentBarValue = Mathf.Clamp(currentBarValue - number, 0, maxBarValue);

        float fill = currentBarValue / maxBarValue;
        BarMask.fillAmount = fill;
    }

    private void Update()
    {
        // Bar flipping logic
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

        // Death Logic
        if (currentBarValue <= 0)
        {
            if (type == "Enemy")
            {
                // Assuming Main class handles static counters
                // Main.MinusEnemyCounter(); 

                EnemyAI enemyAI = GetComponent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.Die();
                    return;
                }
            }

            // If House or Player dies
            Destroy(gameObject);
            if (BarGO != null) BarGO.SetActive(false);
        }
    }

    // ---------------- FIRE SYSTEM START ---------------- //

    public void ApplyBurn(int damagePerTick, float tickRate)
    {
        if (isBurning) return; // If already burning, ignore (or you could reset the tick)

        isBurning = true;
        // Start the infinite loop
        burnCoroutine = StartCoroutine(BurnProcess(damagePerTick, tickRate));

        Debug.Log(gameObject.name + " CAUGHT FIRE! Find Holy Water!");

        if (fireParticleEffect != null) fireParticleEffect.SetActive(true);
    }

    // The logic to STOP the fire
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
        // Infinite loop: Run as long as 'isBurning' is true
        while (isBurning)
        {
            yield return new WaitForSeconds(rate); // Wait for the tick (1.5s)

            // Take damage
            SubHealth(damage);
            Debug.Log(gameObject.name + " took burn damage...");

            // If we die from burning, stop the loop
            if (currentBarValue <= 0) isBurning = false;
        }
    }
    // ---------------- FIRE SYSTEM END ---------------- //


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // --- HOLY WATER CHECK ---
        // If this object (House or Player) is hit by Holy Water while burning...
        if (collision.CompareTag("HolyWater"))
        {
            ExtinguishFire();
            // Optional: Destroy the water bottle so it looks like it was used up
            // Destroy(collision.gameObject); 
        }

        // --- EXISTING LOGIC ---
        if (type == "House")
        {
            if (collision.CompareTag("Enemy"))
            {
                // Main.MinusEnemyCounter();
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
