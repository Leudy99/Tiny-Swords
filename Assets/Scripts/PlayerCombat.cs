using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator anim;
    public Transform attackPoint;
    public float attackRange = 0.6f;
    public int attackDamage = 10;
    public LayerMask enemyLayer;

    private PlayerMovement playerMovement;

    public AudioSource audioSource;
    public AudioClip attackSound;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void Attack()
    {
        anim.SetTrigger("Attack");
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }
    public void DealDamage()
    {
        UpdateAttackPointDirection();

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            ElfHealth elfHealth = enemy.GetComponent<ElfHealth>();

            if (elfHealth == null)
                elfHealth = enemy.GetComponentInParent<ElfHealth>();

            if (elfHealth != null)
            {
                elfHealth.TakeDamage(attackDamage);
                continue;
            }

            TrollBossHealth trollBossHealth = enemy.GetComponent<TrollBossHealth>();

            if (trollBossHealth == null)
                trollBossHealth = enemy.GetComponentInParent<TrollBossHealth>();

            if (trollBossHealth != null)
            {
                trollBossHealth.TakeDamage(attackDamage);
            }
        }
    }

    void UpdateAttackPointDirection()
    {
        if (attackPoint == null || playerMovement == null) return;

        Vector3 localPos = attackPoint.localPosition;
        localPos.x = Mathf.Abs(localPos.x) * playerMovement.facingDirection;
        attackPoint.localPosition = localPos;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}