using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private const float Speed = 5.0f;
    private Rigidbody _enemyRb;
    private GameObject _player;

    // Start is called before the first frame update
    private void Start()
    {
        _enemyRb = GetComponent<Rigidbody>();
        _player = GameObject.Find("Player");
    }

    // FixedUpdate updates at a fixed rate of 50 times/sec
    // Note: Update does not work here as we don't want to update each frame
    private void FixedUpdate()
    {
        // Use normalize to force the enemy to come after the player at the same speed
        // no matter how far or close the enemy is to the player
        var lookDirection = (_player.transform.position - transform.position).normalized;
        _enemyRb.AddForce(lookDirection * Speed);

        // Destroy the enemy if they fall off the map
        if (transform.position.y < -10) Destroy(gameObject);
    }
}