using System;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [NonSerialized] private const float RotationSpeed = 70;
    public GameManager gameManager;

    // Update is called once per frame
    private void Update()
    {
        if (gameManager.isGameActive)
        {
            var horizontalInput = Input.GetAxis("Horizontal");
            transform.Rotate(Vector3.up, horizontalInput * RotationSpeed * Time.deltaTime);
        }
    }
}