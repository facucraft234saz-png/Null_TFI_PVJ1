
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
        BindReferences();
    }

    private void Start()
    {
        BindReferences();

        if (playerGold != null)
            UpdateGold(playerGold.CurrentGold);

        UpdateGiftCounters();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BindReferences();

        if (playerGold != null)
            UpdateGold(playerGold.CurrentGold);

        UpdateGiftCounters();
    }

    private void BindReferences()
    {
        if (playerGold != null)
            playerGold.OnGoldChanged -= UpdateGold;

        playerGold = FindFirstObjectByType<PlayerGold>();
        giftManager = GiftProgressManager2D.Instance;

        if (playerGold != null)
        {
            playerGold.OnGoldChanged += UpdateGold;
            UpdateGold(playerGold.CurrentGold);
        }
    }

    private void Update()
    {
        if (giftManager == null)
            giftManager = GiftProgressManager2D.Instance;

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

        goldText.text =
            "ORO: " + goldValue + " (x" + multiplier + ")";
    }

    private void UpdateGiftCounters()
    {
        if (giftManager == null)
            return;

        bool goldenUnlocked = giftManager.IsUnlocked;

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

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;

        if (playerGold != null)
            playerGold.OnGoldChanged -= UpdateGold;
    }
}
