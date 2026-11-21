using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProgressManager2D : MonoBehaviour
{
    public static LevelProgressManager2D Instance { get; private set; }

    [Header("Referencias")]
    [SerializeField] private ProceduralLevelGenerator2D levelGenerator;
    [SerializeField] private GiftProgressManager2D giftManager;

    [Header("Configuración de escenas")]
    [SerializeField] private string gameplaySceneName = "SampleScene";
    [SerializeField] private string shopSceneName = "Tienda";

    [Header("Progreso")]
    [Min(1)]
    [SerializeField] private int levelsBetweenShopVisits = 2;

    private bool isTransitioning;

    public int CurrentLevel
    {
        get
        {
            if (PlayerScenePersistence2D.Instance == null)
                return 1;

            return PlayerScenePersistence2D.Instance.CurrentLevel;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(InitializeLevel());
    }

    // ------------------------------------------------
    // INICIO DE ESCENA
    // ------------------------------------------------

    private IEnumerator InitializeLevel()
    {
        // Esperamos a que los objetos de la escena estén listos.
        yield return null;

        if (!ValidateReferences())
            yield break;

        isTransitioning = false;

        GenerateCurrentLevel();
    }

    private bool ValidateReferences()
    {
        if (levelGenerator == null)
        {
            Debug.LogError(
                "LevelProgressManager2D: falta asignar Level Generator.",
                this
            );

            return false;
        }

        if (giftManager == null)
            giftManager = GiftProgressManager2D.Instance;

        if (giftManager == null)
        {
            Debug.LogError(
                "LevelProgressManager2D: no se encontró GiftProgressManager2D.",
                this
            );

            return false;
        }

        if (PlayerScenePersistence2D.Instance == null)
        {
            Debug.LogError(
                "No se encontró PlayerScenePersistence2D en el jugador."
            );

            return false;
        }

        return true;
    }

    // ------------------------------------------------
    // GENERAR NIVEL ACTUAL
    // ------------------------------------------------

    private void GenerateCurrentLevel()
    {
        int level = CurrentLevel;

        Debug.Log("================================");
        Debug.Log("COMIENZA EL NIVEL " + level);
        Debug.Log("================================");

        // Primero eliminamos los elementos del nivel anterior.
        giftManager.PrepareForNewLevel();

        // Generamos las nuevas plataformas.
        bool generated = levelGenerator.GenerateLevel(level);

        if (!generated)
        {
            Debug.LogError("No se pudo generar el nivel " + level);
            return;
        }

        // Colocamos al jugador en la plataforma inicial.
        ResetPlayerPosition();

        // Inicializamos los regalos y la puerta del nuevo nivel.
        giftManager.InitializeForNewLevel(
            levelGenerator.ExitDoorPosition,
            levelGenerator.ExitDoorRotation
        );
    }

    // ------------------------------------------------
    // RECOLOCAR JUGADOR
    // ------------------------------------------------

    private void ResetPlayerPosition()
    {
        PlayerController2D player =
            FindFirstObjectByType<PlayerController2D>();

        if (player == null)
        {
            Debug.LogError(
                "LevelProgressManager2D: no se encontró PlayerController2D."
            );

            return;
        }

        Vector3 spawnPosition =
            levelGenerator.PlayerSpawnPosition;

        Rigidbody2D rb =
            player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.position = spawnPosition;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        else
        {
            player.transform.position = spawnPosition;
        }

        Debug.Log("Jugador colocado en la plataforma inicial.");
    }

    // ------------------------------------------------
    // COMPLETAR NIVEL
    // ------------------------------------------------

    public void CompleteCurrentLevel()
    {
        if (isTransitioning)
            return;

        if (PlayerScenePersistence2D.Instance == null)
        {
            Debug.LogError(
                "No se encontró PlayerScenePersistence2D."
            );

            return;
        }

        isTransitioning = true;

        int completedLevel = CurrentLevel;

        Debug.Log("Nivel completado: " + completedLevel);

        // Guardamos el siguiente nivel en el jugador persistente.
        PlayerScenePersistence2D.Instance.AdvanceToNextLevel();

        int shopInterval = Mathf.Max(1, levelsBetweenShopVisits);

        bool shouldVisitShop =
            completedLevel % shopInterval == 0;

        if (shouldVisitShop)
        {
            Debug.Log(
                "Se completaron " + completedLevel +
                " niveles. Entrando a la tienda."
            );

            SceneManager.LoadScene(shopSceneName);
        }
        else
        {
            StartCoroutine(GenerateNextLevel());
        }
    }

    // ------------------------------------------------
    // SIGUIENTE NIVEL EN LA MISMA ESCENA
    // ------------------------------------------------

    private IEnumerator GenerateNextLevel()
    {
        // Esperamos un frame para finalizar la interacción con la puerta.
        yield return null;

        if (!ValidateReferences())
            yield break;

        GenerateCurrentLevel();

        // Permitimos interactuar con la nueva puerta.
        isTransitioning = false;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}