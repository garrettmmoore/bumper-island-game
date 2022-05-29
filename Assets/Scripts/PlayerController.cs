using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PowerUpType currentPowerUp = PowerUpType.None;

    public float speed = 2.0f;
    public float powerUpStrength = 15.0f;

    // Smash
    private float hangTime = 0.5f;
    private float smashSpeed = 10.0f;
    private float explosionForce = 20.0f;
    private float explosionRadius = 12.0f;
    private float _floorY;
    private bool _isSmashing;

    // PowerUp
    public GameObject powerUpIndicator;
    private Coroutine _powerUpCountdown;

    // Rockets
    public GameObject rocketPrefab;
    private GameObject _tmpRocket;

    private GameObject _focalPoint;
    private Rigidbody _playerRb;

    private static readonly int PowerUpRingColor = Shader.PropertyToID("_Color");
    public GameManager gameManager;


    private void Start()
    {
        _playerRb = GetComponent<Rigidbody>();
        _focalPoint = GameObject.Find("Focal Point");
    }

    private void Update()
    {
        // Use powerup
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (currentPowerUp)
            {
                case PowerUpType.Smash when !_isSmashing:
                    _isSmashing = true;
                    StartCoroutine(LaunchSmashAttack());
                    break;
                case PowerUpType.Rockets:
                    LaunchRockets();
                    break;
            }
        }

        // Pause and Resume the game
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (gameManager.pauseText.IsActive())
            {
                gameManager.ResumeGame();
            }
            else
            {
                gameManager.PauseGame();
            }
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

        // Destroy the player if they fall off the map
        if (gameManager.isGameActive && transform.position.y < -10)
        {
            gameObject.SetActive(false);
            gameManager.GameOver();
        }
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
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var powerUpType = other.gameObject.GetComponent<PowerUp>().powerUpType;

        // When the player collects a PowerUp, a visual indicator appears
        if (other.CompareTag("PowerUp"))
        {
            currentPowerUp = powerUpType;
            switch (currentPowerUp)
            {
                case PowerUpType.Pushback:
                    powerUpIndicator.GetComponent<Renderer>()
                                    .material.SetColor(PowerUpRingColor, Color.yellow);
                    break;
                case PowerUpType.Rockets:
                    powerUpIndicator.GetComponent<Renderer>()
                                    .material.SetColor(PowerUpRingColor, Color.green);
                    break;
                case PowerUpType.Smash:
                    powerUpIndicator.GetComponent<Renderer>()
                                    .material.SetColor(PowerUpRingColor, Color.magenta);
                    break;
            }

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
        currentPowerUp = PowerUpType.None;
        powerUpIndicator.gameObject.SetActive(false);
    }

    private void LaunchRockets()
    {
        // Find all of the enemies and launch missiles at each one
        foreach (var enemy in FindObjectsOfType<EnemyController>())
        {
            rocketPrefab = ObjectPool.sharedInstance.GetPooledObject();
            if (rocketPrefab != null)
            {
                // Launch the missiles from above the player to stop the collision from pushing us back
                rocketPrefab.transform.position = transform.position + Vector3.up;
                rocketPrefab.transform.rotation = Quaternion.identity;
                rocketPrefab.GetComponent<RocketBehavior>().Fire(enemy.transform);
                rocketPrefab.SetActive(true);
            }
        }
    }

    private IEnumerator LaunchSmashAttack()
    {
        // Find all of the enemies
        var enemies = FindObjectsOfType<EnemyController>();

        // Store the y position before taking off
        _floorY = transform.position.y;

        // Calculate the amount of time we will go up
        var jumpTime = Time.time + hangTime;

        while (Time.time < jumpTime)
        {
            // move the player up while still keeping their x velocity
            _playerRb.velocity = new Vector2(_playerRb.velocity.x, smashSpeed);
            yield return null;
        }

        // Move the player back down
        while (transform.position.y > _floorY)
        {
            _playerRb.velocity = new Vector2(_playerRb.velocity.x, -smashSpeed * 2);
            yield return null;
        }

        // Find all of the enemies
        foreach (var enemy in enemies)
        {
            // Apply an explosion force that originates from our position
            if (enemy)
            {
                enemy.GetComponent<Rigidbody>()
                     .AddExplosionForce(
                         explosionForce,
                         transform.position,
                         explosionRadius,
                         0.0f,
                         ForceMode.Impulse
                     );
            }
        }

        // We are no longer smashing, so set the boolean to false
        _isSmashing = false;
    }
}