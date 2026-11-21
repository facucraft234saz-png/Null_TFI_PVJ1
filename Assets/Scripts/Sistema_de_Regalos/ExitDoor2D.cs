
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ExitDoor2D : MonoBehaviour
{
    [Header("Sprites de la puerta")]
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openSprite;

    [Header("Configuración de animación")]
    [Tooltip("Intervalo de actualización visual. 0 significa cambio inmediato.")]
    [SerializeField] private float spriteChangeInterval = 0f;

    private SpriteRenderer spriteRenderer;
    private Collider2D doorCollider;

    private bool levelCompleted;

    public bool IsUnlocked { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        doorCollider = GetComponent<Collider2D>();

        IsUnlocked = false;
        levelCompleted = false;

        // La puerta comienza cerrada.
        ShowClosedSprite();
    }

    // --------------------------------------------------
    // SPRITE DE PUERTA CERRADA
    // --------------------------------------------------

    private void ShowClosedSprite()
    {
        if (closedSprite != null)
        {
            spriteRenderer.sprite = closedSprite;
        }
        else
        {
            Debug.LogWarning(
                "ExitDoor2D: no se asignó Closed Sprite.",
                this
            );
        }
    }

    // --------------------------------------------------
    // SPRITE DE PUERTA ABIERTA
    // --------------------------------------------------

    private void ShowOpenSprite()
    {
        if (openSprite != null)
        {
            spriteRenderer.sprite = openSprite;
        }
        else
        {
            Debug.LogWarning(
                "ExitDoor2D: no se asignó Open Sprite.",
                this
            );
        }
    }

    // --------------------------------------------------
    // DESBLOQUEAR PUERTA
    // --------------------------------------------------

    public void Unlock()
    {
        if (IsUnlocked)
            return;

        IsUnlocked = true;

        // Cambiamos al sprite de puerta abierta.
        ShowOpenSprite();

        // Conservamos el comportamiento original.
        if (doorCollider != null)
            doorCollider.enabled = false;

        Debug.Log("¡La puerta está desbloqueada!");
    }

    // --------------------------------------------------
    // ENTRAR A LA PUERTA
    // --------------------------------------------------

    public void EnterDoor()
    {
        if (!IsUnlocked || levelCompleted)
            return;

        if (LevelProgressManager2D.Instance == null)
        {
            Debug.LogError(
                "No se encontró LevelProgressManager2D."
            );

            return;
        }

        levelCompleted = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayExitDoorSound();
            AudioManager.Instance.StopExitPromptLoop();
        }

        Debug.Log("Saliendo del nivel...");

        LevelProgressManager2D.Instance.CompleteCurrentLevel();
    }
}
