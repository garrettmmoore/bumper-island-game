using System;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [NonSerialized] private const float RotationSpeed = 70;

    // Update is called once per frame
    private void Update()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.up, horizontalInput * RotationSpeed * Time.deltaTime);
    }
}