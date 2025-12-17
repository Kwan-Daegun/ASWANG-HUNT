using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuration")]
    // Since this script is on a specific object (Left OR Right), 
    // we don't need a reference to the "other" side. 
    // Just spawn at THIS object's position.
    [SerializeField] private Transform spawnPoint;

    [Header("My Wave Lists")]
    // PUT ONLY THE ENEMIES FOR THIS SPECIFIC SIDE HERE
    [SerializeField] private GameObject[] wave1Enemies;
    [SerializeField] private GameObject[] wave2Enemies;
    [SerializeField] private GameObject[] wave3Enemies;
    [SerializeField] private GameObject[] wave4Enemies;
    [SerializeField] private GameObject[] wave5Enemies;

    [Header("Timing")]
    [Tooltip("Set to 0 to spawn all instantly. Set to 0.2 for fast sequence.")]
    [SerializeField] private float enemySpawnInterval = 0.5f;

    // Internal State
    public bool isSpawning = false;

    // We don't need Update() anymore. 
    // The GameManager will check if enemies are dead.

    public void StartWave(int waveIndex)
    {
        if (isSpawning) return;
        StartCoroutine(SpawnRoutine(waveIndex));
    }

    IEnumerator SpawnRoutine(int waveIndex)
    {
        isSpawning = true;

        GameObject[] enemiesToSpawn = GetEnemyList(waveIndex);

        // If this side has no enemies for this wave, just finish immediately
        if (enemiesToSpawn.Length == 0)
        {
            isSpawning = false;
            yield break;
        }

        for (int i = 0; i < enemiesToSpawn.Length; i++)
        {
            GameObject prefab = enemiesToSpawn[i];

            if (prefab != null)
            {
                // Spawn at the Transform assigned in Inspector (Left or Right)
                GameObject newEnemy = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

                // Reset AI Target
                EnemyAI ai = newEnemy.GetComponent<EnemyAI>();
                if (ai != null) ai.SetTarget(Vector2.zero);
            }

            // Simultaneous Logic:
            if (enemySpawnInterval > 0)
            {
                yield return new WaitForSeconds(enemySpawnInterval);
            }
        }

        isSpawning = false;
    }

    GameObject[] GetEnemyList(int wave)
    {
        switch (wave)
        {
            case 1: return wave1Enemies;
            case 2: return wave2Enemies;
            case 3: return wave3Enemies;
            case 4: return wave4Enemies;
            case 5: return wave5Enemies;
            default: return new GameObject[0];
        }
    }
}
