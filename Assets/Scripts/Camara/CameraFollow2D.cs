
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Camera))]
public class CameraFollow2D : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField] private CameraStats cameraStats;

    [Header("Objetivo")]
    [SerializeField] private Transform target;

    private Camera cam;

    private float targetZoom;

    // Modificadores aplicados por los artículos.
    private float zoomMultiplier = 1f;
    private float followLagReductionPercent = 0f;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;

        if (cameraStats == null)
        {
            Debug.LogError(
                "CameraFollow2D: falta asignar CameraStats.",
                this
            );

            enabled = false;
            return;
        }

        UpdateTargetZoom();
        cam.orthographicSize = targetZoom;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        FindPlayerTarget();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        target = null;
        FindPlayerTarget();
    }

    private void FindPlayerTarget()
    {
        if (target != null)
            return;

        PlayerController2D player =
            FindFirstObjectByType<PlayerController2D>();

        if (player != null)
        {
            target = player.transform;

            Debug.Log(
                "CameraFollow2D: jugador encontrado correctamente."
            );
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            FindPlayerTarget();

            if (target == null)
                return;
        }

        FollowPlayer();
        UpdateZoom();
    }

    // ------------------------------------------------
    // SEGUIMIENTO
    // ------------------------------------------------

    private void FollowPlayer()
    {
        Vector3 targetPosition = new Vector3(
            target.position.x,
            target.position.y + cameraStats.verticalOffset,
            transform.position.z
        );

        // Con 100% de reducción, el seguimiento es instantáneo.
        if (followLagReductionPercent >= 100f)
        {
            transform.position = targetPosition;
            return;
        }

        float reduction = followLagReductionPercent / 100f;

        // Cuanto mayor sea la reducción, más rápido sigue la cámara.
        float effectiveSpeed =
            cameraStats.smoothSpeed / Mathf.Max(0.01f, 1f - reduction);

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            effectiveSpeed * Time.deltaTime
        );
    }

    // ------------------------------------------------
    // ZOOM
    // ------------------------------------------------

    private void UpdateZoom()
    {
        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            targetZoom,
            cameraStats.zoomSmoothSpeed * Time.deltaTime
        );
    }

    private void UpdateTargetZoom()
    {
        float desiredZoom =
            cameraStats.initialZoom * zoomMultiplier;

        targetZoom = Mathf.Clamp(
            desiredZoom,
            cameraStats.minZoom,
            cameraStats.maxZoom
        );
    }

    // Establece un zoom específico.
    public void SetZoom(float newZoom)
    {
        targetZoom = Mathf.Clamp(
            newZoom,
            cameraStats.minZoom,
            cameraStats.maxZoom
        );
    }

    public void IncreaseZoom(float amount)
    {
        SetZoom(targetZoom + amount);
    }

    public void DecreaseZoom(float amount)
    {
        SetZoom(targetZoom - amount);
    }

    public void ResetZoom()
    {
        zoomMultiplier = 1f;
        UpdateTargetZoom();
    }

    // ------------------------------------------------
    // MEJORAS DE ARTICULOS
    // ------------------------------------------------

    public void SetZoomMultiplier(float multiplier)
    {
        zoomMultiplier = Mathf.Max(0.01f, multiplier);
        UpdateTargetZoom();
    }

    public void SetFollowLagReduction(float percentage)
    {
        followLagReductionPercent = Mathf.Clamp(
            percentage,
            0f,
            100f
        );
    }

    public float GetCurrentTargetZoom()
    {
        return targetZoom;
    }

    public float GetFollowLagReduction()
    {
        return followLagReductionPercent;
    }
}
