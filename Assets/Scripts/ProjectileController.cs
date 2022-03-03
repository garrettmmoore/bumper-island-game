using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float powerUpStrength = 15.0f;

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
        Destroy(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // When the projectile collides with an enemy, the enemy goes flying
        if (collision.gameObject.CompareTag("Enemy"))
        {
            var enemyRb = collision.gameObject.GetComponent<Rigidbody>();
            var awayFromPlayer = collision.gameObject.transform.position - transform.position;
            enemyRb.AddForce(awayFromPlayer * powerUpStrength, ForceMode.Impulse);
            Destroy(gameObject, 1);
        }
    }

    private void FixedUpdate()
    {
        // Destroy the projectile if it falls off the map
        if (transform.position.y < -10) Destroy(gameObject);
    }
}
