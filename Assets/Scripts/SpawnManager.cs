using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject enemyPrefabMedium;
    public GameObject powerUpPrefab;
    public GameObject powerUpVariantPrefab;

    private const float SpawnRange = 9.0f;
    private static int _enemyCount = 1;
    public int waveNumber = 1;

    public void Start()
    {
        Instantiate(powerUpPrefab, GenerateRandomSpawnPosition(), powerUpPrefab.transform.rotation);
        SpawnEnemyWave(waveNumber);
    }

    private void FixedUpdate()
    {
        _enemyCount = FindObjectsOfType<EnemyController>().Length;

        if (_enemyCount == 0)
        {
            // The number of enemies spawned increases after every wave is defeated
            waveNumber++;
            SpawnEnemyWave(waveNumber);

            var powerUpSelection = Random.Range(0, 1);

            if (powerUpSelection == 1)
            {
                // A new powerUp spawns with every wave
                Instantiate(powerUpPrefab, GenerateRandomSpawnPosition(), powerUpPrefab.transform.rotation);
            }
            else
            {
                // A new powerUpVariant spawns
                Instantiate(powerUpVariantPrefab, GenerateRandomSpawnPosition(), powerUpVariantPrefab.transform.rotation);
            }
        }
    }

    private void SpawnEnemyWave(int enemiesToSpawn)
    {
        for (var i = 0; i < enemiesToSpawn; i++)
        {
            var enemyDifficulty = Random.Range(0, 3);

            // Randomly instantiate enemy with increased difficulty after second wave
            if (waveNumber > 2 && enemyDifficulty == 2)
            {
                enemyPrefabMedium.GetComponent<EnemyController>().speed = 7f;
                Instantiate(enemyPrefabMedium, GenerateRandomSpawnPosition(), enemyPrefabMedium.transform.rotation);
            }
            else
            {
                enemyPrefab.GetComponent<EnemyController>().speed = 5f;
                Instantiate(enemyPrefab, GenerateRandomSpawnPosition(), enemyPrefab.transform.rotation);
            }
        }
    }

    private static Vector3 GenerateRandomSpawnPosition()
    {
        var spawnPosX = Random.Range(-SpawnRange, SpawnRange);
        var spawnPosY = Random.Range(-SpawnRange, SpawnRange);
        return new Vector3(spawnPosX, 0, spawnPosY);
    }
}