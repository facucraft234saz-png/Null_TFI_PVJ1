
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController2D))]
[RequireComponent(typeof(PlayerVitals))]
[RequireComponent(typeof(PlayerInventory))]
public class PlayerItemEffects : MonoBehaviour
{
    [Header("Configuracion base")]
    [SerializeField] private PlayerStats baseStats;

    private PlayerController2D controller;
    private PlayerVitals vitals;
    private PlayerInventory inventory;

    // Cantidad de mejoras que ya fueron aplicadas.
    private Dictionary<InventoryEntryData, int> itemStacks =
        new Dictionary<InventoryEntryData, int>();

    public event Action<InventoryEntryData, int> OnUpgradeApplied;

    private void Awake()
    {
        controller = GetComponent<PlayerController2D>();
        vitals = GetComponent<PlayerVitals>();
        inventory = GetComponent<PlayerInventory>();

        if (baseStats == null)
        {
            Debug.LogError(
                "PlayerItemEffects: falta asignar PlayerStats.",
                this
            );

            enabled = false;
        }
    }

    private void OnEnable()
    {
        if (inventory != null)
            inventory.OnEntryAdded += HandleEntryAdded;
    }

    private void Start()
    {
        if (!enabled || inventory == null)
            return;

        // Aplicamos los elementos que ya estaban configurados
        // en Starting Entries antes de comenzar la partida.
        foreach (InventoryStack stack in inventory.Entries)
        {
            if (stack.data == null)
                continue;

            SynchronizeEntry(stack.data, stack.amount);
        }

        RecalculateMovementStats();

        Debug.Log("Mejoras iniciales del jugador aplicadas.");
    }

    private void OnDisable()
    {
        if (inventory != null)
            inventory.OnEntryAdded -= HandleEntryAdded;
    }

    // ------------------------------------------------
    // NUEVOS ELEMENTOS DEL INVENTARIO
    // ------------------------------------------------

    private void HandleEntryAdded(
        InventoryEntryData item,
        int currentAmount)
    {
        SynchronizeEntry(item, currentAmount);
        RecalculateMovementStats();
    }

    private void SynchronizeEntry(
        InventoryEntryData item,
        int inventoryAmount)
    {
        if (item == null)
            return;

        int maximum = Mathf.Max(1, item.maxStack);

        int targetAmount = Mathf.Min(
            inventoryAmount,
            maximum
        );

        int alreadyApplied = GetStack(item);

        // No aplicamos dos veces una mejora que ya existe.
        while (alreadyApplied < targetAmount)
        {
            ApplySingleUpgrade(item);
            alreadyApplied++;
        }
    }

    // ------------------------------------------------
    // APLICAR UNA MEJORA
    // ------------------------------------------------

    private void ApplySingleUpgrade(InventoryEntryData item)
    {
        if (item == null)
            return;

        if (item.upgradeType == PlayerUpgradeType.None)
        {
            Debug.LogWarning(
                "El item " + item.entryName +
                " no tiene una mejora configurada."
            );

            return;
        }

        int currentStack = GetStack(item);

        if (currentStack >= Mathf.Max(1, item.maxStack))
            return;

        itemStacks[item] = currentStack + 1;

        switch (item.upgradeType)
        {
            case PlayerUpgradeType.AddHearts:
                vitals.AddHearts(
                    Mathf.Max(1, Mathf.RoundToInt(item.upgradeValue))
                );
                break;

            case PlayerUpgradeType.AddShields:
                vitals.AddShields(
                    Mathf.Max(1, Mathf.RoundToInt(item.upgradeValue))
                );
                break;
        }

        int newStack = itemStacks[item];

        Debug.Log(
            "Mejora aplicada: " + item.entryName +
            " (" + newStack + "/" + item.maxStack + ")"
        );

        OnUpgradeApplied?.Invoke(item, newStack);
    }

    public int GetStack(InventoryEntryData item)
    {
        if (item == null)
            return 0;

        return itemStacks.TryGetValue(item, out int amount)
            ? amount
            : 0;
    }

    // ------------------------------------------------
    // RECALCULAR ATRIBUTOS
    // ------------------------------------------------

    private void RecalculateMovementStats()
    {
        float speedBonusPercent = 0f;
        float accelerationReduction = 0f;
        float decelerationReduction = 0f;
        float gravityChange = 0f;
        float jumpForceChange = 0f;

        int extraJumps = 0;

        foreach (KeyValuePair<InventoryEntryData, int> pair
            in itemStacks)
        {
            InventoryEntryData item = pair.Key;
            int amount = pair.Value;

            if (item == null)
                continue;

            float totalValue = item.upgradeValue * amount;

            switch (item.upgradeType)
            {
                case PlayerUpgradeType.MoveSpeedPercent:
                    speedBonusPercent += totalValue;
                    break;

                case PlayerUpgradeType.AccelerationTimeReduction:
                    accelerationReduction += totalValue;
                    break;

                case PlayerUpgradeType.DecelerationTimeReduction:
                    decelerationReduction += totalValue;
                    break;

                case PlayerUpgradeType.GravityChange:
                    gravityChange += totalValue;
                    break;

                case PlayerUpgradeType.JumpForceChange:
                    jumpForceChange += totalValue;
                    break;

                case PlayerUpgradeType.ExtraJumps:
                    extraJumps += Mathf.RoundToInt(totalValue);
                    break;
            }
        }

        // Velocidad: suma porcentajes acumulados.
        float finalSpeed =
            baseStats.maxSpeed * (1f + speedBonusPercent);

        // Aceleración y desaceleración.
        // Llegar a cero significa movimiento instantáneo.
        float finalAcceleration = Mathf.Max(
            0f,
            baseStats.accelerationTime - accelerationReduction
        );

        float finalDeceleration = Mathf.Max(
            0f,
            baseStats.decelerationTime - decelerationReduction
        );

        // Gravedad y fuerza del salto.
        float finalGravity = Mathf.Max(
            0f,
            baseStats.gravityScale + gravityChange
        );

        float finalJumpForce = Mathf.Max(
            0f,
            baseStats.jumpForce + jumpForceChange
        );

        // Cantidad total de saltos.
        int finalJumps = Mathf.Max(
            1,
            baseStats.maxJumps + extraJumps
        );

        controller.SetMaxSpeed(finalSpeed);
        controller.SetAccelerationTime(finalAcceleration);
        controller.SetDecelerationTime(finalDeceleration);
        controller.SetGravity(finalGravity);
        controller.SetJumpForce(finalJumpForce);
        controller.SetMaxJumps(finalJumps);

        Debug.Log(
            "ATRIBUTOS ACTUALIZADOS\n" +
            "Velocidad: " + finalSpeed +
            "\nAceleración: " + finalAcceleration +
            "\nDesaceleración: " + finalDeceleration +
            "\nGravedad: " + finalGravity +
            "\nFuerza de salto: " + finalJumpForce +
            "\nSaltos máximos: " + finalJumps
        );
    }
}

