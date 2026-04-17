using System.Collections;
using TMPro;
using UnityEngine;

public class LevelProgressManager : MonoBehaviour
{
    public PlayerLevel playerLevel;
    public PlayerHealth playerHealth;
    public PlayerLevelSystem playerLevelSystem;
    public Transform playerSpawnPoint;
    public Transform enemiesParent;

    [Header("UI")]
    public GameObject levelCompletePanel;
    public TMP_Text levelCompleteText;
    public CanvasGroup levelCompleteCanvasGroup;

    [Header("Settings")]
    public float fadeDuration = 0.5f;
    public float visibleDuration = 2f;
    public int finalLevel = 5;

    [Header("Completion Rules")]
    public bool canCompleteByKillingAllEnemies = true;
    public bool canCompleteByExperience = true;

    [Header("XP Required Per Level")]
    public int level1RequiredXP = 100;
    public int level2RequiredXP = 150;
    public int level3RequiredXP = 200;
    public int level4RequiredXP = 250;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip victorySound;

    private bool levelCompleted = false;
    private bool bossDefeated = false;
    private int levelStartExperience = 0;

    void Start()
    {
        if (playerLevel != null)
        {
            playerLevel.OnLevelChanged += HandleLevelChanged;
            levelStartExperience = GetCurrentTotalExperience();
        }
    }

    void OnDestroy()
    {
        if (playerLevel != null)
        {
            playerLevel.OnLevelChanged -= HandleLevelChanged;
        }
    }

    private void HandleLevelChanged(int newLevel)
    {
        bossDefeated = false;
        levelStartExperience = GetCurrentTotalExperience();
    }

    public void SetBossDefeated()
    {
        bossDefeated = true;
    }

    void Update()
    {
        if (levelCompleted) return;
        if (playerLevel == null) return;

        bool completedByEnemies = false;
        bool completedByExperience = false;

        if (canCompleteByKillingAllEnemies && enemiesParent != null)
        {
            completedByEnemies = enemiesParent.childCount == 0;
        }

        if (canCompleteByExperience && playerLevelSystem != null)
        {
            int gainedExperienceThisLevel = playerLevelSystem.totalExperience - levelStartExperience;
            int requiredXP = GetRequiredExperienceForCurrentLevel();

            completedByExperience = gainedExperienceThisLevel >= requiredXP;
        }

        bool shouldCompleteLevel = completedByEnemies || completedByExperience;

        if (!shouldCompleteLevel) return;

        if (playerLevel.currentLevel >= finalLevel)
        {
            if (bossDefeated)
            {
                StartCoroutine(HandleLevelComplete());
            }
        }
        else
        {
            StartCoroutine(HandleLevelComplete());
        }
    }

    IEnumerator HandleLevelComplete()
    {
        levelCompleted = true;

        int completedLevel = playerLevel.currentLevel;
        bool isFinalLevel = completedLevel >= finalLevel;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.MuteMusic();
        }

        if (levelCompleteText != null)
        {
            if (isFinalLevel)
                levelCompleteText.text = "YOU WIN";
            else
                levelCompleteText.text = "LEVEL " + completedLevel + " COMPLETE";
        }

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);

        if (levelCompleteCanvasGroup != null)
            levelCompleteCanvasGroup.alpha = 0f;

        if (audioSource != null && victorySound != null)
            audioSource.PlayOneShot(victorySound);

        yield return StartCoroutine(FadeCanvasGroup(0f, 1f, fadeDuration));
        yield return new WaitForSeconds(visibleDuration);
        yield return StartCoroutine(FadeCanvasGroup(1f, 0f, fadeDuration));

        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);

        if (playerLevel != null)
        {
            if (isFinalLevel)
                playerLevel.SetLevel(1);
            else
                playerLevel.SetLevel(completedLevel + 1);
        }

        if (playerHealth != null)
        {
            playerHealth.ResetPlayerState();
        }

        if (playerHealth != null && playerSpawnPoint != null)
        {
            playerHealth.transform.position = playerSpawnPoint.position;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.UpdateMusicForLevel(playerLevel.currentLevel);
            AudioManager.Instance.UnmuteMusic();
        }

        levelCompleted = false;
    }

    IEnumerator FadeCanvasGroup(float start, float end, float duration)
    {
        if (levelCompleteCanvasGroup == null)
            yield break;

        float elapsed = 0f;
        levelCompleteCanvasGroup.alpha = start;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            levelCompleteCanvasGroup.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        levelCompleteCanvasGroup.alpha = end;
    }

    private int GetCurrentTotalExperience()
    {
        if (playerLevelSystem != null)
            return playerLevelSystem.totalExperience;

        return 0;
    }

    private int GetRequiredExperienceForCurrentLevel()
    {
        switch (playerLevel.currentLevel)
        {
            case 1:
                return level1RequiredXP;
            case 2:
                return level2RequiredXP;
            case 3:
                return level3RequiredXP;
            case 4:
                return level4RequiredXP;
            default:
                return 999999;
        }
    }
}