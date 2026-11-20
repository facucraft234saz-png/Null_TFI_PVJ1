
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ExitDoor2D : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D doorCollider;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        doorCollider = GetComponent<Collider2D>();

        spriteRenderer.color = Color.black;
    }

    public void Unlock()
    {
        spriteRenderer.color = Color.white;

        if (doorCollider != null)
            doorCollider.enabled = false;

        Debug.Log("¡La puerta está desbloqueada!");
    }
}
