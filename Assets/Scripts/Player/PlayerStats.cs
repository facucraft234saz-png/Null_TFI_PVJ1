

using UnityEngine;

[CreateAssetMenu(
    fileName = "PlayerStats",
    menuName = "Game/Player/Player Stats"
)]
public class PlayerStats : ScriptableObject
{
    [Header("Movimiento horizontal")]
    [Tooltip("Velocidad horizontal maxima.")]
    public float maxSpeed = 6f;

    [Tooltip("Segundos para alcanzar la velocidad maxima. 0 = instantaneo.")]
    [Min(0f)]
    public float accelerationTime = 1f;

    [Tooltip("Segundos para frenar. 0 = frenado instantaneo.")]
    [Min(0f)]
    public float decelerationTime = 0.5f;

    [Header("Salto")]
    public float jumpForce = 12f;

    [Min(1)]
    public int maxJumps = 1;

    [Header("Gravedad")]
    public float gravityScale = 3f;

    [Header("Deteccion del suelo")]
    public float groundCheckRadius = 0.18f;

    [Header("Vida y defensa")]
    [Min(1)]
    public int startingHearts = 3;

    [Min(0)]
    public int startingShields = 0;

    [Tooltip("Segundos de proteccion despues de recibir un golpe.")]
    [Min(0f)]
    public float invulnerabilityDuration = 0.75f;
}

