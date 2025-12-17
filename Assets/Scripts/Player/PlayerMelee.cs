using System.Collections;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackOrigin; // If empty, damage comes from Player center
    public float attackRadius = 2f; // RADIUS OF THE GREEN GIZMO
    public LayerMask enemyMask;
    public int baseDamage = 25;
    public float cooldownTime = 0.5f;

    [Header("Slash Visuals")]
    public Sprite[] slashSprites;
    public float frameSpeed = 0.04f;

    // ADJUST THIS to move the Yellow Box (Visual)
    public Vector3 slashOffset = new Vector3(1f, 0.2f, 0f);
    public int sortOrder = 10;

    [Header("Audio")]
    public AudioClip attackSound;

    [Header("References")]
    private AudioSource audioSource;
    private float cooldownTimer = 0f;
    private SpriteRenderer slashRenderer;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Create Slash Object
        GameObject slashObj = new GameObject("SlashEffect_Auto");
        slashObj.transform.parent = transform;
        slashObj.transform.localPosition = slashOffset;
        slashObj.transform.localScale = Vector3.one;

        slashRenderer = slashObj.AddComponent<SpriteRenderer>();
        slashRenderer.sortingOrder = sortOrder;
        slashRenderer.enabled = false;
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
        if (attackSound != null && audioSource != null)
            audioSource.PlayOneShot(attackSound);

        if (slashRenderer != null && slashSprites.Length > 0)
        {
            slashRenderer.enabled = true;
            for (int i = 0; i < slashSprites.Length; i++)
            {
                slashRenderer.sprite = slashSprites[i];
                yield return new WaitForSeconds(frameSpeed);
            }
            slashRenderer.enabled = false;
        }

        int finalDamage = Mathf.RoundToInt(baseDamage * GlobalData.DamageMultiplier);

        // DAMAGE LOGIC: Handles null attackOrigin automatically
        Vector2 origin = (attackOrigin != null) ? attackOrigin.position : transform.position;

        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(origin, attackRadius, enemyMask);

        foreach (var enemy in enemiesInRange)
        {
            HP enemyHP = enemy.GetComponent<HP>();
            if (enemyHP != null)
            {
                enemyHP.SubHealth(finalDamage);
            }
        }
    }

    // --- IMPROVED GIZMO DRAWING ---
    private void OnDrawGizmos()
    {
        // 1. Determine Damage Center (Handle null case)
        Vector3 damageCenter = (attackOrigin != null) ? attackOrigin.position : transform.position;

        // 2. Draw Damage Range (GREEN CIRCLE)
        // Green = Ready to Attack, Red = Cooldown
        Gizmos.color = (cooldownTimer <= 0) ? Color.green : Color.red;
        Gizmos.DrawWireSphere(damageCenter, attackRadius);

        // 3. Draw Visual Location (YELLOW BOX)
        // This shows where the Slash Image appears relative to the player
        Gizmos.color = Color.yellow;

        // Calculate visual position based on facing direction
        Vector3 visualPos = transform.position + slashOffset;
        if (transform.localScale.x < 0) // If player is facing left
            visualPos = transform.position + new Vector3(-slashOffset.x, slashOffset.y, slashOffset.z);

        Gizmos.DrawWireCube(visualPos, new Vector3(0.5f, 0.5f, 0.1f));

        // 4. Draw a line connecting them to help you align them
        Gizmos.color = Color.white;
        Gizmos.DrawLine(damageCenter, visualPos);
    }



    /*[Header("Attack Settings")]
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
    }*/
}
