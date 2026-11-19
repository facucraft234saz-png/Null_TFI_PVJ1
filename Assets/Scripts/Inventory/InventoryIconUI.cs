
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryIconUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text amountText;

    private static Sprite placeholderSprite;

    private void Awake()
    {
        if (placeholderSprite == null)
        {
            placeholderSprite = Sprite.Create(
                Texture2D.whiteTexture,
                new Rect(0, 0, 1, 1),
                new Vector2(0.5f, 0.5f)
            );
        }
    }

    public void Configure(InventoryEntryData data, int amount)
    {
        if (data == null || iconImage == null)
            return;

        if (data.icon != null)
        {
            iconImage.sprite = data.icon;
            iconImage.color = Color.white;
        }
        else
        {
            iconImage.sprite = placeholderSprite;
            iconImage.color = data.placeholderColor;
        }

        // Mostrar cantidad solamente si es mayor a uno.
        if (amountText != null)
        {
            amountText.gameObject.SetActive(amount > 1);

            if (amount > 1)
            {
                amountText.text = amount.ToString();
            }
        }

        gameObject.name = "Icon_" + data.entryName;
    }
}
