
using UnityEngine;

public class GameOverUIController : MonoBehaviour
{
    [Header("Panel Game Over")]
    [SerializeField] private GameObject gameOverPanel;

    private void Awake()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        else
            Debug.LogError("GameOverUIController: falta asignar GameOverPanel.");
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;

            // Por si el objeto se activa cuando ya ocurrió el Game Over.
            HandleGameStateChanged(GameManager.Instance.CurrentState);
        }
        else
        {
            Debug.LogError("GameOverUIController: no se encontró GameManager.");
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (gameOverPanel == null)
            return;

        gameOverPanel.SetActive(state == GameState.GameOver);
    }

    public void ReturnToMainMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMainMenu();
        }
        else
        {
            Debug.LogError("No se encontró GameManager para volver al menú.");
        }
    }
}
