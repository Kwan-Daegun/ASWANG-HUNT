using System.Collections;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackOrigin;
    public float attackRadius = 1f;
    public LayerMask enemyMask;
    public int baseDamage = 25;
    public float cooldownTime = 0.5f;

    [Header("Visuals & Audio")]
    // RENAME: Used to be slashEffect, now it's slashPrefab to be clear
    public GameObject slashPrefab;
    public float effectDuration = 0.15f;
    public AudioClip attackSound;

    [Header("References")]
    public Animator animator;
    private AudioSource audioSource;
    private float cooldownTimer = 0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0 && Input.GetMouseButtonDown(1))
        {
            StartCoroutine(PerformAttackSequence());
            cooldownTimer = cooldownTime;
        }
    }

    private IEnumerator PerformAttackSequence()
    {
        // 1. Play Animation
        if (animator != null) animator.SetTrigger("Melee");

        // 2. Play Sound
        if (attackSound != null && audioSource != null)
            audioSource.PlayOneShot(attackSound);

        // 3. SPAWN THE SLASH PREFAB (The Fix)
        if (slashPrefab != null && attackOrigin != null)
        {
            // Create the clone at the attack position
            // We use transform.rotation to ensure it faces the same way as the player
            GameObject currentSlash = Instantiate(slashPrefab, attackOrigin.position, transform.rotation);

            // Destroy the clone after the duration finishes to keep the game clean
            Destroy(currentSlash, effectDuration);
        }

        // 4. Calculate Damage
        int finalDamage = Mathf.RoundToInt(baseDamage * GlobalData.DamageMultiplier);
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(attackOrigin.position, attackRadius, enemyMask);

        foreach (var enemy in enemiesInRange)
        {
            HP enemyHP = enemy.GetComponent<HP>();
            if (enemyHP != null)
            {
                enemyHP.SubHealth(finalDamage);
            }
        }

        // 5. Wait slightly before allowing next attack (optional animation sync)
        yield return new WaitForSeconds(effectDuration);
    }

    private void OnDrawGizmos()
    {
        if (attackOrigin != null)
        {
            Gizmos.color = (cooldownTimer <= 0) ? Color.green : Color.red;
            Gizmos.DrawWireSphere(attackOrigin.position, attackRadius);
        }
    }
}
