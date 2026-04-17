using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    [Header("Defense")]
    public int defense = 0;
    public int minimumDamageTaken = 1;

    public TMP_Text healthText;
    public Animator healthTextAnim;

    [Header("Health Bar")]
    public RectTransform healthFill;
    public float maxFillWidth = 160f;

    public DamageFlash damageFlash;

    public AudioSource audioSource;
    public AudioClip hurtSound;
    public AudioClip deathSound;

    [Header("Death")]
    public float deathDelay = 0.5f;

    [Header("Game Over")]
    public GameOverManager gameOverManager;

    private bool isDead = false;

    private void Start()
    {
        UpdateHealthUI();
    }

    public void TakeDamage(int incomingDamage)
    {
        if (isDead)
            return;

        int finalDamage = incomingDamage - defense;

        if (finalDamage < minimumDamageTaken)
            finalDamage = minimumDamageTaken;

        ChangeHealth(-finalDamage);
    }

    public void ChangeHealth(int amount)
    {
        if (isDead)
            return;

        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        if (currentHealth < 0)
            currentHealth = 0;

        if (amount < 0)
        {
            if (damageFlash != null)
            {
                damageFlash.Flash();
            }

            if (audioSource != null && hurtSound != null)
            {
                audioSource.PlayOneShot(hurtSound);
            }
        }

        if (healthTextAnim != null)
        {
            healthTextAnim.Play("TextUpdate");
        }

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        if (isDead) yield break;
        isDead = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = false;
        }

        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null)
            playerMovement.enabled = false;

        PlayerCombat playerCombat = GetComponent<PlayerCombat>();
        if (playerCombat != null)
            playerCombat.enabled = false;

        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetFloat("horizontal", 0);
            anim.SetFloat("vertical", 0);
        }

        if (audioSource != null && deathSound != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(deathSound);
            yield return new WaitForSeconds(deathSound.length);
        }
        else
        {
            yield return new WaitForSeconds(deathDelay);
        }

        Debug.Log("Player murió, llamando a GameOverManager");

        if (gameOverManager != null)
        {
            gameOverManager.TriggerGameOver();
        }
        else
        {
            Debug.Log("GameOverManager es NULL");
        }
    }

    public void ResetPlayerState()
    {
        currentHealth = maxHealth;
        isDead = false;

        UpdateHealthUI();

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = true;
        }

        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null)
            playerMovement.enabled = true;

        PlayerCombat playerCombat = GetComponent<PlayerCombat>();
        if (playerCombat != null)
            playerCombat.enabled = true;

        gameObject.SetActive(true);
    }

    public void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "HP: " + currentHealth + "/" + maxHealth;
        }

        if (healthFill != null)
        {
            float healthPercent = (float)currentHealth / maxHealth;
            healthFill.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxFillWidth * healthPercent);
        }
    }
}