
using UnityEngine;

public class PlayerScenePersistence2D : MonoBehaviour
{
    private static PlayerScenePersistence2D instance;
    private static bool startNewRunRequested;

    [Header("Progreso de la partida")]
    [SerializeField, Min(1)]
    private int currentLevel = 1;

    private int shopVisits = 0;

    public static PlayerScenePersistence2D Instance => instance;
    public int CurrentLevel => currentLevel;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);

        if (startNewRunRequested)
        {
            ResetRunProgress();
            startNewRunRequested = false;
        }

        currentLevel = Mathf.Max(1, currentLevel);
    }

    // NUEVA PARTIDA
    public static void PrepareNewRun()
    {
        startNewRunRequested = true;

        if (instance != null)
        {
            instance.ResetRunProgress();
            startNewRunRequested = false;
        }
    }

    private void ResetRunProgress()
    {
        currentLevel = 1;
        shopVisits = 0;

        Debug.Log("Progreso reiniciado. Nivel inicial: 1.");
    }

    // FINALIZAR PARTIDA
    public static void EndRun()
    {
        startNewRunRequested = true;

        if (instance != null)
        {
            PlayerScenePersistence2D oldInstance = instance;
            instance = null;

            Destroy(oldInstance.gameObject);
        }
    }

    // NIVELES
    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public int AdvanceToNextLevel()
    {
        currentLevel++;

        Debug.Log("Comenzando nivel: " + currentLevel);

        return currentLevel;
    }

    // TIENDA
    public int RegisterShopVisitAndGetRerolls()
    {
        shopVisits++;

        return 2 + shopVisits;
    }
}
