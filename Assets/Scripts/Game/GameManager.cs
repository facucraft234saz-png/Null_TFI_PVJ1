
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Escenas")]
    [SerializeField] private string gameplaySceneName = "SampleScene";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public GameState CurrentState { get; private set; }

    public event Action<GameState> OnGameStateChanged;

    private bool isLoadingGame;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("GameManager duplicado. Se elimina: " + gameObject.name);
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        CurrentState = GameState.Start;
        Time.timeScale = 1f;

        Debug.Log("GameManager creado correctamente y marcado como persistente.");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Escena cargada: " + scene.name +
                  " | GameManager existe: " + (Instance != null));
    }

    public void StartGame()
    {
        if (isLoadingGame)
            return;

        isLoadingGame = true;

        Time.timeScale = 1f;

        PlayerScenePersistence2D.PrepareNewRun();

        ChangeState(GameState.Playing);

        Debug.Log("Iniciando partida: " + gameplaySceneName);

        SceneManager.LoadScene(gameplaySceneName);
    }

    public void ChangeState(GameState newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState = newState;

        if (newState == GameState.GameOver ||
            newState == GameState.Paused ||
            newState == GameState.Victory)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }

        Debug.Log("Estado del juego: " + CurrentState);

        OnGameStateChanged?.Invoke(CurrentState);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        PlayerScenePersistence2D.EndRun();

        isLoadingGame = false;

        ChangeState(GameState.Start);

        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Debug.LogWarning("GameManager persistente destruido.");

            Time.timeScale = 1f;
            Instance = null;
        }
    }
}
