using System.Collections;
using UnityEngine;

public class ElfHealth : MonoBehaviour
{
    public int currentHealth = 50;
    public int maxHealth = 50;

    [Header("Health Bar")]
    public Transform healthFill;

    private Vector3 originalScale;

    public DamageFlash damageFlash;

    public AudioSource audioSource;
    public AudioClip hurtSound;
    public AudioClip deathSound;

    private bool isDead = false;

    void Start()
    {
        if (healthFill != null)
        {
            originalScale = healthFill.localScale;
        }

        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
         currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        if (damageFlash != null)
        {
            damageFlash.Flash();
        }

        if (audioSource != null && hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

        private IEnumerator Die()
    {
        if (isDead) yield break;
        isDead = true;

        // parar movimiento del enemigo
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        // desactivar colliders
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        // desactivar scripts del enemigo que no deben seguir funcionando
        ElfMovement elfMovement = GetComponent<ElfMovement>();
        if (elfMovement != null)
            elfMovement.enabled = false;

        ElfCombat elfCombat = GetComponent<ElfCombat>();
        if (elfCombat != null)
            elfCombat.enabled = false;

        // opcional: si tienes animator y quieres dejarlo quieto
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetBool("isIdle", false);
            anim.SetBool("isChasing", false);
            anim.SetBool("isAttacking", false);
        }

        // reproducir sonido de muerte
        if (audioSource != null && deathSound != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(deathSound);
            yield return new WaitForSeconds(deathSound.length);
        }
        else
        {
            yield return new WaitForSeconds(0.2f);
        }

        Destroy(gameObject);
    }
    void UpdateHealthBar()
    {
        if (healthFill != null)
        {
            float healthPercent = (float)currentHealth / maxHealth;

            healthFill.localScale = new Vector3(
                originalScale.x * healthPercent,
                originalScale.y,
                originalScale.z
            );
        }
    }
}