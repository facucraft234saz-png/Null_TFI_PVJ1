
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerController2D))]
[RequireComponent(typeof(PlayerVitals))]
[RequireComponent(typeof(PlayerInventory))]
[RequireComponent(typeof(PlayerGold))]
public class PlayerItemEffects : MonoBehaviour
{
    [Header("Configuracion base")]
    [SerializeField] private PlayerStats baseStats;

    private PlayerController2D controller;
    private PlayerVitals vitals;
    private PlayerInventory inventory;
    private PlayerGold playerGold;
    private CameraFollow2D cameraFollow;

    // Cantidad de stacks cuyas mejoras ya fueron procesadas.
    private Dictionary<InventoryEntryData, int> itemStacks =
        new Dictionary<InventoryEntryData, int>();

    public event Action<InventoryEntryData, int> OnUpgradeApplied;

    private void Awake()
    {
        controller = GetComponent<PlayerController2D>();
        vitals = GetComponent<PlayerVitals>();
        inventory = GetComponent<PlayerInventory>();
        playerGold = GetComponent<PlayerGold>();

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

        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void Start()
    {
        if (!enabled || inventory == null)
            return;

        FindCamera();

        // Procesamos todos los artículos que ya tiene el jugador.
        foreach (InventoryStack stack in inventory.Entries)
        {
            if (stack == null || stack.data == null)
                continue;

            SynchronizeEntry(stack.data, stack.amount);
        }

        RecalculatePlayerStats();

        Debug.Log("Mejoras iniciales del jugador aplicadas.");
    }

    private void OnDisable()
    {
        if (inventory != null)
            inventory.OnEntryAdded -= HandleEntryAdded;

        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    // ------------------------------------------------
    // CAMBIO DE ESCENA
    // ------------------------------------------------

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindCamera();
        RecalculateCameraStats();
    }

    private void FindCamera()
    {
        cameraFollow = FindFirstObjectByType<CameraFollow2D>();

        if (cameraFollow != null)
            RecalculateCameraStats();
    }

    // ------------------------------------------------
    // NUEVOS ELEMENTOS DEL INVENTARIO
    // ------------------------------------------------

    private void HandleEntryAdded(
        InventoryEntryData item,
        int currentAmount)
    {
        SynchronizeEntry(item, currentAmount);
        RecalculatePlayerStats();
    }

    private void SynchronizeEntry(
        InventoryEntryData item,
        int inventoryAmount)
    {
        if (item == null || item.category != InventoryCategory.Item)
            return;

        int maximum = Mathf.Max(1, item.maxStack);

        int targetAmount = Mathf.Min(
            inventoryAmount,
            maximum
        );

        int alreadyApplied = GetStack(item);

        // Aplicamos únicamente los stacks nuevos.
        while (alreadyApplied < targetAmount)
        {
            alreadyApplied++;

            itemStacks[item] = alreadyApplied;

            ApplyVitalEffects(item);

            OnUpgradeApplied?.Invoke(item, alreadyApplied);
        }
    }

    // ------------------------------------------------
    // OBTENER LAS MEJORAS DEL ARTICULO
    // ------------------------------------------------

    private List<PlayerUpgradeEffect> GetConfiguredEffects(
        InventoryEntryData item)
    {
        List<PlayerUpgradeEffect> effects =
            new List<PlayerUpgradeEffect>();

        if (item == null)
            return effects;

        // Preferimos la nueva lista de mejoras.
        if (item.upgradeEffects != null &&
            item.upgradeEffects.Count > 0)
        {
            foreach (PlayerUpgradeEffect effect in item.upgradeEffects)
            {
                if (effect != null &&
                    effect.upgradeType != PlayerUpgradeType.None)
                {
                    effects.Add(effect);
                }
            }

            return effects;
        }

        // Compatibilidad con artículos anteriores.
        if (item.upgradeType != PlayerUpgradeType.None)
        {
            effects.Add(new PlayerUpgradeEffect
            {
                upgradeType = item.upgradeType,
                upgradeValue = item.upgradeValue
            });
        }

        return effects;
    }

    // ------------------------------------------------
    // EFECTOS DE VIDA Y ESCUDOS
    // ------------------------------------------------

    private void ApplyVitalEffects(InventoryEntryData item)
    {
        List<PlayerUpgradeEffect> effects =
            GetConfiguredEffects(item);

        foreach (PlayerUpgradeEffect effect in effects)
        {
            int amount = Mathf.RoundToInt(effect.upgradeValue);

            if (amount <= 0)
                continue;

            switch (effect.upgradeType)
            {
                case PlayerUpgradeType.AddHearts:
                    vitals.AddHearts(amount);
                    break;

                case PlayerUpgradeType.AddShields:
                    vitals.AddShields(amount);
                    break;
            }
        }
    }

    // ------------------------------------------------
    // CONSULTAR STACKS
    // ------------------------------------------------

    public int GetStack(InventoryEntryData item)
    {
        if (item == null)
            return 0;

        return itemStacks.TryGetValue(item, out int amount)
            ? amount
            : 0;
    }

    // ------------------------------------------------
    // RECALCULAR ATRIBUTOS DEL JUGADOR
    // ------------------------------------------------

    private void RecalculatePlayerStats()
    {
        float speedBonusPercent = 0f;
        float accelerationReduction = 0f;
        float decelerationReduction = 0f;
        float gravityChange = 0f;
        float jumpForceChange = 0f;

        int extraJumps = 0;

        // Nueva mejora: multiplicador de oro.
        float goldMultiplierBonus = 0f;

        foreach (KeyValuePair<InventoryEntryData, int> pair in itemStacks)
        {
            InventoryEntryData item = pair.Key;
            int amount = pair.Value;

            if (item == null)
                continue;

            List<PlayerUpgradeEffect> effects =
                GetConfiguredEffects(item);

            foreach (PlayerUpgradeEffect effect in effects)
            {
                float totalValue = effect.upgradeValue * amount;

                switch (effect.upgradeType)
                {
                    case PlayerUpgradeType.MoveSpeedPercent:
                        // 0.10 equivale a un 10%.
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

                    case PlayerUpgradeType.GoldMultiplierBonus:
                        goldMultiplierBonus += totalValue;
                        break;
                }
            }
        }

        float finalSpeed =
            baseStats.maxSpeed * (1f + speedBonusPercent);

        float finalAcceleration = Mathf.Max(
            0f,
            baseStats.accelerationTime - accelerationReduction
        );

        float finalDeceleration = Mathf.Max(
            0f,
            baseStats.decelerationTime - decelerationReduction
        );

        float finalGravity = Mathf.Max(
            0f,
            baseStats.gravityScale + gravityChange
        );

        float finalJumpForce = Mathf.Max(
            0f,
            baseStats.jumpForce + jumpForceChange
        );

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

        // Aplicamos la bonificación acumulada de oro.
        if (playerGold != null)
        {
            playerGold.SetUpgradeMultiplierBonus(
                goldMultiplierBonus
            );
        }

        RecalculateCameraStats();

        Debug.Log(
            "ATRIBUTOS ACTUALIZADOS\n" +
            "Velocidad: " + finalSpeed +
            "\nAceleración: " + finalAcceleration +
            "\nDesaceleración: " + finalDeceleration +
            "\nGravedad: " + finalGravity +
            "\nFuerza de salto: " + finalJumpForce +
            "\nSaltos máximos: " + finalJumps +
            "\nBonificación multiplicador de oro: +" +
            goldMultiplierBonus
        );
    }

    // ------------------------------------------------
    // MEJORAS DE CAMARA
    // ------------------------------------------------

    private void RecalculateCameraStats()
    {
        if (cameraFollow == null)
            return;

        float zoomPercent = 0f;
        float followLagReduction = 0f;

        foreach (KeyValuePair<InventoryEntryData, int> pair in itemStacks)
        {
            InventoryEntryData item = pair.Key;
            int amount = pair.Value;

            if (item == null)
                continue;

            foreach (PlayerUpgradeEffect effect in GetConfiguredEffects(item))
            {
                float totalValue = effect.upgradeValue * amount;

                switch (effect.upgradeType)
                {
                    case PlayerUpgradeType.CameraZoomPercent:
                        zoomPercent += totalValue;
                        break;

                    case PlayerUpgradeType.CameraFollowLagReductionPercent:
                        followLagReduction += totalValue;
                        break;
                }
            }
        }

        float zoomMultiplier = Mathf.Max(
            0.01f,
            1f + zoomPercent / 100f
        );

        cameraFollow.SetZoomMultiplier(zoomMultiplier);

        cameraFollow.SetFollowLagReduction(
            Mathf.Clamp(followLagReduction, 0f, 100f)
        );

        Debug.Log(
            "CAMARA ACTUALIZADA\n" +
            "Modificador de zoom: " + zoomPercent + "%" +
            "\nReducción de retraso: " + followLagReduction + "%"
        );
    }
}
