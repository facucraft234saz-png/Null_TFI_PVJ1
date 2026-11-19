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
    [Tooltip("Si no se asigna una imagen, se mostrara un cuadrado de color.")]
    public Sprite icon;

    [Tooltip("Color utilizado como icono provisional.")]
    public Color placeholderColor = Color.white;

    [Header("Mejora del jugador")]
    public PlayerUpgradeType upgradeType = PlayerUpgradeType.None;

    [Tooltip("Valor que modifica el atributo. Su significado depende del tipo de mejora.")]
    public float upgradeValue = 1f;

    [Min(1)]
    [Tooltip("Cantidad maxima de veces que este mismo item puede acumularse.")]
    public int maxStack = 1;
}
