using System.Collections;
using UnityEngine;

public class TrollBossHealth : MonoBehaviour
{
    [Header("Health")]
    public int currentHealth = 500;
    public int maxHealth = 500;

    [Header("UI")]
    public RectTransform healthFill;
    public float maxFillWidth = 300f;
    public GameObject bossHealthUI;

    [Header("Effects")]
    public DamageFlash damageFlash;
    public AudioSource audioSource;
    public AudioClip hurtSound;
    public AudioClip deathSound;

    [Header("Death")]
    public float deathDelay = 2.5f;

    private Animator animator;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (bossHealthUI == null)
        {
            GameObject foundBossUI = GameObject.Find("BossHealthUI");
            if (foundBossUI != null)
            {
                bossHealthUI = foundBossUI;
            }
        }

        if (healthFill == null && bossHealthUI != null)
        {
            Transform fill = bossHealthUI.transform.Find("FillMask1/Fill1");
            if (fill != null)
            {
                healthFill = fill.GetComponent<RectTransform>();
            }
        }

        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        UpdateHealthBar();

        if (damageFlash != null)
            damageFlash.Flash();

        if (audioSource != null && hurtSound != null && currentHealth > 0)
            audioSource.PlayOneShot(hurtSound);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthFill != null)
        {
            float healthPercent = (float)currentHealth / maxHealth;
            healthFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxFillWidth * healthPercent);
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        LevelProgressManager levelProgressManager = FindFirstObjectByType<LevelProgressManager>();
        if (levelProgressManager != null)
        {
            levelProgressManager.SetBossDefeated();
        }

        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);

        if (animator != null)
        {
            animator.SetBool("isDead", true);
            animator.SetBool("isChasing", false);
            animator.SetBool("isWindingUp", false);
            animator.SetBool("isAttacking", false);
            animator.SetBool("isRecovering", false);
            animator.SetBool("isIdle", false);
        }

        TrollBossController bossController = GetComponent<TrollBossController>();
        if (bossController != null)
            bossController.enabled = false;

        TrollBossCombat bossCombat = GetComponent<TrollBossCombat>();
        if (bossCombat != null)
            bossCombat.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(deathDelay);

        if (bossHealthUI != null)
            bossHealthUI.SetActive(false);

        Destroy(gameObject);
    }
}