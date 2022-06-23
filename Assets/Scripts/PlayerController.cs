using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player settings
    [SerializeField] private float movementSpeed = 10.0f;
    private Rigidbody _playerRb;

    // Bump attack
    public float powerUpStrength = 15.0f;

    // Smash attack
    private const float HangTime = 0.5f;
    private const float SmashSpeed = 10.0f;
    private const float ExplosionForce = 20.0f;
    private const float ExplosionRadius = 15.0f;
    private float _floorY;
    private bool _isSmashing;

    // Rocket attack
    public GameObject rocketPrefab;
    private GameObject _tmpRocket;

    // PowerUp
    public PowerUpType currentPowerUp = PowerUpType.None;
    public Transform powerUpIndicator;
    public Renderer powerUpIndicatorRenderer;
    private Coroutine _powerUpCountdown;
    private static readonly int PowerUpIndicatorColor = Shader.PropertyToID("_Color");

    // Camera
    private GameObject _focalPoint;

    // Audio
    private AudioSource _sfxAudioSource;
    [SerializeField] private AudioClip bulletSound;
    [SerializeField] private AudioClip bounceSound;
    [SerializeField] private AudioClip bumpSound;
    [SerializeField] private AudioClip powerUpSound;

    public GameManager gameManager;

    private void Start()
    {
        _sfxAudioSource = GetComponent<AudioSource>();
        _playerRb = GetComponent<Rigidbody>();
        _focalPoint = GameObject.Find("Focal Point");
    }

    private void Update()
    {
        // Use PowerUp
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (currentPowerUp)
            {
                case PowerUpType.Smash when !_isSmashing:
                    _isSmashing = true;
                    StartCoroutine(LaunchSmashAttack());
                    break;
                case PowerUpType.Rockets:
                    _sfxAudioSource.PlayOneShot(bulletSound);
                    LaunchRockets();
                    break;
            }
        }

        // Pause and resume the game
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

    private void FixedUpdate()
    {
        MovePlayer();
        UpdatePowerUpIndicatorPosition();
        CheckPlayerOutOfBounds();
    }

    private void MovePlayer()
    {
        // Move the player in the direction that our camera is pointing in
        var forwardInput = Input.GetAxis("Vertical");
        var horizontalInput = Input.GetAxis("Horizontal");

        _playerRb.AddForce(_focalPoint.transform.forward * (forwardInput * movementSpeed));
        _playerRb.AddForce(_focalPoint.transform.right * (horizontalInput * movementSpeed));
    }

    private void UpdatePowerUpIndicatorPosition()
    {
        // The powerUpIndicator follows the player's position
        powerUpIndicator.position = transform.position + new Vector3(0, -0.5f, 0);
        powerUpIndicator.Rotate(new Vector3(0, 2, 0));
    }

    private void CheckPlayerOutOfBounds()
    {
        // Destroy the player if they fall off the map
        if (gameManager.isGameActive && transform.position.y < -10)
        {
            gameObject.SetActive(false);
            gameManager.GameOver();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // When the player collides with an enemy while they have the PowerUp, the enemy goes flying
        if (collision.gameObject.CompareTag("Enemy") && currentPowerUp == PowerUpType.Pushback)
        {
            _sfxAudioSource.PlayOneShot(bumpSound);
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
            _sfxAudioSource.PlayOneShot(powerUpSound);
            currentPowerUp = powerUpType;
            switch (currentPowerUp)
            {
                case PowerUpType.Pushback:
                    powerUpIndicatorRenderer.material.SetColor(PowerUpIndicatorColor, Color.yellow);
                    break;
                case PowerUpType.Rockets:
                    powerUpIndicatorRenderer.material.SetColor(PowerUpIndicatorColor, Color.green);
                    break;
                case PowerUpType.Smash:
                    powerUpIndicatorRenderer.material.SetColor(PowerUpIndicatorColor, Color.magenta);
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
        var wait = Helpers.GetWait(7);
        yield return wait;
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
        var jumpTime = Time.time + HangTime;

        while (Time.time < jumpTime)
        {
            // move the player up while still keeping their x velocity
            _playerRb.velocity = new Vector2(_playerRb.velocity.x, SmashSpeed);
            yield return null;
        }

        // Move the player back down
        while (transform.position.y > _floorY)
        {
            _playerRb.velocity = new Vector2(_playerRb.velocity.x, -SmashSpeed * 2);
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
                         ExplosionForce,
                         transform.position,
                         ExplosionRadius,
                         0.0f,
                         ForceMode.Impulse
                     );
            }
        }

        _sfxAudioSource.PlayOneShot(bounceSound);
        // We are no longer smashing, so set the boolean to false
        _isSmashing = false;
    }
}