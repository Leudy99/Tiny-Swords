using TMPro;
using UnityEngine;

public class EnemyCountUI : MonoBehaviour
{
    public Transform enemiesParent;
    public TMP_Text enemiesText;

    void Update()
    {
        UpdateEnemyCount();
    }

    void UpdateEnemyCount()
    {
        if (enemiesParent == null || enemiesText == null)
            return;

        int aliveEnemies = 0;

        foreach (Transform child in enemiesParent)
        {
            if (!child.gameObject.activeInHierarchy)
                continue;

            if (child.GetComponent<ElfHealth>() != null || child.GetComponent<TrollBossHealth>() != null)
            {
                aliveEnemies++;
            }
        }

        enemiesText.text = "ENM: " + aliveEnemies;
    }
}