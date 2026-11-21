
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ExitDoor2D : MonoBehaviour
{
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

        spriteRenderer.color = Color.black;
    }

    public void Unlock()
    {
        if (IsUnlocked)
            return;

        IsUnlocked = true;

        spriteRenderer.color = Color.white;

        if (doorCollider != null)
            doorCollider.enabled = false;

        Debug.Log("¡La puerta está desbloqueada!");
    }

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
