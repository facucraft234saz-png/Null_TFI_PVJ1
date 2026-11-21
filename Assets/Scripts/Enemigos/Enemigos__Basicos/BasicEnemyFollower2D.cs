
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BasicEnemyFollower2D : MonoBehaviour
{
    [Header("Animación Idle")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] idleSprites;
    [SerializeField, Min(0.01f)] private float idleFrameInterval = 0.2f;

    private Rigidbody2D rb;

    private Transform target;
    private PlayerVitals targetVitals;

    private float movementSpeed;
    private int damageHearts;
    private float contactImmunityTime;

    private bool configured;

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

        Collider2D[] colliders =
            GetComponentsInChildren<Collider2D>();

        foreach (Collider2D enemyCollider in colliders)
            enemyCollider.isTrigger = true;

        ApplyIdleSprite();
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
                "No se pudo configurar el enemigo.",
                this
            );

            return;
        }

        target = player;
        targetVitals = playerVitals;

        movementSpeed = stats.movementSpeed;
        damageHearts = stats.damageHearts;
        contactImmunityTime = stats.contactImmunityTime;

        transform.localScale = stats.enemyScale;
        gameObject.name = definition.enemyName;

        configured = true;

        Debug.Log(
            definition.enemyName +
            " configurado con sus estadísticas finales."
        );
    }

    private void Update()
    {
        if (!configured)
            return;

        UpdateIdleAnimation();
        UpdateFacing();
    }

    private void FixedUpdate()
    {
        if (!configured || target == null)
            return;

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
    // ANIMACIÓN IDLE
    // --------------------------------------------------

    private void UpdateIdleAnimation()
    {
        if (spriteRenderer == null ||
            idleSprites == null ||
            idleSprites.Length == 0)
            return;

        animationTimer += Time.deltaTime;

        float interval = Mathf.Max(0.01f, idleFrameInterval);

        while (animationTimer >= interval)
        {
            animationTimer -= interval;

            currentFrame++;

            if (currentFrame >= idleSprites.Length)
                currentFrame = 0;

            ApplyIdleSprite();
        }
    }

    private void ApplyIdleSprite()
    {
        if (spriteRenderer == null ||
            idleSprites == null ||
            idleSprites.Length == 0)
            return;

        currentFrame = Mathf.Clamp(
            currentFrame,
            0,
            idleSprites.Length - 1
        );

        Sprite newSprite = idleSprites[currentFrame];

        if (newSprite == null || newSprite == currentDisplayedSprite)
            return;

        spriteRenderer.sprite = newSprite;
        currentDisplayedSprite = newSprite;
    }

    // --------------------------------------------------
    // DIRECCIÓN DEL ENEMIGO
    // --------------------------------------------------

    private void UpdateFacing()
    {
        if (spriteRenderer == null || target == null)
            return;

        spriteRenderer.flipX =
            target.position.x < transform.position.x;
    }

    // --------------------------------------------------
    // DAÑO AL JUGADOR
    // --------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    private void TryDamagePlayer(Collider2D other)
    {
        if (!configured || targetVitals == null)
            return;

        PlayerVitals detectedVitals =
            other.GetComponentInParent<PlayerVitals>();

        if (detectedVitals == null)
            return;

        if (detectedVitals != targetVitals)
            return;

        targetVitals.TakeDamage(
            damageHearts,
            contactImmunityTime
        );
    }
}
