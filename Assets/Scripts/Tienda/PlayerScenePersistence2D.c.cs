
using UnityEngine;

public class PlayerScenePersistence2D : MonoBehaviour
{
    private static PlayerScenePersistence2D instance;

    [Header("Progreso de la partida")]
    [SerializeField, Min(1)]
    private int currentLevel = 1;

    // Cantidad total de veces que se entró a la tienda.
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

        currentLevel = Mathf.Max(1, currentLevel);
    }

    // ------------------------------------------------
    // NIVELES
    // ------------------------------------------------

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

    // ------------------------------------------------
    // TIENDA
    // ------------------------------------------------

    public int RegisterShopVisitAndGetRerolls()
    {
        shopVisits++;

        // 2 usos iniciales + 1 por cada visita.
        return 2 + shopVisits;
    }
}
