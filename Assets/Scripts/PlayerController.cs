using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Player settings
    private Rigidbody _playerRb;
    [SerializeField] private float movementSpeed = 10.0f;

    // Smash attack
    private const float HangTime = 0.5f;
    private const float SmashSpeed = 10.0f;
    private const float ExplosionForce = 20.0f;
    private const float ExplosionRadius = 30.0f;
    private float _floorY;
    private bool _isSmashing;
    private bool _isJumping;

    // Rocket attack
    public GameObject rocketPrefab;
    private GameObject _tmpRocket;

    // Bump attack
    public float powerUpStrength = 15.0f;

    // PowerUp
    public PowerUpType currentPowerUp = PowerUpType.None;
    private Coroutine _powerUpCountdown;

    // PowerUp Indicator
    private Transform _powerUpIndicator;
    private Renderer _powerUpIndicatorRenderer;

    private readonly int _powerUpIndicatorColor = Shader.PropertyToID("_Color");

    // Movement
    private Vector2 m_move;

    // Camera
    private Transform _focalPoint;
    private PlayerInput _playerInput;

    // Audio
    private static AudioSource _sfxAudioSource;
    [SerializeField] private AudioClip bulletSound;
    [SerializeField] private AudioClip bounceSound;
    [SerializeField] private AudioClip bumpSound;
    [SerializeField] private AudioClip powerUpSound;
    private GameManager _gameManager;

    private void Awake()
    {
        _sfxAudioSource = gameObject.AddComponent<AudioSource>();
        _playerRb = GetComponent<Rigidbody>();
        _powerUpIndicator = Instantiate(Resources.Load("Prefabs/SelectionRing", typeof(Transform))) as Transform;
        if (_powerUpIndicator != null)
        {
            _powerUpIndicator.gameObject.SetActive(false);
            _powerUpIndicatorRenderer = _powerUpIndicator.gameObject.GetComponent<Renderer>();
        }
        
        _focalPoint = GameObject.FindGameObjectWithTag("PlayerFocalPoint").transform;
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (transform.position.y <= .25)
        {
            _isJumping = false;
        }

        if (!_isSmashing && !_isJumping && context.performed)
        {
            _playerRb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            _isJumping = true;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
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
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!_gameManager.pauseText.IsActive())
            {
                _gameManager.PauseGame();
                // Select topmost button.
                EventSystem.current.SetSelectedGameObject(_gameManager.resumeButton.gameObject);
            }
            else if (_gameManager.pauseText.IsActive())
            {
                _gameManager.ResumeGame();
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // Move the player in the direction that our camera is pointing in
        m_move = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        MovePlayer(m_move);
        UpdatePowerUpIndicatorPosition();
        CheckPlayerOutOfBounds();
    }

    private void MovePlayer(Vector2 direction)
    {
        _playerRb.AddForce(_focalPoint.forward * (direction.y * movementSpeed));
        _playerRb.AddForce(_focalPoint.right * (direction.x * movementSpeed));
    }

    private void UpdatePowerUpIndicatorPosition()
    {
        // The powerUpIndicator follows the player's position
        _powerUpIndicator.position = transform.position + new Vector3(0, -0.5f, 0);
        _powerUpIndicator.Rotate(new Vector3(0, 2, 0));
    }

    private void CheckPlayerOutOfBounds()
    {
        // Destroy the player if they fall off the map
        if (_gameManager.isGameActive && transform.position.y < -10)
        {
            gameObject.SetActive(false);
            _gameManager.GameOver();
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
                    _powerUpIndicatorRenderer.sharedMaterial.SetColor(_powerUpIndicatorColor, Color.yellow);
                    break;
                case PowerUpType.Rockets:
                    _powerUpIndicatorRenderer.sharedMaterial.SetColor(_powerUpIndicatorColor, Color.green);
                    break;
                case PowerUpType.Smash:
                    _powerUpIndicatorRenderer.sharedMaterial.SetColor(_powerUpIndicatorColor, Color.magenta);
                    break;
            }

            _powerUpIndicator.gameObject.SetActive(true);

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
        _powerUpIndicator.gameObject.SetActive(false);
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