
using UnityEngine;

[CreateAssetMenu(
    fileName = "NuevasEstadisticasCamara",
    menuName = "Crear/Juego/Cámara/Estadísticas de cámara"
)]
public class CameraStats : ScriptableObject
{
    [Header("Seguimiento de la cámara")]

    [Tooltip("Velocidad con la que la cámara sigue al jugador. Un valor mayor reduce el retraso.")]
    [Min(0f)]
    public float smoothSpeed = 5f;

    [Tooltip("Distancia vertical entre el jugador y la cámara.")]
    public float verticalOffset = 1f;

    [Header("Zoom de la cámara")]

    [Tooltip("Tamaño inicial de la cámara. Un valor menor acerca la cámara y uno mayor la aleja.")]
    [Min(0.1f)]
    public float initialZoom = 5f;

    [Tooltip("Velocidad con la que cambia el zoom.")]
    [Min(0f)]
    public float zoomSmoothSpeed = 2f;

    [Tooltip("Límite mínimo del zoom. Evita acercar demasiado la cámara.")]
    [Min(0.1f)]
    public float minZoom = 3f;

    [Tooltip("Límite máximo del zoom. Evita alejar demasiado la cámara.")]
    [Min(0.1f)]
    public float maxZoom = 12f;
}
