
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Configuracion del jugador")]
    [SerializeField] private PlayerStats stats;

    [Header("Deteccion del suelo")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;

    // Entrada del jugador.
    private float horizontalInput;
    private bool jumpRequested;

    // Estado del movimiento.
    private float currentHorizontalSpeed;
    private bool isGrounded;

    // Estadisticas modificables durante la partida.
    private float currentMaxSpeed;
    private float currentAccelerationTime;
    private float currentDecelerationTime;
    private float currentJumpForce;
    private float currentGravityScale;
    private int currentMaxJumps;

    private int jumpsRemaining;

    // Eventos para comunicar acciones a otros sistemas.
    public event Action<bool> OnGroundedChanged;
    public event Action OnJumped;

    // Propiedades publicas de consulta.
    public bool IsGrounded => isGrounded;
    public float CurrentSpeed => currentHorizontalSpeed;
    public int JumpsRemaining => jumpsRemaining;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (stats == null)
        {
            Debug.LogError(
                "PlayerController2D: falta asignar PlayerStats.",
                this
            );

            enabled = false;
            return;
        }

        // Copiamos los valores del ScriptableObject.
        // Asi las mejoras no modifican el asset original.
        currentMaxSpeed = stats.maxSpeed;
        currentAccelerationTime = stats.accelerationTime;
        currentDecelerationTime = stats.decelerationTime;
        currentJumpForce = stats.jumpForce;
        currentGravityScale = stats.gravityScale;
        currentMaxJumps = stats.maxJumps;

        rb.gravityScale = currentGravityScale;
        rb.freezeRotation = true;

        jumpsRemaining = currentMaxJumps;
    }

    private void Update()
    {
        ReadMovementInput();
        ReadJumpInput();
    }

    private void FixedUpdate()
    {
        CheckGround();
        MovePlayer();
        HandleJump();
    }

    // ------------------------------------------------
    // ENTRADA DEL JUGADOR
    // ------------------------------------------------

    private void ReadMovementInput()
    {
        horizontalInput = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            horizontalInput = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            horizontalInput = 1f;
        }
    }

    private void ReadJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequested = true;
        }
    }

    // ------------------------------------------------
    // DETECCION DEL SUELO
    // ------------------------------------------------

    private void CheckGround()
    {
        bool previousGrounded = isGrounded;

        if (groundCheck == null)
        {
            isGrounded = false;
            return;
        }

        // Solo consideramos suelo si estamos descendiendo
        // o permanecemos apoyados sobre una plataforma.
        bool touchingGround = Physics2D.OverlapCircle(
            groundCheck.position,
            stats.groundCheckRadius,
            groundLayer
        );

        isGrounded =
            touchingGround && rb.linearVelocity.y <= 0.01f;

        // Recuperamos los saltos al aterrizar.
        if (isGrounded)
        {
            jumpsRemaining = currentMaxJumps;
        }

        if (previousGrounded != isGrounded)
        {
            OnGroundedChanged?.Invoke(isGrounded);
        }
    }

    // ------------------------------------------------
    // MOVIMIENTO HORIZONTAL
    // ------------------------------------------------

    private void MovePlayer()
    {
        float targetSpeed = horizontalInput * currentMaxSpeed;

        float accelerationTime = Mathf.Max(
            0.01f,
            currentAccelerationTime
        );

        float decelerationTime = Mathf.Max(
            0.01f,
            currentDecelerationTime
        );

        // Calculamos cuanto puede cambiar la velocidad
        // durante este paso de fisica.
        float accelerationRate =
            currentMaxSpeed / accelerationTime;

        float decelerationRate =
            currentMaxSpeed / decelerationTime;

        float selectedRate;

        // Si soltamos el movimiento o cambiamos de direccion,
        // utilizamos la desaceleracion.
        bool changingDirection =
            Mathf.Abs(currentHorizontalSpeed) > 0.01f &&
            horizontalInput != 0f &&
            Mathf.Sign(currentHorizontalSpeed) != horizontalInput;

        if (horizontalInput == 0f || changingDirection)
        {
            selectedRate = decelerationRate;
        }
        else
        {
            selectedRate = accelerationRate;
        }

        // Time.fixedDeltaTime es el tiempo correcto
        // para calcular cambios de velocidad en FixedUpdate.
        currentHorizontalSpeed = Mathf.MoveTowards(
            currentHorizontalSpeed,
            targetSpeed,
            selectedRate * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector2(
            currentHorizontalSpeed,
            rb.linearVelocity.y
        );
    }

    // ------------------------------------------------
    // SALTO
    // ------------------------------------------------

    private void HandleJump()
    {
        if (!jumpRequested)
            return;

        jumpRequested = false;

        if (jumpsRemaining <= 0)
            return;

        jumpsRemaining--;

        // Reiniciamos la velocidad vertical para que
        // cada salto tenga una fuerza consistente.
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            0f
        );

        rb.AddForce(
            Vector2.up * currentJumpForce,
            ForceMode2D.Impulse
        );

        // Evitamos considerar que seguimos en el suelo.
        isGrounded = false;

        OnGroundedChanged?.Invoke(false);
        OnJumped?.Invoke();
    }

    // ------------------------------------------------
    // MEJORAS Y MODIFICADORES FUTUROS
    // ------------------------------------------------

    // Modifica la velocidad maxima.
    public void SetMaxSpeed(float newSpeed)
    {
        currentMaxSpeed = Mathf.Max(0f, newSpeed);
    }

    // Modifica el tiempo de aceleracion.
    // Un valor menor significa acelerar mas rapido.
    public void SetAccelerationTime(float newTime)
    {
        currentAccelerationTime = Mathf.Max(0.01f, newTime);
    }

    // Modifica el tiempo de desaceleracion.
    // Un valor menor significa frenar mas rapido.
    public void SetDecelerationTime(float newTime)
    {
        currentDecelerationTime = Mathf.Max(0.01f, newTime);
    }

    // Modifica la fuerza del salto.
    public void SetJumpForce(float newForce)
    {
        currentJumpForce = Mathf.Max(0f, newForce);
    }

    // Modifica la gravedad.
    public void SetGravity(float newGravity)
    {
        currentGravityScale = Mathf.Max(0f, newGravity);
        rb.gravityScale = currentGravityScale;
    }

    // Modifica la cantidad de saltos disponibles.
    public void SetMaxJumps(int newMaxJumps)
    {
        currentMaxJumps = Mathf.Max(1, newMaxJumps);
        jumpsRemaining = currentMaxJumps;
    }

    // Aplica varias mejoras de movimiento juntas.
    public void ApplyMovementUpgrade(
        float speedBonus,
        float accelerationTimeReduction,
        float decelerationTimeReduction)
    {
        currentMaxSpeed += speedBonus;

        currentAccelerationTime = Mathf.Max(
            0.01f,
            currentAccelerationTime - accelerationTimeReduction
        );

        currentDecelerationTime = Mathf.Max(
            0.01f,
            currentDecelerationTime - decelerationTimeReduction
        );
    }

    // ------------------------------------------------
    // VISUALIZACION DEL GROUND CHECK
    // ------------------------------------------------

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null || stats == null)
            return;

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            stats.groundCheckRadius
        );
    }
}
