
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlatformGiftGenerator2D : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject blackGiftPrefab;

    [Header("Distribucion")]
    [Min(1)]
    [SerializeField] private int giftCount = 5;

    [Min(0f)]
    [SerializeField] private float sideMargin = 0.5f;

    [Min(0f)]
    [SerializeField] private float verticalOffset = 0.5f;

    [Header("Generacion")]
    [SerializeField] private bool generateOnStart = true;

    private BoxCollider2D platformCollider;
    private Transform generatedGiftsParent;

    private void Awake()
    {
        platformCollider = GetComponent<BoxCollider2D>();
        platformCollider.isTrigger = false;
    }

    private void Start()
    {
        if (generateOnStart)
            GenerateGifts();
    }

    [ContextMenu("Generar regalos")]
    public void GenerateGifts()
    {
        if (blackGiftPrefab == null)
        {
            Debug.LogError(
                "Falta asignar el prefab del regalo negro.",
                this
            );
            return;
        }

        ClearGeneratedGifts();

        Bounds bounds = platformCollider.bounds;

        float availableWidth =
            bounds.size.x - sideMargin * 2f;

        if (availableWidth < 0f)
        {
            Debug.LogWarning(
                "Los margenes son demasiado grandes para esta plataforma.",
                this
            );
            return;
        }

        generatedGiftsParent = new GameObject("GeneratedGifts").transform;
        generatedGiftsParent.SetParent(transform);
        generatedGiftsParent.localPosition = Vector3.zero;

        for (int i = 0; i < giftCount; i++)
        {
            float x;

            if (giftCount == 1)
            {
                x = bounds.center.x;
            }
            else
            {
                float progress = (float)i / (giftCount - 1);

                x = bounds.min.x + sideMargin +
                    availableWidth * progress;
            }

            float y = bounds.max.y + verticalOffset;

            Vector3 spawnPosition = new Vector3(
                x,
                y,
                transform.position.z
            );

            Instantiate(
                blackGiftPrefab,
                spawnPosition,
                Quaternion.identity,
                generatedGiftsParent
            );
        }

        Debug.Log(
            gameObject.name + ": generados " +
            giftCount + " regalos negros.",
            this
        );
    }

    [ContextMenu("Eliminar regalos generados")]
    public void ClearGeneratedGifts()
    {
        Transform existing = transform.Find("GeneratedGifts");

        if (existing == null)
            return;

        if (Application.isPlaying)
            Destroy(existing.gameObject);
        else
            DestroyImmediate(existing.gameObject);

        generatedGiftsParent = null;
    }
}
