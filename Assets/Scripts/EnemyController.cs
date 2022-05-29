using UnityEngine;

public enum EnemyType
{
    Easy,
    Medium,
    Hard
}

public class EnemyController : MonoBehaviour
{
    public float speed;
    private Rigidbody _enemyRb;
    private GameObject _player;
    private GameManager _gameManagerObj;
    public EnemyType enemyType; 


    private void Start()
    {
        _player = GameObject.Find("Player");
        _enemyRb = GetComponent<Rigidbody>();
        _gameManagerObj = FindObjectOfType<GameManager>();
    }

    // FixedUpdate updates at a fixed rate of 50 times/sec
    // Note: Update does not work here as we don't want to update each frame
    private void FixedUpdate()
    {
        if (_player.activeSelf == false) return;

        // Use normalize to force the enemy to come after the player at the same speed
        // no matter how far or close the enemy is to the player
        var lookDirection = (_player.transform.position - transform.position).normalized;
        _enemyRb.AddForce(lookDirection * speed);

        // Destroy the enemy if they fall off the map
        if (transform.position.y < -10)
        {
            _gameManagerObj.UpdateScore(enemyType);
            Destroy(gameObject);
        }
    }
}