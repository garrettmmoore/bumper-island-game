using UnityEngine;

public class RocketBehavior : MonoBehaviour
{
    private Transform _target;

    private bool _homing;
    private const float Speed = 15.0f;
    private const float RocketStrength = 15.0f;
    private const float AliveTimer = 5.0f;

    // Takes in a Transform that we will set as the target.
    // It will set the homing boolean to true and then set the GameObject to be destroyed after 5 seconds
    public void Fire(Transform homingTarget)
    {
        _target = homingTarget;
        _homing = true;
        Destroy(gameObject, AliveTimer);
    }

    private void Update()
    {
        // Rotate and move the missiles toward the target
        if (_homing && _target)
        {
            var moveDirection = (_target.transform.position - transform.position).normalized;
            transform.position += moveDirection * (Speed * Time.deltaTime);
            transform.LookAt(_target);
        }
    }

    // Add a force to whatever is hit
    private void OnCollisionEnter(Collision collision)
    {
        // Check if we have a target
        if (!_target) return;
        
        // Compare the tag of the colliding object with the tag of the target
        if (!collision.gameObject.CompareTag(_target.tag)) return;
        
        // Get the rigidbody of the target
        var targetRigidbody = collision.gameObject.GetComponent<Rigidbody>();

        // Use the normal of the collision contact to determine which direction to push the target in
        var away = -collision.contacts[0].normal;

        // Apply the force to the target
        targetRigidbody.AddForce(away * RocketStrength, ForceMode.Impulse);

        // Destroy the missile
        Destroy(gameObject);
    }
}