using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PowerUpType currentPowerUp = PowerUpType.None;

    public float speed = 2.0f;
    public float powerUpStrength = 15.0f;
    public bool hasPowerUp;

    public GameObject powerUpIndicator;
    public GameObject rocketPrefab;

    private GameObject _focalPoint;
    private GameObject _tmpRocket;
    private Coroutine _powerUpCountdown;
    private Rigidbody _playerRb;

    private void Start()
    {
        _playerRb = GetComponent<Rigidbody>();
        _focalPoint = GameObject.Find("Focal Point");
    }

    private void Update()
    {
        if (currentPowerUp == PowerUpType.Rockets && Input.GetKeyDown(KeyCode.Space))
        {
            LaunchRockets();
            Debug.Log("Space Pressed and PowerUp Variant Clone found!");
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        // Move the player in the direction that our camera is pointing in
        var forwardInput = Input.GetAxis("Vertical");
        var horizontalInput = Input.GetAxis("Horizontal");

        _playerRb.AddForce(_focalPoint.transform.forward * (forwardInput * speed));
        _playerRb.AddForce(_focalPoint.transform.right * (horizontalInput * speed));

        // The powerUpIndicator follows the player's position
        powerUpIndicator.transform.position = transform.position + new Vector3(0, -0.5f, 0);
        powerUpIndicator.transform.Rotate(new Vector3(0, 2, 0));
    }

    // Use OnCollision if using physics
    private void OnCollisionEnter(Collision collision)
    {
        // When the player collides with an enemy while they have the PowerUp, the enemy goes flying
        if (collision.gameObject.CompareTag("Enemy") && currentPowerUp == PowerUpType.Pushback)
        {
            var enemyRb = collision.gameObject.GetComponent<Rigidbody>();
            var awayFromPlayer = collision.gameObject.transform.position - transform.position;
            enemyRb.AddForce(awayFromPlayer * powerUpStrength, ForceMode.Impulse);
            Debug.Log("Player collided with: " + collision.gameObject.name + " with PowerUp set to " + currentPowerUp);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // When the player collects a PowerUp, a visual indicator appears
        if (other.CompareTag("PowerUp") && hasPowerUp == false)
        {
            hasPowerUp = true;
            currentPowerUp = other.gameObject.GetComponent<PowerUp>().powerUpType;
            powerUpIndicator.gameObject.SetActive(true);

            // Remove the PowerUp from the view
            Destroy(other.gameObject);

            if (_powerUpCountdown != null)
            {
                StopCoroutine(_powerUpCountdown);
            }

            _powerUpCountdown = StartCoroutine(PowerUpCountDownRoutine());
        }
    }

    // After a certain amount of time, the PowerUp ability and indicator disappear
    private IEnumerator PowerUpCountDownRoutine()
    {
        yield return new WaitForSeconds(7);
        hasPowerUp = false;
        currentPowerUp = PowerUpType.None;
        powerUpIndicator.gameObject.SetActive(false);
    }

    private void LaunchRockets()
    {
        // Find all of the enemies and launch missiles at each one
        foreach (var enemy in FindObjectsOfType<EnemyController>())
        {
            // Launch the missiles from above the player to stop the collision from pushing us back
            _tmpRocket = Instantiate(rocketPrefab, transform.position + Vector3.up, Quaternion.identity);
            _tmpRocket.GetComponent<RocketBehavior>().Fire(enemy.transform);
        }
    }
}