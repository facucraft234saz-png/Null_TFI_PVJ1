
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class DistanceEnemyFollower2D : MonoBehaviour
{
    [Header("Referencias de animación")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Animación Cooldown")]
    [SerializeField] private Sprite[] cooldownSprites;
    [SerializeField, Min(0.01f)] private float cooldownFrameInterval = 0.2f;

    [Header("Animación Attack")]
    [SerializeField] private Sprite[] attackSprites;
    [SerializeField, Min(0.01f)] private float attackFrameInterval = 0.1f;

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

    // Control de animación.
    private enum EnemyAnimation
    {
        None,
        Cooldown,
        Attack
    }

    private EnemyAnimation currentAnimation = EnemyAnimation.None;

    private float animationTimer;
    private int currentFrame;
    private Sprite currentDisplayedSprite;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.freezeRotation = true;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);

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
        if (!configured)
            return;

        UpdateFacing();

        // Atacando utiliza sus sprites de Attack.
        // Cuando no dispara utiliza los sprites de Cooldown.
        if (isShooting)
        {
            Animate(
                attackSprites,
                attackFrameInterval,
                EnemyAnimation.Attack
            );
        }
        else
        {
            Animate(
                cooldownSprites,
                cooldownFrameInterval,
                EnemyAnimation.Cooldown
            );
        }

        if (target == null ||
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

    // --------------------------------------------------
    // MOVIMIENTO
    // --------------------------------------------------

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

    // --------------------------------------------------
    // DIRECCIÓN
    // --------------------------------------------------

    private void UpdateFacing()
    {
        if (spriteRenderer == null || target == null)
            return;

        spriteRenderer.flipX =
            target.position.x < transform.position.x;
    }

    // --------------------------------------------------
    // ANIMACIONES
    // --------------------------------------------------

    private void Animate(
        Sprite[] sprites,
        float frameInterval,
        EnemyAnimation animation)
    {
        if (spriteRenderer == null ||
            sprites == null ||
            sprites.Length == 0)
            return;

        // Reiniciamos los frames cuando cambia de animación.
        if (currentAnimation != animation)
        {
            currentAnimation = animation;
            currentFrame = 0;
            animationTimer = 0f;

            ApplyAnimationSprite(sprites);
            return;
        }

        animationTimer += Time.deltaTime;

        float interval = Mathf.Max(0.01f, frameInterval);

        while (animationTimer >= interval)
        {
            animationTimer -= interval;

            currentFrame++;

            if (currentFrame >= sprites.Length)
                currentFrame = 0;

            ApplyAnimationSprite(sprites);
        }
    }

    private void ApplyAnimationSprite(Sprite[] sprites)
    {
        if (sprites == null ||
            sprites.Length == 0 ||
            spriteRenderer == null)
            return;

        currentFrame = Mathf.Clamp(
            currentFrame,
            0,
            sprites.Length - 1
        );

        Sprite newSprite = sprites[currentFrame];

        if (newSprite == null || newSprite == currentDisplayedSprite)
            return;

        spriteRenderer.sprite = newSprite;
        currentDisplayedSprite = newSprite;
    }

    // --------------------------------------------------
    // DISPAROS
    // --------------------------------------------------

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

            ShootSingleProjectile(i, count);

            if (i < count - 1 && projectileInterval > 0f)
            {
                yield return new WaitForSeconds(projectileInterval);
            }
        }

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

    // --------------------------------------------------
    // LIMPIEZA
    // --------------------------------------------------

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
