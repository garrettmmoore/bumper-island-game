using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    private const float SpawnRange = 9.0f;
    private static int _enemyCount = 1;

    public GameObject[] enemyPrefabs;
    public GameObject[] powerUpPrefabs;
    private GameObject _tmpEnemy;
    public int waveNumber = 1;

    public void Start()
    {
        SpawnEnemyWave(waveNumber);
        SpawnPowerUpIndicator();
    }

    private void FixedUpdate()
    {
        _enemyCount = FindObjectsOfType<EnemyController>().Length;
        
        if (_enemyCount == 0)
        {
            // The number of enemies spawned increases after every wave is defeated
            waveNumber++;
            SpawnEnemyWave(waveNumber);
            SpawnPowerUpIndicator();
        }
    }

    private void SpawnPowerUpIndicator()
    {
        var randomPowerUp = Random.Range(0, powerUpPrefabs.Length);
        Instantiate(powerUpPrefabs[randomPowerUp], GenerateRandomSpawnPosition(), powerUpPrefabs[randomPowerUp].transform.rotation);
    }

    private void SpawnEnemy(int currentWave)
    {
        // Randomly instantiate enemy with increased difficulty after second wave
        if (currentWave > 2 && currentWave != 5 && currentWave != 10)
        {
            var randomEnemyType = (EnemyType)Random.Range(0, (int)Enum.GetValues(typeof(EnemyType)).Cast<EnemyType>().Max());
            var enemy = enemyPrefabs[(int)randomEnemyType];
            if (!enemy) return;
            
            _tmpEnemy = Instantiate(enemy, GenerateRandomSpawnPosition(), enemy.transform.rotation);
            _tmpEnemy.GetComponent<EnemyController>().speed = randomEnemyType == EnemyType.Easy ? 3.0f : 7.0f;
        }
        else
        {
            _tmpEnemy = Instantiate(enemyPrefabs[(int)EnemyType.Easy], GenerateRandomSpawnPosition(), enemyPrefabs[(int)EnemyType.Easy].transform.rotation);
            _tmpEnemy.GetComponent<EnemyController>().speed = 3.0f;
        }
    }

    private void SpawnBossWave()
    {
        _tmpEnemy = Instantiate(enemyPrefabs[(int)EnemyType.Hard], GenerateRandomSpawnPosition(), enemyPrefabs[(int)EnemyType.Hard].transform.rotation);
        _tmpEnemy.GetComponent<EnemyController>().speed = 12.0f;

    }

    private void SpawnEnemyWave(int enemiesToSpawn)
    {
        if (waveNumber != 5 && waveNumber != 10)
        {
            for (var i = 0; i < enemiesToSpawn; i++)
            {
                SpawnEnemy(waveNumber);
            }
        }
        else
        {
            SpawnBossWave();
        }
    }

    private static Vector3 GenerateRandomSpawnPosition()
    {
        var spawnPosX = Random.Range(-SpawnRange, SpawnRange);
        var spawnPosY = Random.Range(-SpawnRange, SpawnRange);
        return new Vector3(spawnPosX, 0, spawnPosY);
    }
}