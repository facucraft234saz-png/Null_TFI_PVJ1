
using TMPro;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject difficultyPanel;

    [Header("Dificultades")]
    [SerializeField] private DifficultyData normalDifficulty;
    [SerializeField] private DifficultyData hardDifficulty;
    [SerializeField] private DifficultyData extremeDifficulty;

    [Header("Informacion de dificultad")]
    [SerializeField] private TMP_Text difficultyNameText;
    [SerializeField] private TMP_Text difficultyDescriptionText;
    [SerializeField] private TMP_Text difficultyAttributesText;

    private DifficultyData selectedDifficulty;

    private void Start()
    {
        // Al iniciar mostramos solamente el menu principal.
        ShowMainMenu();

        // Seleccion inicial: Normal.
        SelectNormalDifficulty();
    }

    // ------------------------------------------------
    // NAVEGACION ENTRE PANELES
    // ------------------------------------------------

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        difficultyPanel.SetActive(false);
    }

    public void OpenDifficultyMenu()
    {
        mainMenuPanel.SetActive(false);
        difficultyPanel.SetActive(true);

        // Si todavia no hay seleccion, seleccionamos Normal.
        if (selectedDifficulty == null)
        {
            SelectNormalDifficulty();
        }
    }

    public void BackToMainMenu()
    {
        ShowMainMenu();
    }

    // ------------------------------------------------
    // SELECCION DE DIFICULTAD
    // ------------------------------------------------

    public void SelectNormalDifficulty()
    {
        SelectDifficulty(normalDifficulty);
    }

    public void SelectHardDifficulty()
    {
        SelectDifficulty(hardDifficulty);
    }

    public void SelectExtremeDifficulty()
    {
        SelectDifficulty(extremeDifficulty);
    }

    private void SelectDifficulty(DifficultyData difficulty)
    {
        if (difficulty == null)
        {
            Debug.LogError(
                "Falta asignar uno de los assets de dificultad.",
                this
            );

            return;
        }

        selectedDifficulty = difficulty;

        // Actualiza la informacion visual.
        difficultyNameText.text = difficulty.difficultyName;

        difficultyDescriptionText.text = difficulty.description;

        difficultyAttributesText.text =
            "VIDAS INICIALES: " + difficulty.startingLives + "\n" +
            "MULTIPLICADOR DE ORO: x" +
                difficulty.goldMultiplier.ToString("0.0") + "\n\n" +

            "SELECCION DE ENEMIGOS: cada " +
                difficulty.enemySelectionEveryLevels + " nivel(es)\n" +

            "MALDICIONES: cada " +
                difficulty.curseSelectionEveryLevels + " nivel(es)\n" +

            "TIENDA: cada " +
                difficulty.shopEveryLevels + " nivel(es)";

        Debug.Log(
            "Dificultad seleccionada: " +
            difficulty.difficultyName
        );
    }

    // ------------------------------------------------
    // CREAR PARTIDA
    // ------------------------------------------------

    public void CreateGame()
    {
        if (selectedDifficulty == null)
        {
            Debug.LogWarning(
                "Selecciona una dificultad antes de continuar."
            );

            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError(
                "No se encontro el GameManager en la escena."
            );

            return;
        }

        // Guardamos la dificultad elegida en el GameManager.
        GameManager.Instance.SetDifficulty(selectedDifficulty);

        // Cambia el estado a Playing y carga Level01.
        GameManager.Instance.StartGame();
    }
}
