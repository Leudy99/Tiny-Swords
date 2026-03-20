using System.Collections;
using TMPro;
using UnityEngine;

public class LevelProgressManager : MonoBehaviour
{
    public PlayerLevel playerLevel;
    public PlayerHealth playerHealth;
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

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip victorySound;

    private bool levelCompleted = false;
    private bool bossDefeated = false;

    void Start()
    {
        if (playerLevel != null)
        {
            playerLevel.OnLevelChanged += HandleLevelChanged;
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
    }

    public void SetBossDefeated()
    {
        bossDefeated = true;
    }

    void Update()
    {
        if (levelCompleted) return;
        if (playerLevel == null || enemiesParent == null) return;

        if (enemiesParent.childCount == 0)
        {
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
}