using System.Collections.Generic;
using UnityEngine;

public class LevelPickupSpawner : MonoBehaviour
{
    [System.Serializable]
    public class PickupSpawnEntry
    {
        public GameObject pickupPrefab;
        public int quantity = 1;
    }

    [System.Serializable]
    public class LevelPickupGroup
    {
        public int level = 1;
        public List<PickupSpawnEntry> pickups = new List<PickupSpawnEntry>();
    }

    [Header("References")]
    public PlayerLevel playerLevel;
    public Transform pickupsParent;
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("Level Config")]
    public List<LevelPickupGroup> levelPickups = new List<LevelPickupGroup>();

    private List<GameObject> spawnedPickups = new List<GameObject>();

    void Start()
    {
        if (playerLevel != null)
        {
            playerLevel.OnLevelChanged += HandleLevelChanged;
            SpawnPickupsForLevel(playerLevel.currentLevel);
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
        SpawnPickupsForLevel(newLevel);
    }

    public void SpawnPickupsForLevel(int level)
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("No hay spawn points asignados en LevelPickupSpawner.");
            return;
        }

        if (pickupsParent != null)
        {
            pickupsParent.gameObject.SetActive(true);
        }

        ClearCurrentPickups();

        LevelPickupGroup group = levelPickups.Find(g => g.level == level);

        if (group == null)
        {
            Debug.LogWarning("No hay configuración de pickups para el nivel " + level);

            if (pickupsParent != null)
                pickupsParent.gameObject.SetActive(false);

            return;
        }

        List<Transform> availablePoints = new List<Transform>(spawnPoints);

        foreach (PickupSpawnEntry entry in group.pickups)
        {
            if (entry.pickupPrefab == null || entry.quantity <= 0)
                continue;

            for (int i = 0; i < entry.quantity; i++)
            {
                if (availablePoints.Count == 0)
                {
                    Debug.LogWarning("No hay suficientes spawn points para colocar más pickups en el nivel " + level);
                    break;
                }

                int randomIndex = Random.Range(0, availablePoints.Count);
                Transform selectedPoint = availablePoints[randomIndex];
                availablePoints.RemoveAt(randomIndex);

                GameObject spawned = Instantiate(entry.pickupPrefab, selectedPoint.position, Quaternion.identity);

                if (pickupsParent != null)
                    spawned.transform.SetParent(pickupsParent);

                spawnedPickups.Add(spawned);
            }
        }
    }

    public void ClearCurrentPickups()
    {
        for (int i = 0; i < spawnedPickups.Count; i++)
        {
            if (spawnedPickups[i] != null)
                Destroy(spawnedPickups[i]);
        }

        spawnedPickups.Clear();
    }
}