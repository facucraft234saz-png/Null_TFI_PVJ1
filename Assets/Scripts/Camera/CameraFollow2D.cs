using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Objetivo")]
    [SerializeField] private Transform target;

    [Header("Seguimiento")]
    [SerializeField] private float smoothSpeed = 5f;

    [Tooltip("Distancia vertical entre el jugador y la camara.")]
    [SerializeField] private float verticalOffset = 1f;

    [Header("Zoom")]
    [SerializeField] private float initialZoom = 5f;

    [Tooltip("Velocidad con la que cambia el zoom.")]
    [SerializeField] private float zoomSmoothSpeed = 2f;

    [Tooltip("Limite de alejamiento de la camara.")]
    [SerializeField] private float maxZoom = 12f;

    private Camera cam;

    private float targetZoom;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        if (cam == null)
        {
            Debug.LogError(
                "CameraFollow2D necesita una Camera."
            );

            enabled = false;
            return;
        }

        cam.orthographic = true;

        targetZoom = initialZoom;
        cam.orthographicSize = initialZoom;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        FollowPlayer();
        UpdateZoom();
    }

    // Sigue al jugador suavemente.
    private void FollowPlayer()
    {
        Vector3 targetPosition = new Vector3(
            target.position.x,
            target.position.y + verticalOffset,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }

    // Actualiza el zoom progresivamente.
    private void UpdateZoom()
    {
        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            targetZoom,
            zoomSmoothSpeed * Time.deltaTime
        );
    }

    // Permite establecer un nuevo zoom desde otro sistema.
    public void SetZoom(float newZoom)
    {
        targetZoom = Mathf.Clamp(
            newZoom,
            initialZoom,
            maxZoom
        );
    }

    // Aleja la camara una cantidad determinada.
    public void IncreaseZoom(float amount)
    {
        SetZoom(targetZoom + amount);
    }

    // Acerca la camara.
    public void DecreaseZoom(float amount)
    {
        SetZoom(targetZoom - amount);
    }

    // Devuelve la camara al zoom inicial.
    public void ResetZoom()
    {
        SetZoom(initialZoom);
    }
}