
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

    [Tooltip("Segundos que tarda en alcanzar la velocidad maxima.")]
    [Min(0.01f)]
    public float accelerationTime = 1f;

    [Tooltip("Segundos que tarda en frenar hasta detenerse.")]
    [Min(0.01f)]
    public float decelerationTime = 0.5f;

    [Header("Salto")]

    [Tooltip("Fuerza inicial del salto.")]
    public float jumpForce = 12f;

    [Tooltip("Cantidad total de saltos antes de tocar el suelo.")]
    [Min(1)]
    public int maxJumps = 1;

    [Header("Gravedad")]

    public float gravityScale = 3f;

    [Header("Deteccion del suelo")]

    public float groundCheckRadius = 0.18f;
}
