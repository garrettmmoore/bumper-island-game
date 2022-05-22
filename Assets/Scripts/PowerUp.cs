using UnityEngine;

// Enum to allow for more power-ups to be added
public enum PowerUpType
{
    None,
    Pushback,
    Rockets,
    Smash
}

public class PowerUp : MonoBehaviour
{
    public PowerUpType powerUpType;
}