
using UnityEngine;
using UnityEngine.UI;

public class PlayerVitalsHUD : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField] private PlayerVitals playerVitals;

    [Header("Contenedores")]
    [SerializeField] private Transform heartsContainer;
    [SerializeField] private Transform shieldsContainer;

    [Header("Prefab del icono")]
    [SerializeField] private Image iconPrefab;

    [Header("Sprites opcionales")]
    [SerializeField] private Sprite heartSprite;
    [SerializeField] private Sprite shieldSprite;

    [Header("Colores provisionales")]
    [SerializeField] private Color heartColor = Color.red;
    [SerializeField] private Color shieldColor = Color.cyan;

    private static Sprite placeholderSprite;

    private void Awake()
    {
        CreatePlaceholderSprite();
    }

    private void OnEnable()
    {
        if (playerVitals == null)
            return;

        playerVitals.OnHeartsChanged += UpdateHearts;
        playerVitals.OnShieldsChanged += UpdateShields;
    }

    private void Start()
    {
        if (playerVitals == null || iconPrefab == null)
        {
            Debug.LogError(
                "PlayerVitalsHUD: faltan referencias.",
                this
            );
            return;
        }

        // Mostramos los valores iniciales.
        UpdateHearts(
            playerVitals.CurrentHearts,
            playerVitals.MaxHearts
        );

        UpdateShields(playerVitals.CurrentShields);
    }

    private void OnDisable()
    {
        if (playerVitals == null)
            return;

        playerVitals.OnHeartsChanged -= UpdateHearts;
        playerVitals.OnShieldsChanged -= UpdateShields;
    }

    private void CreatePlaceholderSprite()
    {
        if (placeholderSprite != null)
            return;

        placeholderSprite = Sprite.Create(
            Texture2D.whiteTexture,
            new Rect(0, 0, 1, 1),
            new Vector2(0.5f, 0.5f)
        );
    }

    private void UpdateHearts(int current, int maximum)
    {
        RebuildIcons(
            heartsContainer,
            current,
            heartSprite,
            heartColor
        );
    }

    private void UpdateShields(int amount)
    {
        RebuildIcons(
            shieldsContainer,
            amount,
            shieldSprite,
            shieldColor
        );
    }

    private void RebuildIcons(
        Transform container,
        int amount,
        Sprite customSprite,
        Color placeholderColor)
    {
        if (container == null || iconPrefab == null)
            return;

        // Eliminamos los iconos anteriores.
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        // Creamos uno por cada corazón o escudo.
        for (int i = 0; i < amount; i++)
        {
            Image newIcon = Instantiate(
                iconPrefab,
                container
            );

            newIcon.sprite =
                customSprite != null
                ? customSprite
                : placeholderSprite;

            newIcon.color =
                customSprite != null
                ? Color.white
                : placeholderColor;

            newIcon.preserveAspect = true;
            newIcon.raycastTarget = false;

            newIcon.rectTransform.sizeDelta =
                new Vector2(22, 22);
        }
    }
}
