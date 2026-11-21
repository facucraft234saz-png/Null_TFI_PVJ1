
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GoldenGift2D : MonoBehaviour
{
    [Header("Recompensa")]
    [Min(1)]
    [SerializeField] private int goldAmount = 1;

    private bool collected;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
            return;

        PlayerGold playerGold =
            other.GetComponentInParent<PlayerGold>();

        if (playerGold == null)
            return;

        collected = true;

        playerGold.AddGold(goldAmount);

        if (GiftProgressManager2D.Instance != null)
        {
            GiftProgressManager2D.Instance
                .RegisterGoldenGiftCollected();
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGoldenGiftSound();
        }

        Destroy(gameObject);
    }
}
