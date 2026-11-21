
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panelTransform;

    [Header("Contenedores")]
    [SerializeField] private Transform itemsContent;
    [SerializeField] private Transform enemiesContent;
    [SerializeField] private Transform cursesContent;

    [Header("Prefab")]
    [SerializeField] private InventoryIconUI iconPrefab;

    [Header("Animacion")]
    [SerializeField] private float animationDuration = 0.25f;
    [SerializeField] private Vector2 closedOffset = new Vector2(0, -80);

    private Vector2 openPosition;
    private Vector2 closedPosition;

    private bool isOpen;
    private Coroutine animationCoroutine;

    private void Awake()
    {
        if (panelTransform != null)
        {
            openPosition = panelTransform.anchoredPosition;
            closedPosition = openPosition + closedOffset;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        BindInventory();
    }

    private void Start()
    {
        isOpen = false;

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        if (panelTransform != null)
            panelTransform.anchoredPosition = closedPosition;

        RefreshInventory();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BindInventory();
        RefreshInventory();

        // Al volver de otra escena, el inventario comienza cerrado.
        isOpen = false;

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        if (panelTransform != null)
            panelTransform.anchoredPosition = closedPosition;
    }

    private void BindInventory()
    {
        if (playerInventory != null)
            playerInventory.OnInventoryChanged -= RefreshInventory;

        playerInventory = FindFirstObjectByType<PlayerInventory>();

        if (playerInventory != null)
            playerInventory.OnInventoryChanged += RefreshInventory;
        else
            Debug.LogWarning("InventoryUI: no se encontró PlayerInventory.");
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;

        if (playerInventory != null)
            playerInventory.OnInventoryChanged -= RefreshInventory;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            ToggleInventory();
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;

        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        if (isOpen)
        {
            BindInventory();
            RefreshInventory();

            if (inventoryPanel != null)
                inventoryPanel.SetActive(true);
        }

        animationCoroutine = StartCoroutine(AnimatePanel(isOpen));
    }

    private IEnumerator AnimatePanel(bool opening)
    {
        if (panelTransform == null || canvasGroup == null)
            yield break;

        float elapsed = 0f;

        Vector2 startPosition = panelTransform.anchoredPosition;
        float startAlpha = canvasGroup.alpha;

        Vector2 targetPosition = opening ? openPosition : closedPosition;
        float targetAlpha = opening ? 1f : 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / animationDuration);
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            panelTransform.anchoredPosition =
                Vector2.Lerp(startPosition, targetPosition, smoothProgress);

            canvasGroup.alpha =
                Mathf.Lerp(startAlpha, targetAlpha, smoothProgress);

            yield return null;
        }

        panelTransform.anchoredPosition = targetPosition;
        canvasGroup.alpha = targetAlpha;

        if (!opening && inventoryPanel != null)
            inventoryPanel.SetActive(false);

        animationCoroutine = null;
    }

    public void RefreshInventory()
    {
        if (playerInventory == null || iconPrefab == null)
            return;

        ClearContainer(itemsContent);
        ClearContainer(enemiesContent);
        ClearContainer(cursesContent);

        foreach (InventoryStack stack in playerInventory.Entries)
        {
            if (stack == null || stack.data == null)
                continue;

            Transform destination = GetContainer(stack.data.category);

            if (destination == null)
                continue;

            InventoryIconUI newIcon = Instantiate(
                iconPrefab,
                destination
            );

            newIcon.Configure(stack.data, stack.amount);
        }
    }

    private Transform GetContainer(InventoryCategory category)
    {
        switch (category)
        {
            case InventoryCategory.Item:
                return itemsContent;

            case InventoryCategory.Enemy:
                return enemiesContent;

            case InventoryCategory.Curse:
                return cursesContent;

            default:
                return null;
        }
    }

    private void ClearContainer(Transform container)
    {
        if (container == null)
            return;

        foreach (Transform child in container)
            Destroy(child.gameObject);
    }
}
