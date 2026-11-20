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
        // Buscamos todos los regalos negros activos de la escena.
        BlackGift2D[] blackGifts =
            FindObjectsByType<BlackGift2D>(FindObjectsSortMode.None);

        BlackGiftsMaximum = blackGifts.Length;

        Debug.Log(
            "Regalos negros encontrados: " + BlackGiftsMaximum
        );

        if (BlackGiftsMaximum == 0)
        {
            Debug.LogWarning(
                "No se encontraron regalos negros en la escena."
            );
        }
    }

    public void RegisterBlackGiftCollected(BlackGift2D gift)
    {
        if (IsUnlocked || gift == null)
            return;

        blackGiftPositions.Add(gift.transform.position);

        BlackGiftsCollected++;

        Debug.Log(
            "Regalos negros: " +
            BlackGiftsCollected + "/" + BlackGiftsMaximum
        );

        if (BlackGiftsCollected >= BlackGiftsMaximum)
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
        SpawnDoor();

        Debug.Log("¡Todos los regalos negros fueron recolectados!");
    }

    private void SpawnGoldenGifts()
    {
        if (goldenGiftPrefab == null)
        {
            Debug.LogError(
                "Falta asignar Golden Gift Prefab."
            );
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

        ExitDoor2D doorScript = door.GetComponent<ExitDoor2D>();

        if (doorScript != null)
        {
            StartCoroutine(UnlockDoorAfterDelay(doorScript));
        }
        else
        {
            Debug.LogError(
                "El prefab de la puerta no tiene ExitDoor2D."
            );
        }
    }

    private IEnumerator UnlockDoorAfterDelay(ExitDoor2D door)
    {
        yield return new WaitForSeconds(0.5f);

        if (door != null)
            door.Unlock();
    }

    public void RegisterGoldenGiftCollected()
    {
        if (!IsUnlocked)
            return;

        GoldenGiftsCollected++;

        Debug.Log(
            "Regalos dorados: " +
            GoldenGiftsCollected + "/" + GoldenGiftsMaximum
        );
    }
}