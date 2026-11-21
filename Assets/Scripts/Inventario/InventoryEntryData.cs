
using System.Collections.Generic;
using UnityEngine;

public enum PlayerUpgradeType
{
    [InspectorName("Ninguna")]
    None,

    [InspectorName("Agregar corazones")]
    AddHearts,

    [InspectorName("Agregar escudos")]
    AddShields,

    [InspectorName("Aumentar velocidad (%)")]
    MoveSpeedPercent,

    [InspectorName("Reducir tiempo de aceleración")]
    AccelerationTimeReduction,

    [InspectorName("Reducir tiempo de desaceleración")]
    DecelerationTimeReduction,

    [InspectorName("Modificar gravedad")]
    GravityChange,

    [InspectorName("Modificar fuerza de salto")]
    JumpForceChange,

    [InspectorName("Saltos adicionales")]
    ExtraJumps,

    [InspectorName("Alejar cámara (%)")]
    CameraZoomPercent,

    [InspectorName("Reducir retraso de cámara (%)")]
    CameraFollowLagReductionPercent,

    [InspectorName("Aumentar multiplicador de oro")]
    GoldMultiplierBonus
}

[System.Serializable]
public class PlayerUpgradeEffect
{
    [InspectorName("Tipo de mejora")]
    public PlayerUpgradeType upgradeType = PlayerUpgradeType.None;

    [Tooltip("Valor de esta mejora. En porcentajes, 10 equivale a 10%. Para el multiplicador de oro, 0.5 agrega +0.5x.")]
    [InspectorName("Valor de la mejora")]
    public float upgradeValue = 1f;
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

    [Header("Mejoras del jugador")]
    [Tooltip("Podés agregar tantas mejoras como necesite este artículo.")]
    public List<PlayerUpgradeEffect> upgradeEffects =
        new List<PlayerUpgradeEffect>();

    // Compatibilidad con artículos anteriores.
    [HideInInspector]
    public PlayerUpgradeType upgradeType = PlayerUpgradeType.None;

    [HideInInspector]
    public float upgradeValue = 1f;

    [Header("Configuracion general")]
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
