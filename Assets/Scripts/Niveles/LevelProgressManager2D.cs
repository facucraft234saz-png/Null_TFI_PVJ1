
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProgressManager2D : MonoBehaviour
{
    public static LevelProgressManager2D Instance { get; private set; }

    [Header("Referencias")]
    [SerializeField] private ProceduralLevelGenerator2D levelGenerator;
    [SerializeField] private GiftProgressManager2D giftManager;

    [Header("Configuracion de escenas")]
    [SerializeField] private string gameplaySceneName = "SampleScene";

    [Tooltip("Tienda normal de objetos.")]
    [SerializeField] private string shopSceneName = "Tienda";

    [Tooltip("Tienda donde se seleccionan enemigos.")]
    [SerializeField] private string enemyShopSceneName = "TiendaEnemigos";

    [Tooltip("Tienda donde se seleccionan maldiciones.")]
    [SerializeField] private string curseShopSceneName = "SceneMaldiciones";

    [Header("Tienda normal")]
    [Tooltip("La tienda normal aparece cada cierta cantidad de niveles pares.")]
    [Min(1)]
    [SerializeField] private int levelsBetweenShopVisits = 2;

    [Header("Tienda de maldiciones")]
    [Tooltip("La tienda de maldiciones aparece cada 5 niveles.")]
    [Min(1)]
    [SerializeField] private int levelsBetweenCurseShopVisits = 5;

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
                "LevelProgressManager2D: no se encontro GiftProgressManager2D.",
                this
            );

            return false;
        }

        if (PlayerScenePersistence2D.Instance == null)
        {
            Debug.LogError(
                "No se encontro PlayerScenePersistence2D."
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

        giftManager.PrepareForNewLevel();

        bool generated = levelGenerator.GenerateLevel(level);

        if (!generated)
        {
            Debug.LogError("No se pudo generar el nivel " + level);
            return;
        }

        ResetPlayerPosition();

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
                "LevelProgressManager2D: no se encontro PlayerController2D."
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
    // TIENDA DE ENEMIGOS
    // ------------------------------------------------

    private bool ShouldVisitEnemyShop(int completedLevel)
    {
        int remainder = completedLevel % 10;

        return remainder == 1 ||
               remainder == 3 ||
               remainder == 7 ||
               remainder == 9;
    }

    // ------------------------------------------------
    // TIENDA NORMAL
    // ------------------------------------------------

    private bool ShouldVisitNormalShop(int completedLevel)
    {
        int interval = Mathf.Max(1, levelsBetweenShopVisits);

        return completedLevel % interval == 0;
    }

    // ------------------------------------------------
    // TIENDA DE MALDICIONES
    // ------------------------------------------------

    private bool ShouldVisitCurseShop(int completedLevel)
    {
        int interval = Mathf.Max(1, levelsBetweenCurseShopVisits);

        return completedLevel % interval == 0;
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
                "No se encontro PlayerScenePersistence2D."
            );

            return;
        }

        isTransitioning = true;

        int completedLevel = CurrentLevel;

        Debug.Log("Nivel completado: " + completedLevel);

        // Guardamos el siguiente nivel.
        PlayerScenePersistence2D.Instance.AdvanceToNextLevel();

        // PRIORIDAD 1: TIENDA DE MALDICIONES.
        if (ShouldVisitCurseShop(completedLevel))
        {
            Debug.Log(
                "Nivel " + completedLevel +
                ": entrando a la TIENDA DE MALDICIONES."
            );

            SceneManager.LoadScene(curseShopSceneName);
            return;
        }

        // PRIORIDAD 2: TIENDA DE ENEMIGOS.
        if (ShouldVisitEnemyShop(completedLevel))
        {
            Debug.Log(
                "Nivel " + completedLevel +
                ": entrando a la TIENDA DE ENEMIGOS."
            );

            SceneManager.LoadScene(enemyShopSceneName);
            return;
        }

        // PRIORIDAD 3: TIENDA NORMAL.
        if (ShouldVisitNormalShop(completedLevel))
        {
            Debug.Log(
                "Nivel " + completedLevel +
                ": entrando a la TIENDA NORMAL."
            );

            SceneManager.LoadScene(shopSceneName);
            return;
        }

        // Si no corresponde ninguna tienda, continuamos.
        Debug.Log(
            "Nivel " + completedLevel +
            ": no corresponde tienda. Generando siguiente nivel."
        );

        StartCoroutine(GenerateNextLevel());
    }

    // ------------------------------------------------
    // SIGUIENTE NIVEL EN LA MISMA ESCENA
    // ------------------------------------------------

    private IEnumerator GenerateNextLevel()
    {
        yield return null;

        if (!ValidateReferences())
            yield break;

        GenerateCurrentLevel();

        isTransitioning = false;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
