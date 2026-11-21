
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EnemyShopManager2D : MonoBehaviour
{
    [Header("Enemigos disponibles")]
    [SerializeField]
    private List<InventoryEntryData> availableEnemies =
        new List<InventoryEntryData>();

    [Header("Escena de regreso")]
    [SerializeField] private string returnSceneName = "SampleScene";

    [Header("Referencias del jugador")]
    [SerializeField] private PlayerInventory playerInventory;

    [Header("Interfaz del enemigo")]
    [SerializeField] private TMP_Text enemyNameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text stackText;
    [SerializeField] private Image enemyIcon;

    [Header("Botones")]
    [SerializeField] private Button selectButton;
    [SerializeField] private Button rerollButton;

    [Header("Texto de rerolls")]
    [SerializeField] private TMP_Text rerollUsesText;

    private InventoryEntryData selectedEnemy;

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

    private void Start()
    {
        FindPlayerReferences();

        if (playerInventory == null)
        {
            Debug.LogError(
                "EnemyShopManager2D: no se encontró PlayerInventory."
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
            selectButton.onClick.AddListener(SelectEnemy);

        if (rerollButton != null)
            rerollButton.onClick.AddListener(RerollEnemy);

        SelectRandomEnemy();
        UpdateRerollText();
    }

    private void FindPlayerReferences()
    {
        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();
    }

    // ------------------------------------------------
    // ENEMIGOS VALIDOS
    // ------------------------------------------------

    private List<InventoryEntryData> GetValidEnemies()
    {
        return availableEnemies.FindAll(enemy =>
            enemy != null &&
            enemy.category == InventoryCategory.Enemy &&
            enemy.enemyDefinition != null
        );
    }

    // ------------------------------------------------
    // SELECCION ALEATORIA
    // ------------------------------------------------

    private void SelectRandomEnemy()
    {
        List<InventoryEntryData> validEnemies = GetValidEnemies();

        if (validEnemies.Count == 0)
        {
            ShowNoEnemiesAvailable();
            return;
        }

        selectedEnemy = validEnemies[
            Random.Range(0, validEnemies.Count)
        ];

        UpdateEnemyUI();
    }

    // ------------------------------------------------
    // REROLL
    // ------------------------------------------------

    private void RerollEnemy()
    {
        if (isSelecting ||
            rerollsRemaining <= 0 ||
            selectedEnemy == null)
        {
            return;
        }

        List<InventoryEntryData> alternatives = GetValidEnemies();

        alternatives.RemoveAll(enemy => enemy == selectedEnemy);

        if (alternatives.Count == 0)
        {
            Debug.Log("No hay otros enemigos disponibles.");
            return;
        }

        selectedEnemy = alternatives[
            Random.Range(0, alternatives.Count)
        ];

        rerollsRemaining--;

        UpdateEnemyUI();
        UpdateRerollText();

        Debug.Log(
            "Enemigo cambiado. Rerolls restantes: " +
            rerollsRemaining
        );
    }

    // ------------------------------------------------
    // INTERFAZ
    // ------------------------------------------------

    private void UpdateEnemyUI()
    {
        if (selectedEnemy == null)
            return;

        if (enemyNameText != null)
            enemyNameText.text = selectedEnemy.entryName;

        if (descriptionText != null)
            descriptionText.text = selectedEnemy.description;

        if (enemyIcon != null)
        {
            enemyIcon.sprite = selectedEnemy.icon;

            enemyIcon.color = selectedEnemy.icon != null
                ? Color.white
                : selectedEnemy.placeholderColor;
        }

        UpdateStackText();
        RefreshSelectButton();
        RefreshRerollButton();
    }

    private void UpdateStackText()
    {
        if (stackText == null || selectedEnemy == null)
            return;

        int currentAmount =
            playerInventory.GetAmount(selectedEnemy);

        int maximumAmount =
            Mathf.Max(1, selectedEnemy.maxStack);

        stackText.text =
            "EQUIPADOS: " + currentAmount +
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

        List<InventoryEntryData> alternatives = GetValidEnemies();

        if (selectedEnemy != null)
            alternatives.RemoveAll(enemy => enemy == selectedEnemy);

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

        bool validEnemy =
            selectedEnemy != null &&
            playerInventory != null;

        bool canAdd =
            validEnemy &&
            playerInventory.CanAddEntry(selectedEnemy);

        // El botón sigue habilitado aunque el stack esté lleno.
        // En ese caso, simplemente permite regresar.
        selectButton.interactable =
            !isSelecting && validEnemy;

        TMP_Text buttonLabel =
            selectButton.GetComponentInChildren<TMP_Text>();

        if (buttonLabel != null)
        {
            buttonLabel.text = canAdd
                ? "SELECCIONAR"
                : "VOLVER (MAXIMO)";
        }
    }

    private void ShowNoEnemiesAvailable()
    {
        selectedEnemy = null;

        if (enemyNameText != null)
            enemyNameText.text = "SIN ENEMIGOS DISPONIBLES";

        if (descriptionText != null)
            descriptionText.text = "";

        if (stackText != null)
            stackText.text = "";

        if (enemyIcon != null)
        {
            enemyIcon.sprite = null;
            enemyIcon.color = Color.clear;
        }

        if (selectButton != null)
            selectButton.interactable = false;

        if (rerollButton != null)
            rerollButton.interactable = false;
    }

    // ------------------------------------------------
    // SELECCIONAR O VOLVER
    // ------------------------------------------------

    private void SelectEnemy()
    {
        if (isSelecting ||
            selectedEnemy == null ||
            playerInventory == null)
        {
            return;
        }

        // Comprobamos nuevamente el stack al pulsar.
        // No confiamos solamente en el estado visual del botón.
        if (playerInventory.CanAddEntry(selectedEnemy))
        {
            bool added =
                playerInventory.TryAddEntry(selectedEnemy);

            if (added)
            {
                Debug.Log(
                    "Enemigo agregado: " +
                    selectedEnemy.entryName
                );
            }
            else
            {
                Debug.Log(
                    "No se agregó el enemigo porque alcanzó el máximo."
                );
            }
        }
        else
        {
            Debug.Log(
                "Stack máximo alcanzado. Volviendo sin agregar enemigo."
            );
        }

        // En ambos casos regresamos al nivel.
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
                "EnemyShopManager2D: no se encontró PlayerController2D."
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
            selectButton.onClick.RemoveListener(SelectEnemy);

        if (rerollButton != null)
            rerollButton.onClick.RemoveListener(RerollEnemy);

        RestorePlayer();
    }
}
