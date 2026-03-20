using System.Collections;
using TMPro;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public PlayerLevel playerLevel;
    public PlayerHealth playerHealth;
    public Transform playerSpawnPoint;

    [Header("UI")]
    public GameObject gameOverPanel;
    public TMP_Text gameOverText;
    public CanvasGroup gameOverCanvasGroup;

    [Header("Settings")]
    public float fadeDuration = 0.5f;
    public float visibleDuration = 2f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip loseSound;

    private bool isHandlingGameOver = false;

    public void TriggerGameOver()
    {
        if (isHandlingGameOver) return;
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        isHandlingGameOver = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.MuteMusic();
        }

        if (gameOverText != null)
            gameOverText.text = "YOU LOSE";

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (gameOverCanvasGroup != null)
            gameOverCanvasGroup.alpha = 0f;

        if (audioSource != null && loseSound != null)
            audioSource.PlayOneShot(loseSound);

        yield return StartCoroutine(FadeCanvasGroup(0f, 1f, fadeDuration));
        yield return new WaitForSeconds(visibleDuration);
        yield return StartCoroutine(FadeCanvasGroup(1f, 0f, fadeDuration));

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (playerLevel != null)
            playerLevel.SetLevel(1);

        if (playerHealth != null)
            playerHealth.ResetPlayerState();

        if (playerHealth != null && playerSpawnPoint != null)
        {
            playerHealth.transform.position = playerSpawnPoint.position;
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.UpdateMusicForLevel(1);
            AudioManager.Instance.UnmuteMusic();
        }

        isHandlingGameOver = false;
    }

    private IEnumerator FadeCanvasGroup(float start, float end, float duration)
    {
        if (gameOverCanvasGroup == null)
            yield break;

        float elapsed = 0f;
        gameOverCanvasGroup.alpha = start;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            gameOverCanvasGroup.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        gameOverCanvasGroup.alpha = end;
    }
}