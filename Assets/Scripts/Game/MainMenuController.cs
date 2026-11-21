
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("PANELES")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject clasePanel;

    private void Start()
    {
        ShowMainMenu();
    }

    // --------------------------------------------
    // MOSTRAR MENU PRINCIPAL
    // --------------------------------------------

    public void ShowMainMenu()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (clasePanel != null)
            clasePanel.SetActive(false);
    }

    // --------------------------------------------
    // ABRIR PANEL DE CLASE
    // --------------------------------------------

    public void OpenClasePanel()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (clasePanel != null)
            clasePanel.SetActive(true);
    }

    // --------------------------------------------
    // VOLVER AL MENU PRINCIPAL
    // --------------------------------------------

    public void BackToMainMenu()
    {
        ShowMainMenu();
    }

    // --------------------------------------------
    // CREAR PARTIDA
    // --------------------------------------------

    public void CreateGame()
    {
        Debug.Log("BOTÓN CREAR PARTIDA PRESIONADO");

        if (GameManager.Instance == null)
        {
            Debug.LogError("No se encontró GameManager.");
            return;
        }

        GameManager.Instance.StartGame();
    }
}
