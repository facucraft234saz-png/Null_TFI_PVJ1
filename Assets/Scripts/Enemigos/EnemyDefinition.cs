
using UnityEngine;

[CreateAssetMenu(
    fileName = "NewEnemyDefinition",
    menuName = "Game/Enemies/Enemy Definition"
)]
public class EnemyDefinition : ScriptableObject
{
    [Header("Informacion")]
    public string enemyName;

    [TextArea(2, 5)]
    public string description;

    [Header("Tipo de enemigo")]
    public EnemyType enemyType = EnemyType.Basic;

    [Header("Prefab")]
    public GameObject enemyPrefab;

    [Header("Movimiento")]
    [Min(0f)]
    public float movementSpeed = 1.5f;

    [Header("Ataque de contacto - EnemyBasic")]
    [Min(1)]
    public int damageHearts = 1;

    [Min(0f)]
    public float contactImmunityTime = 2f;

    [Header("Aparicion")]
    [Tooltip("Segundos que espera antes de aparecer.")]
    [Min(0f)]
    public float spawnDelay = 2f;

    [Tooltip("Distancia desde el jugador donde aparece.")]
    [Min(0f)]
    public float spawnDistance = 3f;

    [Header("Tamaño del enemigo")]
    public Vector2 enemyScale = new Vector2(0.7f, 0.7f);

    [Header("Ataque a distancia - EnemyDistance")]
    [Tooltip("Distancia maxima a la que puede disparar.")]
    [Min(0f)]
    public float attackRange = 5f;

    [Tooltip("Tiempo de espera despues de terminar una rafaga completa.")]
    [Min(0.01f)]
    public float attackCooldown = 2f;

    [Tooltip("Segundos entre cada proyectil de una misma rafaga. 0 = disparos consecutivos.")]
    [Min(0f)]
    public float projectileInterval = 1f;

    [Header("Configuracion de proyectiles")]
    [Min(1)]
    public int projectileDamage = 1;

    [Min(1)]
    public int projectileCount = 1;

    [Min(0f)]
    public float projectileSpeed = 5f;

    public Vector2 projectileScale = new Vector2(0.25f, 0.25f);

    public GameObject projectilePrefab;

    [Min(0.1f)]
    public float projectileLifetime = 5f;
}
