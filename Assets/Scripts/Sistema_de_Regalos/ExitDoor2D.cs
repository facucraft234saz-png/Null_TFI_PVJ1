
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ExitDoor2D : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D doorCollider;

    public bool IsUnlocked { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        doorCollider = GetComponent<Collider2D>();

        IsUnlocked = false;

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
        if (!IsUnlocked)
            return;

        if (LevelProgressManager2D.Instance == null)
        {
            Debug.LogError(
                "No se encontró LevelProgressManager2D."
            );

            return;
        }

        Debug.Log("Saliendo del nivel...");

        LevelProgressManager2D.Instance.CompleteCurrentLevel();
    }
}
