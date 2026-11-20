
using System;
using System.Globalization;
using UnityEngine;

public class PlayerGold : MonoBehaviour
{
    [Header("Oro inicial")]
    [SerializeField, Min(0)]
    private int startingGold = 0;

    [Header("Multiplicador de oro")]
    [SerializeField, Min(0f)]
    private float goldMultiplier = 1f;

    public float CurrentGold { get; private set; }

    public float GoldMultiplier => goldMultiplier;

    public event Action<float> OnGoldChanged;

    private void Awake()
    {
        CurrentGold = startingGold;
    }

    public void AddGold(int baseAmount)
    {
        if (baseAmount <= 0)
            return;

        float finalAmount = baseAmount * goldMultiplier;

        CurrentGold += finalAmount;

        OnGoldChanged?.Invoke(CurrentGold);

        Debug.Log(
            "Oro obtenido: " + finalAmount.ToString("0.##", CultureInfo.InvariantCulture) +
            " | Oro actual: " + CurrentGold.ToString("0.##", CultureInfo.InvariantCulture)
        );
    }
}
