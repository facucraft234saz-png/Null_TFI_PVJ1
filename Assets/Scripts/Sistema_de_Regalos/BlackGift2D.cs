
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BlackGift2D : MonoBehaviour
{
    private bool collected;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
            return;

        // Verificamos que quien lo recoja sea el jugador.
        PlayerVitals player =
            other.GetComponentInParent<PlayerVitals>();

        if (player == null)
            return;

        if (GiftProgressManager2D.Instance == null)
        {
            Debug.LogError("No existe GiftProgressManager2D.");
            return;
        }

        collected = true;

        GiftProgressManager2D.Instance.RegisterBlackGiftCollected(this);

        Destroy(gameObject);
    }
}
