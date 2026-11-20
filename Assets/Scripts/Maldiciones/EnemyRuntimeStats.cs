
using System.Collections.Generic;
using UnityEngine;

public class EnemyRuntimeStats
{
    public float movementSpeed;
    public int damageHearts;
    public float contactImmunityTime;
    public float spawnDelay;
    public float spawnDistance;
    public Vector2 enemyScale;

    public float attackRange;
    public float attackCooldown;
    public float projectileInterval;

    public float projectileSpeed;
    public Vector2 projectileScale;
    public int projectileCount;
    public int projectileDamage;
    public float projectileLifetime;

    private Dictionary<EnemyStatType, float> percentages =
        new Dictionary<EnemyStatType, float>();

    private Dictionary<EnemyStatType, float> flatValues =
        new Dictionary<EnemyStatType, float>();

    public EnemyRuntimeStats(
        EnemyDefinition definition,
        PlayerInventory inventory)
    {
        movementSpeed = definition.movementSpeed;
        damageHearts = definition.damageHearts;
        contactImmunityTime = definition.contactImmunityTime;
        spawnDelay = definition.spawnDelay;
        spawnDistance = definition.spawnDistance;
        enemyScale = definition.enemyScale;

        attackRange = definition.attackRange;
        attackCooldown = definition.attackCooldown;
        projectileInterval = definition.projectileInterval;

        projectileSpeed = definition.projectileSpeed;
        projectileScale = definition.projectileScale;
        projectileCount = definition.projectileCount;
        projectileDamage = definition.projectileDamage;
        projectileLifetime = definition.projectileLifetime;

        CollectCurses(definition, inventory);
        ApplyCollectedModifiers();
    }

    private void CollectCurses(
        EnemyDefinition definition,
        PlayerInventory inventory)
    {
        if (inventory == null || definition == null)
            return;

        foreach (InventoryStack stack in inventory.Entries)
        {
            if (stack == null || stack.data == null)
                continue;

            InventoryEntryData curse = stack.data;

            if (curse.category != InventoryCategory.Curse)
                continue;

            if (curse.affectedEnemies == null ||
                !curse.affectedEnemies.Contains(definition))
                continue;

            int amount = Mathf.Min(
                stack.amount,
                Mathf.Max(1, curse.maxStack)
            );

            foreach (EnemyCurseModifier modifier in curse.enemyModifiers)
            {
                if (modifier == null)
                    continue;

                float totalValue = modifier.value * amount;

                Dictionary<EnemyStatType, float> destination =
                    modifier.mode == EnemyModifierMode.Percent
                    ? percentages
                    : flatValues;

                if (!destination.ContainsKey(modifier.stat))
                    destination[modifier.stat] = 0f;

                destination[modifier.stat] += totalValue;
            }
        }
    }

    private float GetModifiedValue(
        EnemyStatType stat,
        float original)
    {
        float percentage = percentages.ContainsKey(stat)
            ? percentages[stat]
            : 0f;

        float flat = flatValues.ContainsKey(stat)
            ? flatValues[stat]
            : 0f;

        return original * (1f + percentage / 100f) + flat;
    }

    private void ApplyCollectedModifiers()
    {
        movementSpeed = Mathf.Max(
            0f,
            GetModifiedValue(EnemyStatType.MovementSpeed, movementSpeed)
        );

        damageHearts = Mathf.Max(
            1,
            Mathf.RoundToInt(
                GetModifiedValue(EnemyStatType.DamageHearts, damageHearts)
            )
        );

        contactImmunityTime = Mathf.Max(
            0f,
            GetModifiedValue(
                EnemyStatType.ContactImmunityTime,
                contactImmunityTime
            )
        );

        spawnDelay = Mathf.Max(
            0f,
            GetModifiedValue(EnemyStatType.SpawnDelay, spawnDelay)
        );

        spawnDistance = Mathf.Max(
            0f,
            GetModifiedValue(EnemyStatType.SpawnDistance, spawnDistance)
        );

        float enemySizeMultiplier = GetModifiedValue(
            EnemyStatType.Size,
            1f
        );

        enemyScale *= Vector2.one * Mathf.Max(
            0.01f,
            enemySizeMultiplier
        );

        attackRange = Mathf.Max(
            0f,
            GetModifiedValue(EnemyStatType.AttackRange, attackRange)
        );

        attackCooldown = Mathf.Max(
            0.01f,
            GetModifiedValue(EnemyStatType.AttackCooldown, attackCooldown)
        );

        // El intervalo puede llegar a cero.
        projectileInterval = Mathf.Max(
            0f,
            GetModifiedValue(
                EnemyStatType.ProjectileInterval,
                projectileInterval
            )
        );

        projectileSpeed = Mathf.Max(
            0f,
            GetModifiedValue(
                EnemyStatType.ProjectileSpeed,
                projectileSpeed
            )
        );

        float projectileSizeMultiplier = GetModifiedValue(
            EnemyStatType.ProjectileSize,
            1f
        );

        projectileScale *= Vector2.one * Mathf.Max(
            0.01f,
            projectileSizeMultiplier
        );

        projectileCount = Mathf.Max(
            1,
            Mathf.RoundToInt(
                GetModifiedValue(
                    EnemyStatType.ProjectileCount,
                    projectileCount
                )
            )
        );

        projectileDamage = Mathf.Max(
            1,
            Mathf.RoundToInt(
                GetModifiedValue(
                    EnemyStatType.ProjectileDamage,
                    projectileDamage
                )
            )
        );
    }
}
