
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [Header("PANELES")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject clasePanel;
    [SerializeField] private GameObject ajustesPanel;

    private void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (clasePanel != null)
            clasePanel.SetActive(false);

        if (ajustesPanel != null)
            ajustesPanel.SetActive(false);
    }

    public void OpenClasePanel()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (clasePanel != null)
            clasePanel.SetActive(true);

        if (ajustesPanel != null)
            ajustesPanel.SetActive(false);
    }

    public void OpenAjustesPanel()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (clasePanel != null)
            clasePanel.SetActive(false);

        if (ajustesPanel != null)
            ajustesPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        ShowMainMenu();
    }

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
