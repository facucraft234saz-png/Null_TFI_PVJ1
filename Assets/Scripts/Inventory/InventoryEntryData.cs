
using System.Collections.Generic;
using UnityEngine;

public enum PlayerUpgradeType
{
    None,
    AddHearts,
    AddShields,
    MoveSpeedPercent,
    AccelerationTimeReduction,
    DecelerationTimeReduction,
    GravityChange,
    JumpForceChange,
    ExtraJumps
}

[CreateAssetMenu(
    fileName = "NewInventoryEntry",
    menuName = "Game/Inventory/Entry Data"
)]
public class InventoryEntryData : ScriptableObject
{
    [Header("Informacion")]
    public string entryName;

    [TextArea(2, 5)]
    public string description;

    [Header("Categoria")]
    public InventoryCategory category;

    [Header("Icono")]
    public Sprite icon;
    public Color placeholderColor = Color.white;

    [Header("Mejora del jugador")]
    public PlayerUpgradeType upgradeType = PlayerUpgradeType.None;
    public float upgradeValue = 1f;

    [Min(1)]
    public int maxStack = 1;

    [Header("Configuracion de enemigo")]
    public EnemyDefinition enemyDefinition;

    [Header("Maldicion: enemigos afectados")]
    public List<EnemyDefinition> affectedEnemies =
        new List<EnemyDefinition>();

    [Header("Maldicion: modificadores")]
    public List<EnemyCurseModifier> enemyModifiers =
        new List<EnemyCurseModifier>();

    [Header("Maldicion: enemigos adicionales")]
    public List<CurseAdditionalEnemy> additionalEnemies =
        new List<CurseAdditionalEnemy>();
}
