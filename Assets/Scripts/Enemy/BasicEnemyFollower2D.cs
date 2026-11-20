
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class BasicEnemyFollower2D : MonoBehaviour
{
    private Rigidbody2D rb;

    private Transform target;
    private PlayerVitals targetVitals;

    private float movementSpeed;
    private int damageHearts;
    private float contactImmunityTime;

    private bool configured;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.freezeRotation = true;

        Collider2D[] colliders =
            GetComponentsInChildren<Collider2D>();

        foreach (Collider2D enemyCollider in colliders)
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
                "No se pudo configurar el enemigo.",
                this
            );

            return;
        }

        target = player;
        targetVitals = playerVitals;

        // Estadísticas finales, incluyendo las maldiciones.
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
