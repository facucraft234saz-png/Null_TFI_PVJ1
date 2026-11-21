
using UnityEngine;
using TMPro;

public class ExitDoorPrompt2D : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private ExitDoor2D exitDoor;
    [SerializeField] private TMP_Text promptText;

    [Header("Interaccion")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    private bool playerInside;
    private bool previousUnlockedState;

    private void Start()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);

        if (exitDoor == null)
            exitDoor = GetComponentInParent<ExitDoor2D>();

        if (exitDoor != null)
            previousUnlockedState = exitDoor.IsUnlocked;

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopExitPromptLoop();
    }

    private void Update()
    {
        if (!playerInside || exitDoor == null)
            return;

        // Actualizamos el aviso cuando cambia el estado
        // de la puerta, por ejemplo al desbloquearse.
        if (previousUnlockedState != exitDoor.IsUnlocked)
        {
            previousUnlockedState = exitDoor.IsUnlocked;
            UpdatePrompt();
        }

        if (exitDoor.IsUnlocked &&
            Input.GetKeyDown(interactionKey))
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.StopExitPromptLoop();

            exitDoor.EnterDoor();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other))
            return;

        playerInside = true;

        if (exitDoor != null)
            previousUnlockedState = exitDoor.IsUnlocked;

        UpdatePrompt();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other))
            return;

        playerInside = false;

        if (promptText != null)
            promptText.gameObject.SetActive(false);

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopExitPromptLoop();
    }

    private void UpdatePrompt()
    {
        if (promptText == null || exitDoor == null)
            return;

        promptText.gameObject.SetActive(true);

        if (exitDoor.IsUnlocked)
        {
            promptText.text = "PRESIONA E PARA CONTINUAR";

            if (AudioManager.Instance != null)
                AudioManager.Instance.StartExitPromptLoop();
        }
        else
        {
            promptText.text = "PUERTA BLOQUEADA";

            if (AudioManager.Instance != null)
                AudioManager.Instance.StopExitPromptLoop();
        }
    }

    private bool IsPlayer(Collider2D other)
    {
        return other.GetComponentInParent<PlayerGold>() != null;
    }

    private void OnDisable()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopExitPromptLoop();
    }
}
