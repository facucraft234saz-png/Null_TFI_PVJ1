
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class DistanceEnemyFollower2D : MonoBehaviour
{
    private Rigidbody2D rb;

    private Transform target;
    private PlayerVitals targetVitals;

    private GameObject projectilePrefab;

    private float movementSpeed;
    private float attackRange;
    private float attackCooldown;
    private float projectileInterval;
    private float nextAttackTime;

    private float projectileSpeed;
    private float projectileLifetime;

    private int projectileDamage;
    private int projectileCount;

    private Vector2 projectileScale;

    private bool configured;
    private bool isShooting;

    private Collider2D[] enemyColliders;

    private Coroutine shootingCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.freezeRotation = true;

        enemyColliders =
            GetComponentsInChildren<Collider2D>();

        foreach (Collider2D enemyCollider in enemyColliders)
            enemyCollider.isTrigger = true;
    }

    public void Configure(
        Transform player,
        PlayerVitals playerVitals,
        EnemyDefinition definition,
        EnemyRuntimeStats stats)
    {
        if (player == null ||
            playerVitals == null ||
            definition == null ||
            stats == null)
        {
            Debug.LogError(
                "No se pudo configurar EnemyDistance.",
                this
            );

            return;
        }

        if (definition.projectilePrefab == null)
        {
            Debug.LogError(
                "EnemyDistance no tiene un prefab de proyectil asignado.",
                this
            );

            return;
        }

        target = player;
        targetVitals = playerVitals;

        projectilePrefab = definition.projectilePrefab;

        movementSpeed = stats.movementSpeed;
        attackRange = stats.attackRange;
        attackCooldown = stats.attackCooldown;
        projectileInterval = stats.projectileInterval;

        projectileSpeed = stats.projectileSpeed;
        projectileDamage = stats.projectileDamage;
        projectileCount = stats.projectileCount;
        projectileScale = stats.projectileScale;
        projectileLifetime = stats.projectileLifetime;

        transform.localScale = stats.enemyScale;

        gameObject.name = definition.enemyName;

        // Puede comenzar a disparar inmediatamente al entrar en rango.
        nextAttackTime = Time.time;

        configured = true;

        Debug.Log(
            definition.enemyName +
            " configurado como EnemyDistance." +
            "\nProyectiles por rafaga: " + projectileCount +
            "\nIntervalo entre proyectiles: " + projectileInterval +
            "\nCooldown posterior: " + attackCooldown
        );
    }

    private void FixedUpdate()
    {
        if (!configured || target == null)
            return;

        FollowPlayer();
    }

    private void Update()
    {
        if (!configured ||
            target == null ||
            targetVitals == null ||
            targetVitals.IsGameOver)
        {
            return;
        }

        float distance = Vector2.Distance(
            transform.position,
            target.position
        );

        if (distance <= attackRange)
            TryShoot();
    }

    private void FollowPlayer()
    {
        Vector2 currentPosition = rb.position;
        Vector2 targetPosition = target.position;

        Vector2 newPosition = Vector2.MoveTowards(
            currentPosition,
            targetPosition,
            movementSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPosition);
    }

    private void TryShoot()
    {
        if (isShooting)
            return;

        if (Time.time < nextAttackTime)
            return;

        if (projectilePrefab == null)
            return;

        shootingCoroutine = StartCoroutine(ShootBurst());
    }

    private IEnumerator ShootBurst()
    {
        isShooting = true;

        int count = Mathf.Max(1, projectileCount);

        for (int i = 0; i < count; i++)
        {
            if (target == null ||
                targetVitals == null ||
                targetVitals.IsGameOver)
            {
                break;
            }

            // Dispara un proyectil.
            ShootSingleProjectile(i, count);

            // Espera solamente si quedan proyectiles por disparar.
            if (i < count - 1 && projectileInterval > 0f)
            {
                yield return new WaitForSeconds(projectileInterval);
            }
        }

        // El cooldown comienza cuando termina toda la rafaga.
        nextAttackTime = Time.time + attackCooldown;

        isShooting = false;
        shootingCoroutine = null;

        Debug.Log(
            gameObject.name +
            " termino su rafaga. Proximo disparo en " +
            attackCooldown + " segundos."
        );
    }

    private void ShootSingleProjectile(
        int projectileIndex,
        int totalProjectiles)
    {
        Vector2 direction =
            ((Vector2)target.position - rb.position).normalized;

        // Separacion angular entre proyectiles.
        float angleSpacing = 12f;

        float totalAngle =
            angleSpacing * (totalProjectiles - 1);

        float startingAngle =
            -totalAngle * 0.5f;

        float angle =
            startingAngle + angleSpacing * projectileIndex;

        Vector2 shotDirection =
            RotateDirection(direction, angle);

        GameObject projectile = Instantiate(
            projectilePrefab,
            transform.position,
            Quaternion.identity
        );

        EnemyProjectile2D projectileScript =
            projectile.GetComponent<EnemyProjectile2D>();

        if (projectileScript == null)
        {
            Debug.LogError(
                "El prefab de proyectil no tiene EnemyProjectile2D.",
                projectile
            );

            Destroy(projectile);
            return;
        }

        projectileScript.Configure(
            shotDirection,
            targetVitals,
            projectileSpeed,
            projectileDamage,
            projectileLifetime,
            projectileScale,
            enemyColliders
        );

        Debug.Log(
            gameObject.name +
            " disparo proyectil " +
            (projectileIndex + 1) +
            "/" + totalProjectiles
        );
    }

    private Vector2 RotateDirection(
        Vector2 direction,
        float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;

        float cosine = Mathf.Cos(radians);
        float sine = Mathf.Sin(radians);

        return new Vector2(
            direction.x * cosine - direction.y * sine,
            direction.x * sine + direction.y * cosine
        ).normalized;
    }

    private void OnDisable()
    {
        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            shootingCoroutine = null;
        }

        isShooting = false;
    }
}
