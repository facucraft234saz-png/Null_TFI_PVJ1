
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("PANEL DE PAUSA")]
    [SerializeField] private GameObject pausePanel;

    [Header("JUGADOR")]
    [SerializeField] private PlayerVitals playerVitals;

    [Header("BOTONES")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;

    private bool isPaused;

    private void Awake()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    // ------------------------------------------------
    // PAUSAR
    // ------------------------------------------------

    public void PauseGame()
    {
        if (isPaused)
            return;

        if (GameManager.Instance == null)
        {
            Debug.LogError(
                "PauseMenuController: no existe GameManager."
            );

            return;
        }

        // Evitamos abrir la pausa después de perder.
        if (playerVitals != null && playerVitals.IsGameOver)
            return;

        isPaused = true;

        pausePanel.SetActive(true);

        GameManager.Instance.ChangeState(GameState.Paused);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPauseMusic();
        }

        Debug.Log("Partida pausada.");
    }

    // ------------------------------------------------
    // REANUDAR
    // ------------------------------------------------

    public void ResumeGame()
    {
        if (!isPaused)
            return;

        isPaused = false;

        pausePanel.SetActive(false);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameState.Playing);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ResumeGameplayMusic();
        }

        Debug.Log("Partida reanudada.");
    }

    // ------------------------------------------------
    // VOLVER AL MENU
    // ------------------------------------------------

    public void ReturnToMainMenu()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonSound();
        }

        isPaused = false;

        Time.timeScale = 1f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMainMenu();
        }
        else
        {
            Debug.LogError(
                "PauseMenuController: no existe GameManager."
            );
        }
    }

    private void OnDestroy()
    {
        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(ResumeGame);

        if (mainMenuButton != null)
            mainMenuButton.onClick.RemoveListener(ReturnToMainMenu);
    }
}
