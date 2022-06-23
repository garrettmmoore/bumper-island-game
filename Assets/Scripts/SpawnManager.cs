using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public GameObject[] powerUpPrefabs;
    private GameObject _tmpEnemy;
    [SerializeField] private GameManager gameManager;

    private const int Easy = (int)EnemyType.Easy;
    private const int Medium = (int)EnemyType.Medium;
    private const int Hard = (int)EnemyType.Hard;

    private const float SpawnRange = 9.0f;
    private readonly int _enemyTypeMaxRange = (int)Enum.GetValues(typeof(EnemyType)).Cast<EnemyType>().Max();
    private int _powerUpGemsMaxRange;

    private int _waveNumber = 1;
    private bool _enemiesExist;

    public void Start()
    {
        _powerUpGemsMaxRange = powerUpPrefabs.Length;
        SpawnEnemyWave(_waveNumber);
        SpawnPowerUpGem();
    }

    private void FixedUpdate()
    {
        _enemiesExist = FindObjectOfType(typeof(EnemyController)) != null;
        if (_enemiesExist) return;

        // The number of enemies spawned increases after every wave is defeated
        _waveNumber++;
        gameManager.UpdateLevel(_waveNumber);

        SpawnEnemyWave(_waveNumber);
        SpawnPowerUpGem();
    }

    private void SpawnEnemyWave(int enemiesToSpawn)
    {
        var isBossRound = _waveNumber % 5 == 0;
        if (isBossRound)
        {
            SpawnEnemy(_waveNumber);
        }
        else
        {
            for (var i = 0; i < enemiesToSpawn; i++)
            {
                SpawnEnemy(_waveNumber);
            }
        }
    }

    private void SpawnPowerUpGem()
    {
        var randomPowerUp = Random.Range(0, _powerUpGemsMaxRange);
        Instantiate(
            powerUpPrefabs[randomPowerUp],
            GenerateRandomSpawnPosition(),
            powerUpPrefabs[randomPowerUp].transform.rotation
        );
    }

    private void SpawnEnemy(int currentWave)
    {
        var isBossRound = currentWave % 5 == 0;

        if (currentWave < 2)
        {
            CreateEnemy(Easy, 3.0f);
        }
        else if (isBossRound)
        {
            CreateEnemy(Hard, 12.0f);
        }
        else
        {
            // Randomly instantiate enemy with increased difficulty after second wave
            var randomEnemyType = (EnemyType)Random.Range(0, _enemyTypeMaxRange);
            if (randomEnemyType == EnemyType.Easy)
            {
                CreateEnemy(Easy, 3.0f);
            }
            else
            {
                CreateEnemy(Medium, 7.0f);
            }
        }
    }

    private void CreateEnemy(int enemyDifficultyIndex, float speed)
    {
        _tmpEnemy = Instantiate(
            enemyPrefabs[enemyDifficultyIndex],
            GenerateRandomSpawnPosition(),
            enemyPrefabs[enemyDifficultyIndex].transform.rotation
        );
        _tmpEnemy.GetComponent<EnemyController>().speed = speed;
    }

    private static Vector3 GenerateRandomSpawnPosition()
    {
        var spawnPosX = Random.Range(-SpawnRange, SpawnRange);
        var spawnPosY = Random.Range(-SpawnRange, SpawnRange);
        return new Vector3(spawnPosX, 0, spawnPosY);
    }
}