
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

    private void Start()
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);

        if (exitDoor == null)
            exitDoor = GetComponentInParent<ExitDoor2D>();
    }

    private void Update()
    {
        if (!playerInside || exitDoor == null)
            return;

        if (!exitDoor.IsUnlocked)
            return;

        if (Input.GetKeyDown(interactionKey))
        {
            exitDoor.EnterDoor();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other))
            return;

        playerInside = true;

        UpdatePrompt();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other))
            return;

        playerInside = false;

        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    private void UpdatePrompt()
    {
        if (promptText == null || exitDoor == null)
            return;

        promptText.gameObject.SetActive(true);

        if (exitDoor.IsUnlocked)
        {
            promptText.text = "PRESIONA E PARA ENTRAR";
        }
        else
        {
            promptText.text = "PUERTA BLOQUEADA";
        }
    }

    private bool IsPlayer(Collider2D other)
    {
        return other.GetComponentInParent<PlayerGold>() != null;
    }
}
