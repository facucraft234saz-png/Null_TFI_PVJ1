
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuracion")]
    [SerializeField] private string gameplaySceneName = "Level01";

    public GameState CurrentState { get; private set; }

    public DifficultyData SelectedDifficulty { get; private set; }

    // Evento para que otros sistemas puedan escuchar
    // los cambios de estado sin depender directamente
    // de este GameManager.
    public event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        // Evita tener dos GameManagers simultaneos.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Conserva el GameManager al cambiar de escena.
        DontDestroyOnLoad(gameObject);

        ChangeState(GameState.Start);
    }

    // ------------------------------------------------
    // DIFICULTAD
    // ------------------------------------------------

    public void SetDifficulty(DifficultyData difficulty)
    {
        if (difficulty == null)
        {
            Debug.LogError("No se puede seleccionar una dificultad nula.");
            return;
        }

        SelectedDifficulty = difficulty;

        Debug.Log(
            "Dificultad seleccionada: " +
            difficulty.difficultyName
        );
    }

    // ------------------------------------------------
    // INICIAR PARTIDA
    // ------------------------------------------------

    public void StartGame()
    {
        if (SelectedDifficulty == null)
        {
            Debug.LogWarning(
                "Primero debes seleccionar una dificultad."
            );

            return;
        }

        // Preparamos el estado antes de cargar el nivel.
        ChangeState(GameState.Playing);

        Debug.Log(
            "Iniciando partida en dificultad: " +
            SelectedDifficulty.difficultyName
        );

        SceneManager.LoadScene(gameplaySceneName);
    }

    // ------------------------------------------------
    // CAMBIO DE ESTADO
    // ------------------------------------------------

    public void ChangeState(GameState newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState = newState;

        Debug.Log("Estado del juego: " + CurrentState);

        OnGameStateChanged?.Invoke(CurrentState);
    }

    // ------------------------------------------------
    // LIMPIEZA
    // ------------------------------------------------

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
