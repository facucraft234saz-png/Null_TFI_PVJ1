
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLevelGenerator2D : MonoBehaviour
{
    [Header("PLATAFORMA INICIAL")]
    [Tooltip("Plataforma donde comienza el jugador. No será eliminada.")]
    [SerializeField] private Transform initialPlatform;

    [Tooltip("Distancia desde la superficie hasta los pies del jugador.")]
    [Min(0f)]
    [SerializeField] private float playerSpawnHeightOffset = 0.5f;

    [Header("PLATAFORMAS DISPONIBLES")]
    [Tooltip("Prefabs que se elegirán aleatoriamente.")]
    [SerializeField]
    private List<GameObject> platformPrefabs = new List<GameObject>();

    [Header("CANTIDAD DE PLATAFORMAS")]
    [Tooltip("Cantidad de plataformas del nivel 1.")]
    [Min(1)]
    [SerializeField] private int initialPlatformCount = 1;

    [Tooltip("Cantidad adicional de plataformas por cada nivel superado.")]
    [Min(0)]
    [SerializeField] private int additionalPlatformsPerLevel = 1;

    [Header("SEPARACIÓN ENTRE PLATAFORMAS")]
    [Min(0f)]
    [SerializeField] private float minimumPlatformGap = 1f;

    [Min(0f)]
    [SerializeField] private float maximumPlatformGap = 3f;

    [Header("ALTURA ALEATORIA")]
    [Min(0f)]
    [SerializeField] private float maximumVerticalDifference = 1.5f;

    [Min(0f)]
    [SerializeField] private float maximumHeightFromStart = 3f;

    [Header("ANCHO DE PLATAFORMAS")]
    [Min(0.1f)]
    [SerializeField] private float defaultPlatformWidth = 4f;

    [Header("DIRECCIÓN ALEATORIA")]
    [Tooltip("Si está activado, cada plataforma puede aparecer a izquierda o derecha.")]
    [SerializeField] private bool generateOnBothSides = true;

    [Header("PUERTA DE SALIDA")]
    [Tooltip("Punto manual donde aparecerá la puerta.")]
    [SerializeField] private Transform doorSpawnPoint;

    [Tooltip("Altura adicional si no se asigna un punto manual.")]
    [SerializeField] private float doorVerticalOffset = 1f;

    private readonly List<GameObject> generatedPlatforms =
        new List<GameObject>();

    private Vector3 exitDoorPosition;
    private Quaternion exitDoorRotation = Quaternion.identity;

    public Vector3 ExitDoorPosition => exitDoorPosition;
    public Quaternion ExitDoorRotation => exitDoorRotation;
    public int GeneratedPlatformCount => generatedPlatforms.Count;

    // ------------------------------------------------
    // POSICIÓN INICIAL DEL JUGADOR
    // ------------------------------------------------

    public Vector3 PlayerSpawnPosition
    {
        get
        {
            if (initialPlatform == null)
                return Vector3.zero;

            Collider2D platformCollider =
                initialPlatform.GetComponent<Collider2D>();

            float surfaceY = platformCollider != null
                ? platformCollider.bounds.max.y
                : initialPlatform.position.y;

            return new Vector3(
                initialPlatform.position.x,
                surfaceY + playerSpawnHeightOffset,
                initialPlatform.position.z
            );
        }
    }

    // ------------------------------------------------
    // GENERAR NIVEL
    // ------------------------------------------------

    public bool GenerateLevel(int level)
    {
        if (initialPlatform == null)
        {
            Debug.LogError(
                "Falta asignar la plataforma inicial.",
                this
            );
            return false;
        }

        if (platformPrefabs == null || platformPrefabs.Count == 0)
        {
            Debug.LogError(
                "No hay prefabs de plataformas asignados.",
                this
            );
            return false;
        }

        ClearGeneratedPlatforms();

        level = Mathf.Max(1, level);

        int platformCount =
            initialPlatformCount +
            (level - 1) * additionalPlatformsPerLevel;

        Vector3 lastLeftPosition = initialPlatform.position;
        Vector3 lastRightPosition = initialPlatform.position;

        float initialWidth =
            GetPlatformWidth(initialPlatform.gameObject);

        float lastLeftWidth = initialWidth;
        float lastRightWidth = initialWidth;

        float initialY = initialPlatform.position.y;

        int leftCount = 0;
        int rightCount = 0;

        Debug.Log(
            "GENERANDO NIVEL " + level +
            "\nPlataformas: " + platformCount
        );

        // --------------------------------------------
        // REGALOS DE LA PLATAFORMA INICIAL
        // --------------------------------------------

        GeneratePlatformGifts(initialPlatform.gameObject);

        // --------------------------------------------
        // GENERAR PLATAFORMAS
        // --------------------------------------------

        for (int i = 0; i < platformCount; i++)
        {
            GameObject selectedPrefab =
                platformPrefabs[
                    Random.Range(0, platformPrefabs.Count)
                ];

            if (selectedPrefab == null)
                continue;

            int direction;

            if (generateOnBothSides)
            {
                direction = Random.value < 0.5f ? -1 : 1;
            }
            else
            {
                direction = 1;
            }

            Vector3 previousPosition;
            float previousWidth;

            if (direction == -1)
            {
                previousPosition = lastLeftPosition;
                previousWidth = lastLeftWidth;
            }
            else
            {
                previousPosition = lastRightPosition;
                previousWidth = lastRightWidth;
            }

            float nextWidth = GetPlatformWidth(selectedPrefab);

            float gap = Random.Range(
                minimumPlatformGap,
                Mathf.Max(minimumPlatformGap, maximumPlatformGap)
            );

            float horizontalDistance =
                previousWidth * 0.5f +
                gap +
                nextWidth * 0.5f;

            float verticalChange = Random.Range(
                -maximumVerticalDifference,
                maximumVerticalDifference
            );

            float nextY = Mathf.Clamp(
                previousPosition.y + verticalChange,
                initialY - maximumHeightFromStart,
                initialY + maximumHeightFromStart
            );

            Vector3 nextPosition = new Vector3(
                previousPosition.x + direction * horizontalDistance,
                nextY,
                initialPlatform.position.z
            );

            GameObject platform = Instantiate(
                selectedPrefab,
                nextPosition,
                Quaternion.identity,
                transform
            );

            platform.name =
                "Plataforma_Generada_" + (i + 1) +
                (direction == -1 ? "_Izquierda" : "_Derecha");

            generatedPlatforms.Add(platform);

            // ----------------------------------------
            // GENERAR REGALOS DE ESTA PLATAFORMA
            // ----------------------------------------

            GeneratePlatformGifts(platform);

            // ----------------------------------------
            // ACTUALIZAR EL LADO CORRESPONDIENTE
            // ----------------------------------------

            if (direction == -1)
            {
                lastLeftPosition = nextPosition;
                lastLeftWidth = GetPlatformWidth(platform);
                leftCount++;
            }
            else
            {
                lastRightPosition = nextPosition;
                lastRightWidth = GetPlatformWidth(platform);
                rightCount++;
            }
        }

        // --------------------------------------------
        // UBICAR PUERTA
        // --------------------------------------------

        CalculateExitDoorPosition();

        Debug.Log(
            "NIVEL GENERADO" +
            "\nTotal de plataformas: " + generatedPlatforms.Count +
            "\nIzquierda: " + leftCount +
            "\nDerecha: " + rightCount
        );

        return true;
    }

    // ------------------------------------------------
    // GENERAR REGALOS DE UNA PLATAFORMA
    // ------------------------------------------------

    private void GeneratePlatformGifts(GameObject platform)
    {
        if (platform == null)
            return;

        PlatformGiftGenerator2D giftGenerator =
            platform.GetComponent<PlatformGiftGenerator2D>();

        if (giftGenerator != null)
        {
            giftGenerator.GenerateGifts();
        }
    }

    // ------------------------------------------------
    // CALCULAR POSICIÓN DE LA PUERTA
    // ------------------------------------------------

    private void CalculateExitDoorPosition()
    {
        if (doorSpawnPoint != null)
        {
            exitDoorPosition = doorSpawnPoint.position;
            exitDoorRotation = doorSpawnPoint.rotation;
            return;
        }

        Collider2D platformCollider =
            initialPlatform.GetComponent<Collider2D>();

        float surfaceY = platformCollider != null
            ? platformCollider.bounds.max.y
            : initialPlatform.position.y;

        exitDoorPosition = new Vector3(
            initialPlatform.position.x,
            surfaceY + doorVerticalOffset,
            initialPlatform.position.z
        );

        exitDoorRotation = Quaternion.identity;
    }

    // ------------------------------------------------
    // CALCULAR ANCHO DE PLATAFORMAS
    // ------------------------------------------------

    private float GetPlatformWidth(GameObject platform)
    {
        if (platform == null)
            return defaultPlatformWidth;

        Collider2D collider =
            platform.GetComponent<Collider2D>();

        if (collider != null && collider.bounds.size.x > 0.01f)
            return collider.bounds.size.x;

        Renderer platformRenderer =
            platform.GetComponent<Renderer>();

        if (platformRenderer != null &&
            platformRenderer.bounds.size.x > 0.01f)
        {
            return platformRenderer.bounds.size.x;
        }

        return defaultPlatformWidth;
    }

    // ------------------------------------------------
    // ELIMINAR PLATAFORMAS GENERADAS
    // ------------------------------------------------

    public void ClearGeneratedPlatforms()
    {
        foreach (GameObject platform in generatedPlatforms)
        {
            if (platform != null)
                Destroy(platform);
        }

        generatedPlatforms.Clear();
    }
}
