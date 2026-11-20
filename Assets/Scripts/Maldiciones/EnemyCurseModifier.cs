
using System;
using UnityEngine;

public enum EnemyStatType
{
    MovementSpeed,
    DamageHearts,
    ContactImmunityTime,
    SpawnDelay,
    SpawnDistance,
    Size,

    AttackRange,
    AttackCooldown,
    ProjectileInterval,
    ProjectileSpeed,
    ProjectileSize,
    ProjectileCount,
    ProjectileDamage
}

public enum EnemyModifierMode
{
    Percent,
    Flat
}

[Serializable]
public class EnemyCurseModifier
{
    [Header("Estadistica")]
    public EnemyStatType stat;

    [Header("Tipo de modificacion")]
    public EnemyModifierMode mode = EnemyModifierMode.Percent;

    [Tooltip("Porcentaje: 50 = +50%, -20 = -20%. Cantidad fija: +1, -1, etc.")]
    public float value;
}
