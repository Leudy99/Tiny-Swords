using System.Collections.Generic;
using UnityEngine;

public class LevelEnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnEntry
    {
        public GameObject enemyPrefab;
        public int quantity = 1;
    }

    [System.Serializable]
    public class LevelEnemyGroup
    {
        public int level = 1;
        public List<EnemySpawnEntry> enemies = new List<EnemySpawnEntry>();
    }

    [Header("References")]
    public PlayerLevel playerLevel;
    public Transform enemiesParent;
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("Level Config")]
    public List<LevelEnemyGroup> levelEnemies = new List<LevelEnemyGroup>();

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Start()
    {
        if (playerLevel != null)
        {
            playerLevel.OnLevelChanged += HandleLevelChanged;
            SpawnEnemiesForLevel(playerLevel.currentLevel);
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
        SpawnEnemiesForLevel(newLevel);
    }

    public void SpawnEnemiesForLevel(int level)
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("No hay spawn points asignados en LevelEnemySpawner.");
            return;
        }

        if (enemiesParent != null)
        {
            enemiesParent.gameObject.SetActive(true);
        }

        ClearCurrentEnemies();

        LevelEnemyGroup group = levelEnemies.Find(g => g.level == level);

        if (group == null)
        {
            Debug.LogWarning("No hay configuración de enemigos para el nivel " + level);
            return;
        }

        List<Transform> availablePoints = new List<Transform>(spawnPoints);

        foreach (EnemySpawnEntry entry in group.enemies)
        {
            if (entry.enemyPrefab == null || entry.quantity <= 0)
                continue;

            for (int i = 0; i < entry.quantity; i++)
            {
                if (availablePoints.Count == 0)
                {
                    Debug.LogWarning("No hay suficientes spawn points para colocar más enemigos en el nivel " + level);
                    break;
                }

                int randomIndex = Random.Range(0, availablePoints.Count);
                Transform selectedPoint = availablePoints[randomIndex];
                availablePoints.RemoveAt(randomIndex);

                GameObject spawned = Instantiate(entry.enemyPrefab, selectedPoint.position, Quaternion.identity);

                if (enemiesParent != null)
                    spawned.transform.SetParent(enemiesParent);

                spawnedEnemies.Add(spawned);
            }
        }
    }

    public void ClearCurrentEnemies()
    {
        for (int i = 0; i < spawnedEnemies.Count; i++)
        {
            if (spawnedEnemies[i] != null)
                Destroy(spawnedEnemies[i]);
        }

        spawnedEnemies.Clear();
    }
}