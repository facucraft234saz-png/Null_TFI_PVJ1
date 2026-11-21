
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CurseShopManager2D : MonoBehaviour
{
    [Header("Maldiciones disponibles")]
    [SerializeField]
    private List<InventoryEntryData> availableCurses =
        new List<InventoryEntryData>();

    [Header("Escena de regreso")]
    [SerializeField] private string returnSceneName = "SampleScene";

    [Header("Referencias del jugador")]
    [SerializeField] private PlayerInventory playerInventory;

    [Header("Interfaz de la maldicion")]
    [SerializeField] private TMP_Text curseNameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text stackText;
    [SerializeField] private Image curseIcon;

    [Header("Botones")]
    [SerializeField] private Button selectButton;
    [SerializeField] private Button rerollButton;

    [Header("Texto de rerolls")]
    [SerializeField] private TMP_Text rerollUsesText;

    private InventoryEntryData selectedCurse;

    private int rerollsRemaining;
    private bool isSelecting;

    // Estado original del jugador.
    private PlayerController2D playerController;
    private Rigidbody2D playerRigidbody;

    private RigidbodyConstraints2D originalConstraints;
    private bool originalControllerEnabled;

    private SpriteRenderer[] playerRenderers;
    private bool[] originalRendererStates;

    private bool playerStateCaptured;

    // ------------------------------------------------
    // INICIO
    // ------------------------------------------------

    private void Start()
    {
        FindPlayerReferences();

        if (playerInventory == null)
        {
            Debug.LogError(
                "CurseShopManager2D: no se encontró PlayerInventory."
            );

            enabled = false;
            return;
        }

        HideAndFreezePlayer();

        PlayerScenePersistence2D persistence =
            PlayerScenePersistence2D.Instance;

        rerollsRemaining = persistence != null
            ? persistence.RegisterShopVisitAndGetRerolls()
            : 3;

        if (selectButton != null)
            selectButton.onClick.AddListener(SelectCurse);

        if (rerollButton != null)
            rerollButton.onClick.AddListener(RerollCurse);

        SelectRandomCurse();
        UpdateRerollText();
    }

    private void FindPlayerReferences()
    {
        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();
    }

    // ------------------------------------------------
    // MALDICIONES VALIDAS
    // ------------------------------------------------

    private List<InventoryEntryData> GetValidCurses()
    {
        return availableCurses.FindAll(curse =>
            curse != null &&
            curse.category == InventoryCategory.Curse
        );
    }

    // ------------------------------------------------
    // SELECCION ALEATORIA
    // ------------------------------------------------

    private void SelectRandomCurse()
    {
        List<InventoryEntryData> validCurses = GetValidCurses();

        if (validCurses.Count == 0)
        {
            ShowNoCursesAvailable();
            return;
        }

        selectedCurse = validCurses[
            Random.Range(0, validCurses.Count)
        ];

        UpdateCurseUI();
    }

    // ------------------------------------------------
    // REROLL
    // ------------------------------------------------

    private void RerollCurse()
    {
        if (isSelecting ||
            rerollsRemaining <= 0 ||
            selectedCurse == null)
        {
            return;
        }

        List<InventoryEntryData> alternatives = GetValidCurses();

        alternatives.RemoveAll(curse => curse == selectedCurse);

        if (alternatives.Count == 0)
        {
            Debug.Log("No hay otras maldiciones disponibles.");
            return;
        }

        selectedCurse = alternatives[
            Random.Range(0, alternatives.Count)
        ];

        rerollsRemaining--;

        UpdateCurseUI();
        UpdateRerollText();

        Debug.Log(
            "Maldición cambiada. Rerolls restantes: " +
            rerollsRemaining
        );
    }

    // ------------------------------------------------
    // INTERFAZ
    // ------------------------------------------------

    private void UpdateCurseUI()
    {
        if (selectedCurse == null)
            return;

        if (curseNameText != null)
            curseNameText.text = selectedCurse.entryName;

        if (descriptionText != null)
            descriptionText.text = selectedCurse.description;

        if (curseIcon != null)
        {
            curseIcon.sprite = selectedCurse.icon;

            curseIcon.color = selectedCurse.icon != null
                ? Color.white
                : selectedCurse.placeholderColor;
        }

        UpdateStackText();
        RefreshSelectButton();
        RefreshRerollButton();
    }

    private void UpdateStackText()
    {
        if (stackText == null || selectedCurse == null)
            return;

        int currentAmount =
            playerInventory.GetAmount(selectedCurse);

        int maximumAmount =
            Mathf.Max(1, selectedCurse.maxStack);

        stackText.text =
            "EQUIPADAS: " + currentAmount +
            " / " + maximumAmount;
    }

    private void UpdateRerollText()
    {
        if (rerollUsesText != null)
            rerollUsesText.text = "REROLLS: " + rerollsRemaining;

        RefreshRerollButton();
    }

    private void RefreshRerollButton()
    {
        if (rerollButton == null)
            return;

        List<InventoryEntryData> alternatives = GetValidCurses();

        if (selectedCurse != null)
            alternatives.RemoveAll(curse => curse == selectedCurse);

        rerollButton.interactable =
            !isSelecting &&
            rerollsRemaining > 0 &&
            alternatives.Count > 0;
    }

    // ------------------------------------------------
    // BOTON SELECCIONAR
    // ------------------------------------------------

    private void RefreshSelectButton()
    {
        if (selectButton == null)
            return;

        bool validCurse =
            selectedCurse != null &&
            playerInventory != null;

        bool canAdd =
            validCurse &&
            playerInventory.CanAddEntry(selectedCurse);

        // El botón permanece habilitado aunque el stack esté lleno.
        selectButton.interactable =
            !isSelecting && validCurse;

        TMP_Text buttonLabel =
            selectButton.GetComponentInChildren<TMP_Text>();

        if (buttonLabel != null)
        {
            buttonLabel.text = canAdd
                ? "SELECCIONAR"
                : "VOLVER (MAXIMO)";
        }
    }

    private void ShowNoCursesAvailable()
    {
        selectedCurse = null;

        if (curseNameText != null)
            curseNameText.text = "SIN MALDICIONES DISPONIBLES";

        if (descriptionText != null)
            descriptionText.text = "";

        if (stackText != null)
            stackText.text = "";

        if (curseIcon != null)
        {
            curseIcon.sprite = null;
            curseIcon.color = Color.clear;
        }

        if (selectButton != null)
            selectButton.interactable = false;

        if (rerollButton != null)
            rerollButton.interactable = false;
    }

    // ------------------------------------------------
    // SELECCIONAR O VOLVER
    // ------------------------------------------------

    private void SelectCurse()
    {
        if (isSelecting ||
            selectedCurse == null ||
            playerInventory == null)
        {
            return;
        }

        // Comprobamos nuevamente el límite del stack.
        if (playerInventory.CanAddEntry(selectedCurse))
        {
            bool added =
                playerInventory.TryAddEntry(selectedCurse);

            if (added)
            {
                Debug.Log(
                    "Maldición agregada: " +
                    selectedCurse.entryName
                );
            }
            else
            {
                Debug.Log(
                    "No se agregó la maldición porque alcanzó el máximo."
                );
            }
        }
        else
        {
            Debug.Log(
                "Stack máximo alcanzado. Volviendo sin agregar maldición."
            );
        }

        // Siempre regresamos a la partida.
        isSelecting = true;

        if (selectButton != null)
            selectButton.interactable = false;

        if (rerollButton != null)
            rerollButton.interactable = false;

        SceneManager.LoadScene(returnSceneName);
    }

    // ------------------------------------------------
    // OCULTAR Y CONGELAR JUGADOR
    // ------------------------------------------------

    private void HideAndFreezePlayer()
    {
        playerController =
            FindFirstObjectByType<PlayerController2D>();

        if (playerController == null)
        {
            Debug.LogError(
                "CurseShopManager2D: no se encontró PlayerController2D."
            );

            return;
        }

        playerRigidbody =
            playerController.GetComponent<Rigidbody2D>();

        playerRenderers =
            playerController.GetComponentsInChildren<SpriteRenderer>(true);

        originalRendererStates =
            new bool[playerRenderers.Length];

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

            playerRigidbody.linearVelocity = Vector2.zero;
            playerRigidbody.angularVelocity = 0f;

            playerRigidbody.constraints =
                RigidbodyConstraints2D.FreezeAll;
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
                    playerRenderers[i].enabled =
                        originalRendererStates[i];
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
    // LIMPIEZA
    // ------------------------------------------------

    private void OnDestroy()
    {
        if (selectButton != null)
            selectButton.onClick.RemoveListener(SelectCurse);

        if (rerollButton != null)
            rerollButton.onClick.RemoveListener(RerollCurse);

        RestorePlayer();
    }
}
