using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float enemySpawnInterval = 2f;

    // --- NIGHT 1 ARRAYS ---
    [Header("--- NIGHT 1 WAVES ---")]
    [SerializeField] private GameObject[] Night1_Wave1;
    [SerializeField] private GameObject[] Night1_Wave2;
    [SerializeField] private GameObject[] Night1_Wave3;

    // --- NIGHT 2 ARRAYS ---
    [Header("--- NIGHT 2 WAVES ---")]
    [SerializeField] private GameObject[] Night2_Wave1;
    [SerializeField] private GameObject[] Night2_Wave2;
    [SerializeField] private GameObject[] Night2_Wave3;
    [SerializeField] private GameObject[] Night2_Wave4; // Extra wave for Night 2

    // --- NIGHT 3 ARRAYS ---
    [Header("--- NIGHT 3 WAVES ---")]
    [SerializeField] private GameObject[] Night3_Wave1;
    [SerializeField] private GameObject[] Night3_Wave2;
    [SerializeField] private GameObject[] Night3_Wave3;
    [SerializeField] private GameObject[] Night3_Wave4;
    [SerializeField] private GameObject[] Night3_Wave5; // Hardest night!

    // Internal State
    public bool isSpawning = false;

    public void StartWave(int waveIndex)
    {
        if (isSpawning) return;
        StartCoroutine(SpawnRoutine(waveIndex));
    }

    IEnumerator SpawnRoutine(int waveIndex)
    {
        isSpawning = true;

        // 1. Ask: "What day is it?" and "What wave is it?"
        GameObject[] enemiesToSpawn = GetEnemyList(waveIndex);

        if (enemiesToSpawn.Length == 0)
        {
            // No enemies configured for this wave? Done.
            isSpawning = false;
            yield break;
        }

        // 2. Spawn them one by one
        for (int i = 0; i < enemiesToSpawn.Length; i++)
        {
            GameObject prefab = enemiesToSpawn[i];

            if (prefab != null)
            {
                GameObject newEnemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

                // Reset AI Target to ensure they walk towards center
                EnemyAI ai = newEnemy.GetComponent<EnemyAI>();
                if (ai != null) ai.SetTarget(Vector2.zero);
            }

            if (enemySpawnInterval > 0)
            {
                yield return new WaitForSeconds(enemySpawnInterval);
            }
        }

        isSpawning = false;
    }

    // This is the brain of the spawner
    GameObject[] GetEnemyList(int wave)
    {
        // Get the current day from your Global Data script
        int currentDay = 1;
        if (DayandNightData.Instance != null)
        {
            currentDay = DayandNightData.Instance.currentDay;
        }

        // --- NIGHT 1 LOGIC ---
        if (currentDay == 1)
        {
            switch (wave)
            {
                case 1: return Night1_Wave1;
                case 2: return Night1_Wave2;
                case 3: return Night1_Wave3;
                default: return new GameObject[0];
            }
        }
        // --- NIGHT 2 LOGIC ---
        else if (currentDay == 2)
        {
            switch (wave)
            {
                case 1: return Night2_Wave1;
                case 2: return Night2_Wave2;
                case 3: return Night2_Wave3;
                case 4: return Night2_Wave4;
                default: return new GameObject[0];
            }
        }
        // --- NIGHT 3 LOGIC ---
        else if (currentDay == 3)
        {
            switch (wave)
            {
                case 1: return Night3_Wave1;
                case 2: return Night3_Wave2;
                case 3: return Night3_Wave3;
                case 4: return Night3_Wave4;
                case 5: return Night3_Wave5;
                default: return new GameObject[0];
            }
        }

        return new GameObject[0];
    }
}
