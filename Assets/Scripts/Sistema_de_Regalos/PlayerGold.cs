
using System;
using System.Globalization;
using UnityEngine;

public class PlayerGold : MonoBehaviour
{
    [Header("Oro inicial")]
    [SerializeField, Min(0)]
    private int startingGold = 0;

    [Header("Multiplicador base de oro")]
    [SerializeField, Min(0f)]
    private float goldMultiplier = 1f;

    // Bonificación acumulada de los artículos.
    private float upgradeMultiplierBonus = 0f;

    public float CurrentGold { get; private set; }

    // Multiplicador final: base + mejoras.
    public float GoldMultiplier =>
        Mathf.Max(0f, goldMultiplier + upgradeMultiplierBonus);

    public event Action<float> OnGoldChanged;

    private void Awake()
    {
        CurrentGold = startingGold;
    }

    // ------------------------------------------------
    // MULTIPLICADOR DE LOS ARTICULOS
    // ------------------------------------------------

    public void SetUpgradeMultiplierBonus(float bonus)
    {
        upgradeMultiplierBonus = Mathf.Max(0f, bonus);

        Debug.Log(
            "Multiplicador de oro actualizado: x" +
            GoldMultiplier.ToString(
                "0.##",
                CultureInfo.InvariantCulture
            )
        );

        // Actualiza el HUD aunque la cantidad de oro no cambie.
        OnGoldChanged?.Invoke(CurrentGold);
    }

    public float GetUpgradeMultiplierBonus()
    {
        return upgradeMultiplierBonus;
    }

    // ------------------------------------------------
    // AGREGAR ORO
    // ------------------------------------------------

    public void AddGold(int baseAmount)
    {
        if (baseAmount <= 0)
            return;

        float finalAmount = baseAmount * GoldMultiplier;

        CurrentGold += finalAmount;

        OnGoldChanged?.Invoke(CurrentGold);

        Debug.Log(
            "Oro obtenido: " +
            finalAmount.ToString("0.##", CultureInfo.InvariantCulture) +
            " | Multiplicador: x" +
            GoldMultiplier.ToString("0.##", CultureInfo.InvariantCulture) +
            " | Oro actual: " +
            CurrentGold.ToString("0.##", CultureInfo.InvariantCulture)
        );
    }

    // ------------------------------------------------
    // GASTAR ORO
    // ------------------------------------------------

    public bool SpendGold(float amount)
    {
        if (amount <= 0f)
            return false;

        if (CurrentGold < amount)
            return false;

        CurrentGold -= amount;

        OnGoldChanged?.Invoke(CurrentGold);

        Debug.Log(
            "Oro gastado: " + amount +
            " | Oro restante: " + CurrentGold
        );

        return true;
    }
}
