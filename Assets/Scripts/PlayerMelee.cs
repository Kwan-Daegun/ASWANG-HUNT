using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    [Header("Attack Settings")]
    public Transform attackOrigin;
    public float attackRadius = 1f;
    public LayerMask enemyMask;
    public int attackDamage = 25;
    public float cooldownTime = 0.5f;

    private float cooldownTimer = 0f;

    [Header("References")]
    public Animator animator;

    private void Update()
    {
        HandleCooldown();

        if (cooldownTimer <= 0 && Input.GetKey(KeyCode.K))
        {
            PerformMeleeAttack();
            cooldownTimer = cooldownTime;
        }
    }

    private void HandleCooldown()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;
    }

    private void PerformMeleeAttack()
    {
        if (animator != null)
            animator.SetTrigger("Melee");

        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(attackOrigin.position, attackRadius, enemyMask);

        foreach (var enemy in enemiesInRange)
        {
            HP enemyHP = enemy.GetComponent<HP>();
            if (enemyHP != null)
                enemyHP.SubHealth(attackDamage);
        }
    }

    private void OnDrawGizmos()
    {
        if (attackOrigin != null)
            Gizmos.DrawWireSphere(attackOrigin.position, attackRadius);
    }
}
