
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftProgressManager2D : MonoBehaviour
{
    public static GiftProgressManager2D Instance { get; private set; }

    [Header("Regalo dorado")]
    [SerializeField] private GameObject goldenGiftPrefab;

    [Header("Puerta")]
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private Transform doorSpawnPoint;

    [Header("Progreso")]
    public int BlackGiftsCollected { get; private set; }
    public int BlackGiftsMaximum { get; private set; }

    public int GoldenGiftsCollected { get; private set; }
    public int GoldenGiftsMaximum { get; private set; }

    public bool IsUnlocked { get; private set; }

    private readonly List<Vector3> blackGiftPositions =
        new List<Vector3>();

    private readonly HashSet<BlackGift2D> collectedGifts =
        new HashSet<BlackGift2D>();

    private ExitDoor2D spawnedDoor;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        ResetProgress();
    }

    private void Start()
    {
        StartCoroutine(InitializeAfterGeneration());
    }

    private void ResetProgress()
    {
        BlackGiftsCollected = 0;
        BlackGiftsMaximum = 0;

        GoldenGiftsCollected = 0;
        GoldenGiftsMaximum = 0;

        IsUnlocked = false;

        blackGiftPositions.Clear();
        collectedGifts.Clear();

        spawnedDoor = null;
    }

    private IEnumerator InitializeAfterGeneration()
    {
        // Esperamos a que Start() de los generadores termine.
        yield return null;

        // Esperamos al final del frame para contar los objetos creados.
        yield return new WaitForEndOfFrame();

        BlackGift2D[] blackGifts =
            FindObjectsByType<BlackGift2D>(
                FindObjectsSortMode.None
            );

        BlackGiftsMaximum = blackGifts.Length;

        Debug.Log(
            "Nuevo nivel inicializado. Regalos negros: " +
            BlackGiftsMaximum
        );

        SpawnDoor();

        if (BlackGiftsMaximum == 0)
        {
            Debug.LogWarning(
                "No se encontraron regalos negros. Revisá los generadores."
            );
        }
    }

    public void RegisterBlackGiftCollected(BlackGift2D gift)
    {
        if (IsUnlocked || gift == null)
            return;

        if (!collectedGifts.Add(gift))
            return;

        blackGiftPositions.Add(gift.transform.position);
        BlackGiftsCollected++;

        Debug.Log(
            "Regalos negros: " +
            BlackGiftsCollected + "/" +
            BlackGiftsMaximum
        );

        if (BlackGiftsCollected >= BlackGiftsMaximum &&
            BlackGiftsMaximum > 0)
        {
            UnlockRewards();
        }
    }

    private void UnlockRewards()
    {
        if (IsUnlocked)
            return;

        IsUnlocked = true;

        SpawnGoldenGifts();

        StartCoroutine(UnlockDoorAfterDelay());

        Debug.Log("¡Todos los regalos negros fueron recolectados!");
    }

    private void SpawnGoldenGifts()
    {
        if (goldenGiftPrefab == null)
        {
            Debug.LogError("Falta asignar Golden Gift Prefab.");
            return;
        }

        foreach (Vector3 position in blackGiftPositions)
        {
            Instantiate(
                goldenGiftPrefab,
                position,
                Quaternion.identity
            );

            GoldenGiftsMaximum++;
        }

        Debug.Log(
            "Regalos dorados disponibles: " +
            GoldenGiftsMaximum
        );
    }

    private void SpawnDoor()
    {
        if (doorPrefab == null)
        {
            Debug.LogError("Falta asignar Door Prefab.");
            return;
        }

        if (doorSpawnPoint == null)
        {
            Debug.LogError("Falta asignar Door Spawn Point.");
            return;
        }

        GameObject door = Instantiate(
            doorPrefab,
            doorSpawnPoint.position,
            doorSpawnPoint.rotation
        );

        spawnedDoor = door.GetComponent<ExitDoor2D>();

        if (spawnedDoor == null)
        {
            Debug.LogError(
                "El prefab de la puerta no tiene ExitDoor2D."
            );
        }
        else
        {
            Debug.Log("Puerta generada para este nivel.");
        }
    }

    private IEnumerator UnlockDoorAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        if (spawnedDoor != null)
            spawnedDoor.Unlock();
    }

    public void RegisterGoldenGiftCollected()
    {
        if (!IsUnlocked)
            return;

        if (GoldenGiftsCollected >= GoldenGiftsMaximum)
            return;

        GoldenGiftsCollected++;

        Debug.Log(
            "Regalos dorados: " +
            GoldenGiftsCollected + "/" +
            GoldenGiftsMaximum
        );
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
