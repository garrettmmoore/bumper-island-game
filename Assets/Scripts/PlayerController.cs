using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _playerRb;
    private GameObject _focalPoint;
    public GameObject powerUpIndicator;

    public float speed = 5.0f;
    public bool hasPowerUp;
    public float powerUpStrength = 15.0f;


    // Start is called before the first frame update
    private void Start()
    {
        _playerRb = GetComponent<Rigidbody>();
        _focalPoint = GameObject.Find("Focal Point");
    }

    private void OnTriggerEnter(Collider other)
    {
        // When the player collects a powerup, a visual indicator appears
        if (other.CompareTag("PowerUp"))
        {
            hasPowerUp = true;
            powerUpIndicator.gameObject.SetActive(true);

            // Remove the PowerUp from the view
            Destroy(other.gameObject);
            StartCoroutine(PowerUpCountDownRoutine());
        }
    }

    // After a certain amount of time, the PowerUp ability and indicator disappear
    private IEnumerator PowerUpCountDownRoutine()
    {
        yield return new WaitForSeconds(6);
        hasPowerUp = false;
        powerUpIndicator.gameObject.SetActive(false);
    }

    // Use OnCollision if using physics
    private void OnCollisionEnter(Collision collision)
    {
        // When the player collides with an enemy while they have the PowerUp, the enemy goes flying
        if (collision.gameObject.CompareTag("Enemy") && hasPowerUp)
        {
            var enemyRb = collision.gameObject.GetComponent<Rigidbody>();
            var awayFromPlayer = collision.gameObject.transform.position - transform.position;
            enemyRb.AddForce(awayFromPlayer * powerUpStrength, ForceMode.Impulse);
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
}
