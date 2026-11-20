
using UnityEngine;
using TMPro;
using System.Globalization;

public class GiftHUD : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerGold playerGold;
    [SerializeField] private GiftProgressManager2D giftManager;

    [Header("HUD superior")]
    [SerializeField] private TMP_Text goldText;

    [Header("HUD inferior")]
    [SerializeField] private TMP_Text blackGiftText;
    [SerializeField] private TMP_Text goldenGiftText;

    private void Start()
    {
        if (playerGold == null)
        {
            Debug.LogError("GiftHUD: falta PlayerGold.");
            return;
        }

        if (giftManager == null)
            giftManager = GiftProgressManager2D.Instance;

        playerGold.OnGoldChanged += UpdateGold;

        UpdateGold(playerGold.CurrentGold);
        UpdateGiftCounters();
    }

    private void Update()
    {
        if (giftManager == null)
            giftManager = GiftProgressManager2D.Instance;

        if (giftManager == null)
            return;

        UpdateGiftCounters();
    }

    private void UpdateGold(float amount)
    {
        if (goldText == null || playerGold == null)
            return;

        string goldValue = amount.ToString(
            "0.##",
            CultureInfo.InvariantCulture
        );

        string multiplier = playerGold.GoldMultiplier.ToString(
            "0.0",
            CultureInfo.InvariantCulture
        );

        goldText.text = "ORO: " + goldValue + " (x" + multiplier + ")";
    }

    private void UpdateGiftCounters()
    {
        if (giftManager == null)
            return;

        bool goldenUnlocked = giftManager.IsUnlocked;

        // Solo se muestra un contador a la vez.
        if (blackGiftText != null)
            blackGiftText.gameObject.SetActive(!goldenUnlocked);

        if (goldenGiftText != null)
            goldenGiftText.gameObject.SetActive(goldenUnlocked);

        if (!goldenUnlocked)
        {
            if (blackGiftText != null)
            {
                blackGiftText.text =
                    "REGALOS NEGROS: " +
                    giftManager.BlackGiftsCollected + "/" +
                    giftManager.BlackGiftsMaximum;
            }
        }
        else
        {
            if (goldenGiftText != null)
            {
                goldenGiftText.text =
                    "REGALOS DORADOS: " +
                    giftManager.GoldenGiftsCollected + "/" +
                    giftManager.GoldenGiftsMaximum;
            }
        }
    }

    private void OnDestroy()
    {
        if (playerGold != null)
            playerGold.OnGoldChanged -= UpdateGold;
    }
}
