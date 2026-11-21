
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[Serializable]
public class ShopOffer2D
{
    public InventoryEntryData item;

    [Min(0.01f)]
    public float price = 10f;
}

public class ShopManager2D : MonoBehaviour
{
    [Header("Articulos disponibles")]
    [SerializeField]
    private List<ShopOffer2D> availableOffers =
        new List<ShopOffer2D>();

    [Header("Escenas")]
    [SerializeField] private string returnSceneName = "Nivel";

    [Header("Referencias del jugador")]
    [SerializeField] private PlayerGold playerGold;
    [SerializeField] private PlayerInventory playerInventory;

    [Header("HUD de oro")]
    [SerializeField] private TMP_Text goldText;

    [Header("Articulo de la tienda")]
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text maxStackText;
    [SerializeField] private Image itemIcon;

    [Header("Botones")]
    [SerializeField] private Button buyButton;
    [SerializeField] private Button rerollButton;
    [SerializeField] private Button continueButton;

    [Header("Texto de rerolls")]
    [SerializeField] private TMP_Text rerollUsesText;

    private ShopOffer2D selectedOffer;

    private int rerollsRemaining;

    // Estado original del jugador para restaurarlo al salir.
    private PlayerController2D playerController;
    private Rigidbody2D playerRigidbody;
    private RigidbodyConstraints2D originalConstraints;
    private Vector2 originalVelocity;
    private bool originalControllerEnabled;

    private SpriteRenderer[] playerRenderers;
    private bool[] originalRendererStates;

    private bool playerStateCaptured;

    private void Start()
    {
        FindPlayerReferences();

        if (playerGold == null || playerInventory == null)
        {
            Debug.LogError(
                "ShopManager2D: no se encontraron PlayerGold o PlayerInventory."
            );

            return;
        }

        // Congelamos y ocultamos al jugador solamente en esta escena.
        HideAndFreezePlayer();

        // Cada entrada a la tienda suma una visita.
        PlayerScenePersistence2D persistence =
            FindFirstObjectByType<PlayerScenePersistence2D>();

        rerollsRemaining = persistence != null
            ? persistence.RegisterShopVisitAndGetRerolls()
            : 3;

        playerGold.OnGoldChanged += UpdateGoldText;

        if (buyButton != null)
            buyButton.onClick.AddListener(BuySelectedItem);

        if (rerollButton != null)
            rerollButton.onClick.AddListener(RerollItem);

        if (continueButton != null)
            continueButton.onClick.AddListener(ContinueGame);

        SelectRandomOffer();

        UpdateGoldText(playerGold.CurrentGold);
        UpdateRerollText();
    }

    private void FindPlayerReferences()
    {
        if (playerGold == null)
            playerGold = FindFirstObjectByType<PlayerGold>();

        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();
    }

    // ------------------------------------------------
    // SELECCION DE ARTICULOS
    // ------------------------------------------------

    private List<ShopOffer2D> GetValidOffers()
    {
        return availableOffers.FindAll(offer =>
            offer != null &&
            offer.item != null &&
            offer.item.category == InventoryCategory.Item &&
            offer.price > 0f
        );
    }

    private void SelectRandomOffer()
    {
        List<ShopOffer2D> validOffers = GetValidOffers();

        if (validOffers.Count == 0)
        {
            ShowNoItemsAvailable();
            return;
        }

        int randomIndex = UnityEngine.Random.Range(
            0,
            validOffers.Count
        );

        selectedOffer = validOffers[randomIndex];

        UpdateItemUI();
    }

    private void RerollItem()
    {
        if (rerollsRemaining <= 0 || selectedOffer == null)
            return;

        List<ShopOffer2D> alternatives = GetValidOffers();

        // Evitamos ofrecer nuevamente el mismo artículo.
        alternatives.RemoveAll(offer =>
            offer.item == selectedOffer.item
        );

        if (alternatives.Count == 0)
        {
            Debug.Log("No hay otros artículos disponibles para cambiar.");
            UpdateRerollText();
            return;
        }

        int randomIndex = UnityEngine.Random.Range(
            0,
            alternatives.Count
        );

        selectedOffer = alternatives[randomIndex];

        rerollsRemaining--;

        UpdateItemUI();
        UpdateRerollText();

        Debug.Log("Artículo cambiado. Rerolls restantes: " + rerollsRemaining);
    }

    private void ShowNoItemsAvailable()
    {
        selectedOffer = null;

        if (itemNameText != null)
            itemNameText.text = "SIN ARTICULOS DISPONIBLES";

        if (descriptionText != null)
            descriptionText.text = "";

        if (priceText != null)
            priceText.text = "";

        if (maxStackText != null)
            maxStackText.text = "";

        if (itemIcon != null)
        {
            itemIcon.sprite = null;
            itemIcon.color = Color.clear;
        }

        if (buyButton != null)
            buyButton.interactable = false;

        if (rerollButton != null)
            rerollButton.interactable = false;
    }

    // ------------------------------------------------
    // ACTUALIZACION DE LA INTERFAZ
    // ------------------------------------------------

    private void UpdateItemUI()
    {
        if (selectedOffer == null)
            return;

        InventoryEntryData item = selectedOffer.item;

        if (itemNameText != null)
            itemNameText.text = item.entryName;

        if (descriptionText != null)
            descriptionText.text = item.description;

        if (priceText != null)
        {
            priceText.text =
                "COMPRAR: " +
                selectedOffer.price.ToString(
                    "0.##",
                    CultureInfo.InvariantCulture
                ) +
                " ORO";
        }

        if (itemIcon != null)
        {
            itemIcon.sprite = item.icon;
            itemIcon.color = item.icon != null
                ? Color.white
                : item.placeholderColor;
        }

        UpdateMaxStackText();
        RefreshBuyButton();
        RefreshRerollButton();
    }

