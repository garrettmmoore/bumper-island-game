using UnityEngine;


public class MoveForward : MonoBehaviour
{
    private float speed = 50f;
    private Transform yourTarget;
    Vector3 directionToTarget;
    float maxTurnSpeed = 60f; // degrees per second
    // Update is called once per frame

    // private void Start()
    // {
    //     yourTarget = GameObject.FindGameObjectWithTag("Enemy").transform;
    // }
    // public void Update()
    // {
    //     directionToTarget = yourTarget.position - transform.position;
    //     var currentDirection = transform.forward;
    //     var resultingDirection = Vector3.RotateTowards(currentDirection, directionToTarget, maxTurnSpeed * Mathf.Deg2Rad * Time.deltaTime, 1f);
    //     transform.rotation = Quaternion.LookRotation(resultingDirection);
    //
    //     // transform.Translate(Vector3.forward * speed * Time.deltaTime);
    //     // transform.Translate(Vector3.forward * Time.deltaTime * speed);
    //     // transform.position += transform.forward * Time.deltaTime * speed;
    // }
}
