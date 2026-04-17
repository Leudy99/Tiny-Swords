using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelSystem : MonoBehaviour
{
    [Header("Level")]
    public int currentLevel = 1;
    public int currentExperience = 0;
    public int totalExperience = 0;
    public int experienceToNextLevel = 100;

    [Header("Growth Per Level")]
    public int healthIncreasePerLevel = 20;
    public int damageIncreasePerLevel = 5;
    public int defenseIncreasePerLevel = 1;

    [Header("References")]
    public PlayerHealth playerHealth;
    public PlayerCombat playerCombat;

    [Header("Optional UI")]
    public TMP_Text levelText;
    public TMP_Text xpText;
    public Slider xpSlider;

    private void Start()
    {
        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();

        if (playerCombat == null)
            playerCombat = GetComponent<PlayerCombat>();

        UpdateUI();
    }

    public void AddExperience(int amount)
    {
        totalExperience += amount;
        currentExperience += amount;

        while (currentExperience >= experienceToNextLevel)
        {
            currentExperience -= experienceToNextLevel;
            LevelUp();
        }

        UpdateUI();
    }

    private void LevelUp()
    {
        currentLevel++;

        if (playerHealth != null)
        {
            playerHealth.maxHealth += healthIncreasePerLevel;
            playerHealth.currentHealth = playerHealth.maxHealth;
            playerHealth.defense += defenseIncreasePerLevel;
            playerHealth.UpdateHealthUI();
        }

        if (playerCombat != null)
        {
            playerCombat.attackDamage += damageIncreasePerLevel;
        }

        experienceToNextLevel += 50;

        Debug.Log("¡Level Up! Ahora eres nivel " + currentLevel);
    }

    private void UpdateUI()
    {
        if (levelText != null)
            levelText.text = "LVL: " + currentLevel;

        if (xpText != null)
            xpText.text = "XP: " + currentExperience + "/" + experienceToNextLevel;

        if (xpSlider != null)
        {
            xpSlider.maxValue = experienceToNextLevel;
            xpSlider.value = currentExperience;
        }
    }
}