    private void UpdateMaxStackText()
    {
        if (maxStackText == null || selectedOffer == null)
            return;

        InventoryEntryData item = selectedOffer.item;

        int currentAmount = playerInventory.GetAmount(item);
        int maximumAmount = item.maxStack;

        maxStackText.text =
            "MAX STACK: " + currentAmount + " / " + maximumAmount;
    }

    private void UpdateRerollText()
    {
        if (rerollUsesText != null)
        {
            rerollUsesText.text =
                "REROLLS: " + rerollsRemaining;
        }

        RefreshRerollButton();
    }

    private void RefreshRerollButton()
    {
        if (rerollButton == null)
            return;

        List<ShopOffer2D> alternatives = GetValidOffers();

        if (selectedOffer != null)
        {
            alternatives.RemoveAll(offer =>
                offer.item == selectedOffer.item
            );
        }

        bool hasAlternative = alternatives.Count > 0;

        rerollButton.interactable =
            rerollsRemaining > 0 && hasAlternative;
    }

    // ------------------------------------------------
    // COMPRAS
    // ------------------------------------------------

    private void BuySelectedItem()
    {
        if (selectedOffer == null)
            return;

        InventoryEntryData item = selectedOffer.item;

        // Comprobamos el límite de unidades.
        if (!playerInventory.CanAddEntry(item))
        {
            RefreshBuyButton();
            UpdateMaxStackText();
            return;
        }

        // Comprobamos el oro disponible.
        if (playerGold.CurrentGold < selectedOffer.price)
        {
            Debug.Log("No tenés suficiente oro.");
            RefreshBuyButton();
            return;
        }

        if (!playerGold.SpendGold(selectedOffer.price))
            return;

        bool added = playerInventory.TryAddEntry(item);

        if (!added)
        {
            Debug.LogError(
                "No se pudo agregar el artículo comprado."
            );

            return;
        }

        Debug.Log(
            "Compraste " + item.entryName +
            " (" + playerInventory.GetAmount(item) +
            "/" + item.maxStack + ")"
        );

        UpdateMaxStackText();
        RefreshBuyButton();
    }

    private void RefreshBuyButton()
    {
        if (buyButton == null || selectedOffer == null)
            return;

        InventoryEntryData item = selectedOffer.item;

        bool hasSpace = playerInventory.CanAddEntry(item);
        bool hasGold = playerGold.CurrentGold >= selectedOffer.price;

        buyButton.interactable = hasSpace && hasGold;

        TMP_Text buttonLabel =
            buyButton.GetComponentInChildren<TMP_Text>();

        if (buttonLabel == null)
            return;

        if (!hasSpace)
        {
            buttonLabel.text = "MAXIMO ALCANZADO";
        }
        else if (!hasGold)
        {
            buttonLabel.text = "ORO INSUFICIENTE";
        }
        else
        {
            buttonLabel.text =
                "COMPRAR: " +
                selectedOffer.price.ToString(
                    "0.##",
                    CultureInfo.InvariantCulture
                );
        }
    }

    private void UpdateGoldText(float amount)
    {
        if (goldText != null)
        {
            goldText.text =
                "ORO: " +
                amount.ToString(
                    "0.##",
                    CultureInfo.InvariantCulture
                );
        }

        RefreshBuyButton();
    }

    // ------------------------------------------------
    // OCULTAR Y CONGELAR AL JUGADOR
    // ------------------------------------------------

    private void HideAndFreezePlayer()
    {
        playerController =
            FindFirstObjectByType<PlayerController2D>();

        if (playerController == null)
            return;

        playerRigidbody =
            playerController.GetComponent<Rigidbody2D>();

        playerRenderers =
            playerController.GetComponentsInChildren<SpriteRenderer>(true);

        originalRendererStates = new bool[playerRenderers.Length];

        for (int i = 0; i < playerRenderers.Length; i++)
        {
            originalRendererStates[i] = playerRenderers[i].enabled;
            playerRenderers[i].enabled = false;
        }

        originalControllerEnabled = playerController.enabled;
        playerController.enabled = false;

        if (playerRigidbody != null)
        {
            originalConstraints = playerRigidbody.constraints;
            originalVelocity = playerRigidbody.linearVelocity;

            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0f;
            playerRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        playerStateCaptured = true;
    }

    private void RestorePlayer()
    {
        if (!playerStateCaptured)
            return;

        if (playerRenderers != null && originalRendererStates != null)
        {
            for (int i = 0; i < playerRenderers.Length; i++)
            {
                if (playerRenderers[i] != null)
                    playerRenderers[i].enabled = originalRendererStates[i];
            }
        }

        if (playerRigidbody != null)
        {
            playerRigidbody.constraints = originalConstraints;
            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0f;
        }

        if (playerController != null)
            playerController.enabled = originalControllerEnabled;

        playerStateCaptured = false;
    }

    // ------------------------------------------------
    // SALIR DE LA TIENDA
    // ------------------------------------------------

    private void ContinueGame()
    {
        if (string.IsNullOrWhiteSpace(returnSceneName))
        {
            Debug.LogError("Falta configurar Return Scene Name.");
            return;
        }

        SceneManager.LoadScene(returnSceneName);
    }

    private void OnDestroy()
    {
        if (playerGold != null)
            playerGold.OnGoldChanged -= UpdateGoldText;

        if (buyButton != null)
            buyButton.onClick.RemoveListener(BuySelectedItem);

        if (rerollButton != null)
            rerollButton.onClick.RemoveListener(RerollItem);

        if (continueButton != null)
            continueButton.onClick.RemoveListener(ContinueGame);

        // Se ejecuta al abandonar la escena Tienda.
        RestorePlayer();
    }
}
