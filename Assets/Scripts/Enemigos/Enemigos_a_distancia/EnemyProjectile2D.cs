
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class EnemyProjectile2D : MonoBehaviour
{
    private Rigidbody2D rb;

    private PlayerVitals targetVitals;

    private Vector2 direction;
    private float speed;
    private int damage;
    private float lifetime;

    private bool configured;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.freezeRotation = true;

        Collider2D[] colliders =
            GetComponentsInChildren<Collider2D>();

        foreach (Collider2D projectileCollider in colliders)
            projectileCollider.isTrigger = true;
    }

    public void Configure(
        Vector2 shootDirection,
        PlayerVitals playerVitals,
        float projectileSpeed,
        int projectileDamage,
        float projectileLifetime,
        Vector2 projectileSize,
        Collider2D[] shooterColliders)
    {
        if (playerVitals == null)
        {
            Destroy(gameObject);
            return;
        }

        targetVitals = playerVitals;

        direction = shootDirection.normalized;
        speed = projectileSpeed;
        damage = projectileDamage;
        lifetime = projectileLifetime;

        transform.localScale = new Vector3(
            projectileSize.x,
            projectileSize.y,
            1f
        );

        // Evitar que el proyectil choque con su propio enemigo.
        Collider2D[] projectileColliders =
            GetComponentsInChildren<Collider2D>();

        foreach (Collider2D projectileCollider in projectileColliders)
        {
            foreach (Collider2D shooterCollider in shooterColliders)
            {
                if (shooterCollider != null)
                {
                    Physics2D.IgnoreCollision(
                        projectileCollider,
                        shooterCollider,
                        true
                    );
                }
            }
        }

        configured = true;

        Destroy(gameObject, Mathf.Max(0.1f, lifetime));
    }

    private void FixedUpdate()
    {
        if (!configured)
            return;

        Vector2 nextPosition =
            rb.position + direction * speed * Time.fixedDeltaTime;

        rb.MovePosition(nextPosition);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!configured)
            return;

        PlayerVitals detectedVitals =
            other.GetComponentInParent<PlayerVitals>();

        if (detectedVitals == null)
            return;

        if (detectedVitals != targetVitals)
            return;

        targetVitals.TakeDamage(damage);

        Destroy(gameObject);
    }
}
