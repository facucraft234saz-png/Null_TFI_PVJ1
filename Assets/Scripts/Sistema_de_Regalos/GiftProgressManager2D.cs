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

    private readonly List<GameObject> spawnedGoldenGifts =
        new List<GameObject>();

    private ExitDoor2D spawnedDoor;
    private Coroutine initializationCoroutine;

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

    // ------------------------------------------------
    // PREPARAR NIVEL NUEVO
    // ------------------------------------------------

    public void PrepareForNewLevel()
    {
        if (initializationCoroutine != null)
        {
            StopCoroutine(initializationCoroutine);
            initializationCoroutine = null;
        }

        // Eliminamos los regalos negros que hayan quedado
        // del nivel anterior.
        BlackGift2D[] oldBlackGifts =
            FindObjectsByType<BlackGift2D>(
                FindObjectsSortMode.None
            );

        foreach (BlackGift2D gift in oldBlackGifts)
        {
            if (gift != null)
                Destroy(gift.gameObject);
        }

        // Eliminamos los regalos dorados del nivel anterior.
        foreach (GameObject gift in spawnedGoldenGifts)
        {
            if (gift != null)
                Destroy(gift);
        }

        spawnedGoldenGifts.Clear();

        // Eliminamos la puerta anterior.
        if (spawnedDoor != null)
        {
            Destroy(spawnedDoor.gameObject);
            spawnedDoor = null;
        }

        ResetProgress();
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
    }

    // ------------------------------------------------
    // INICIALIZAR DESPUÉS DE GENERAR PLATAFORMAS
    // ------------------------------------------------

    public void InitializeForNewLevel(
        Vector3 doorPosition,
        Quaternion doorRotation)
    {
        if (initializationCoroutine != null)
            StopCoroutine(initializationCoroutine);

        initializationCoroutine = StartCoroutine(
            InitializeAfterGeneration(doorPosition, doorRotation)
        );
    }

    private IEnumerator InitializeAfterGeneration(
        Vector3 doorPosition,
        Quaternion doorRotation)
    {
        // Esperamos a que los generadores de regalos
        // de las plataformas terminen de ejecutarse.
        yield return null;
        yield return new WaitForEndOfFrame();

        BlackGift2D[] blackGifts =
            FindObjectsByType<BlackGift2D>(
                FindObjectsSortMode.None
            );

        BlackGiftsMaximum = blackGifts.Length;

        Debug.Log(
            "Nivel inicializado. Regalos negros: " +
            BlackGiftsMaximum
        );

        SpawnDoor(doorPosition, doorRotation);

        if (BlackGiftsMaximum == 0)
        {
            Debug.LogWarning(
                "No hay regalos negros. Se desbloquea la puerta."
            );

            UnlockRewards();
        }

        initializationCoroutine = null;
    }

    // ------------------------------------------------
    // REGALOS NEGROS
    // ------------------------------------------------

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

    // ------------------------------------------------
    // DESBLOQUEAR RECOMPENSAS
    // ------------------------------------------------

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
            Debug.LogError(
                "GiftProgressManager2D: falta asignar Golden Gift Prefab."
            );

            return;
        }

        foreach (Vector3 position in blackGiftPositions)
        {
            GameObject gift = Instantiate(
                goldenGiftPrefab,
                position,
                Quaternion.identity
            );

            spawnedGoldenGifts.Add(gift);
            GoldenGiftsMaximum++;
        }

        Debug.Log(
            "Regalos dorados disponibles: " +
            GoldenGiftsMaximum
        );

        Debug.Log(
    "Regalos dorados disponibles: " +
    GoldenGiftsMaximum
);

        // Sonido cuando aparecen los regalos dorados.
        if (GoldenGiftsMaximum > 0 &&
            AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGoldenGiftsAppearSound();
        }
    }

    // ------------------------------------------------
    // PUERTA
    // ------------------------------------------------

    private void SpawnDoor(
        Vector3 position,
        Quaternion rotation)
    {
        if (doorPrefab == null)
        {
            Debug.LogError(
                "GiftProgressManager2D: falta asignar Door Prefab."
            );

            return;
        }

        GameObject door = Instantiate(
            doorPrefab,
            position,
            rotation
        );

        spawnedDoor = door.GetComponent<ExitDoor2D>();

        if (spawnedDoor == null)
        {
            Debug.LogError(
                "El prefab de la puerta no tiene ExitDoor2D."
            );

            Destroy(door);
            return;
        }

        Debug.Log("Puerta generada al final del nivel.");
    }

    private IEnumerator UnlockDoorAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        if (spawnedDoor != null)
            spawnedDoor.Unlock();
    }

    // ------------------------------------------------
    // REGALOS DORADOS
    // ------------------------------------------------

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