using TMPro;
using UnityEngine;
using System;

public class PlayerLevel : MonoBehaviour
{
    public int currentLevel = 1;
    public TMP_Text levelText;

    [Header("Boss UI")]
    public GameObject bossHealthUI;
    public int bossLevel = 5;

    public event Action<int> OnLevelChanged;

    void Start()
    {
        UpdateLevelUI();
        UpdateBossUI();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.UpdateMusicForLevel(currentLevel);
        }
    }

    public void SetLevel(int newLevel)
    {
        currentLevel = newLevel;
        UpdateLevelUI();
        UpdateBossUI();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.UpdateMusicForLevel(currentLevel);
        }

        OnLevelChanged?.Invoke(currentLevel);
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = "LVL: " + currentLevel;
        }
    }

    private void UpdateBossUI()
    {
        if (bossHealthUI != null)
        {
            bossHealthUI.SetActive(currentLevel >= bossLevel);
        }
    }
}