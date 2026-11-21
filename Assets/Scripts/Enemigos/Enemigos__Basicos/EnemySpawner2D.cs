
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner2D : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PlayerVitals playerVitals;

    [Header("Escenas")]
    [SerializeField] private string shopSceneName = "Tienda";

    private bool initialized;
    private int lastInitializedSceneHandle = -1;

    private void Awake()
    {
        FindPlayerReferences();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;

        if (playerInventory != null)
            playerInventory.OnEntryAdded += HandleEntryAdded;
    }

    private void Start()
    {
        InitializeForScene(SceneManager.GetActiveScene());
    }

    private void FindPlayerReferences()
    {
        if (playerInventory == null)
            playerInventory = GetComponent<PlayerInventory>();

        if (playerVitals == null)
            playerVitals = GetComponent<PlayerVitals>();

        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();

        if (playerVitals == null)
            playerVitals = FindFirstObjectByType<PlayerVitals>();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeForScene(scene);
    }

    private void InitializeForScene(Scene scene)
    {
        // No invocamos enemigos dentro de la tienda.
        if (scene.name == shopSceneName)
        {
            initialized = false;
            return;
        }

        // Evitamos inicializar dos veces la misma carga.
        if (scene.handle == lastInitializedSceneHandle)
            return;

        lastInitializedSceneHandle = scene.handle;
        initialized = false;

        FindPlayerReferences();

        if (playerInventory == null || playerVitals == null)
        {
            Debug.LogError(
                "EnemySpawner2D: no se encontraron PlayerInventory o PlayerVitals.",
                this
            );
            return;
        }

        Debug.Log("Reinicializando enemigos del inventario para: " + scene.name);

        foreach (InventoryStack stack in playerInventory.Entries)
        {
            if (stack == null || stack.data == null)
                continue;

            InventoryEntryData entry = stack.data;

            if (entry.category == InventoryCategory.Enemy)
            {
                if (entry.enemyDefinition == null)
                    continue;

                for (int i = 0; i < stack.amount; i++)
                {
                    StartCoroutine(
                        SpawnEnemyAfterDelay(entry.enemyDefinition)
                    );
                }
            }
            else if (entry.category == InventoryCategory.Curse)
            {
                int curseStacks = Mathf.Min(
                    stack.amount,
                    Mathf.Max(1, entry.maxStack)
                );

                ProcessCurseAdditionalEnemies(
                    entry,
                    curseStacks
                );
            }
        }

        initialized = true;
    }

    private void HandleEntryAdded(
        InventoryEntryData entry,
        int currentAmount)
    {
        if (!initialized || entry == null)
            return;

        if (entry.category == InventoryCategory.Enemy)
        {
            if (entry.enemyDefinition == null)
                return;

            StartCoroutine(
                SpawnEnemyAfterDelay(entry.enemyDefinition)
            );
        }
        else if (entry.category == InventoryCategory.Curse)
        {
            if (currentAmount > Mathf.Max(1, entry.maxStack))
                return;

            ProcessCurseAdditionalEnemies(entry, 1);
        }
    }

    private void ProcessCurseAdditionalEnemies(
        InventoryEntryData curse,
        int curseStacks)
    {
        if (curse == null || curse.additionalEnemies == null)
            return;

        foreach (CurseAdditionalEnemy additional in curse.additionalEnemies)
        {
            if (additional == null)
                continue;

            EnemyDefinition definition = additional.enemyDefinition;

            if (definition == null)
                continue;

            if (definition.enemyType != additional.enemyType)
            {
                Debug.LogWarning(
                    "El tipo seleccionado no coincide con EnemyDefinition: " +
                    definition.enemyName
                );
                continue;
            }

            int totalAmount =
                Mathf.Max(1, additional.amount) * curseStacks;

            for (int i = 0; i < totalAmount; i++)
            {
                StartCoroutine(
                    SpawnEnemyAfterDelay(definition)
                );
            }
        }
    }

    private IEnumerator SpawnEnemyAfterDelay(
        EnemyDefinition definition)
    {
        if (definition == null)
            yield break;

        EnemyRuntimeStats stats = new EnemyRuntimeStats(
            definition,
            playerInventory
        );

        if (stats.spawnDelay > 0f)
        {
            yield return new WaitForSeconds(stats.spawnDelay);
        }

        if (playerVitals == null || playerVitals.IsGameOver)
            yield break;

        SpawnEnemy(definition, stats);
    }

    private void SpawnEnemy(
        EnemyDefinition definition,
        EnemyRuntimeStats stats)
    {
        if (definition == null || definition.enemyPrefab == null)
        {
            Debug.LogWarning(
                "EnemySpawner2D: falta asignar el prefab."
            );
            return;
        }

        float randomAngle = Random.Range(0f, 360f);

        Vector2 direction = new Vector2(
            Mathf.Cos(randomAngle * Mathf.Deg2Rad),
            Mathf.Sin(randomAngle * Mathf.Deg2Rad)
        );

        Vector2 spawnPosition =
            (Vector2)transform.position +
            direction * stats.spawnDistance;

        GameObject enemy = Instantiate(
            definition.enemyPrefab,
            spawnPosition,
            Quaternion.identity
        );

        if (definition.enemyType == EnemyType.Basic)
        {
            BasicEnemyFollower2D follower =
                enemy.GetComponent<BasicEnemyFollower2D>();

            if (follower == null)
            {
                Debug.LogError(
                    "El prefab EnemyBasic no tiene BasicEnemyFollower2D.",
                    enemy
                );

                Destroy(enemy);
                return;
            }

            follower.Configure(
                transform,
                playerVitals,
                definition,
                stats
            );
        }
        else if (definition.enemyType == EnemyType.Distance)
        {
            DistanceEnemyFollower2D follower =
                enemy.GetComponent<DistanceEnemyFollower2D>();

            if (follower == null)
            {
                Debug.LogError(
                    "El prefab EnemyDistance no tiene DistanceEnemyFollower2D.",
                    enemy
                );

                Destroy(enemy);
                return;
            }

            follower.Configure(
                transform,
                playerVitals,
                definition,
                stats
            );
        }

        Debug.Log("Enemigo invocado: " + definition.enemyName);
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;

        if (playerInventory != null)
            playerInventory.OnEntryAdded -= HandleEntryAdded;

        StopAllCoroutines();
    }
}
