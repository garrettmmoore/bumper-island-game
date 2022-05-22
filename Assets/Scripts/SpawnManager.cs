using UnityEngine;

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
            var randomEnemyType = Random.Range(0, 2);
            _tmpEnemy = Instantiate(enemyPrefabs[randomEnemyType], GenerateRandomSpawnPosition(), enemyPrefabs[randomEnemyType].transform.rotation);
            _tmpEnemy.GetComponent<EnemyController>().speed = randomEnemyType == 0 ? 3.0f : 7.0f;
        }
        else
        {
            _tmpEnemy = Instantiate(enemyPrefabs[0], GenerateRandomSpawnPosition(), enemyPrefabs[0].transform.rotation);
            _tmpEnemy.GetComponent<EnemyController>().speed = 3.0f;
        }
    }

    private void SpawnBossWave()
    {
        _tmpEnemy = Instantiate(enemyPrefabs[2], GenerateRandomSpawnPosition(), enemyPrefabs[2].transform.rotation);
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