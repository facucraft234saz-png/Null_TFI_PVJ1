
using UnityEngine;

public class PlayerScenePersistence2D : MonoBehaviour
{
    private static PlayerScenePersistence2D instance;

    // Cantidad total de veces que se entró a la tienda.
    private int shopVisits = 0;

    public static PlayerScenePersistence2D Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int RegisterShopVisitAndGetRerolls()
    {
        shopVisits++;

        // 2 usos iniciales + 1 por cada visita.
        return 2 + shopVisits;
    }
}
