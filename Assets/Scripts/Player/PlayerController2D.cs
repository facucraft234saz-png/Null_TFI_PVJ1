
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

    [Header("SONIDOS")]
    [Tooltip("Tiempo entre cada sonido de pasos.")]
    [SerializeField, Min(0.05f)]
    private float footstepInterval = 0.7f;

    [Tooltip("Movimiento minimo necesario para reproducir pasos.")]
    [SerializeField, Min(0f)]
    private float movementSoundThreshold = 0.1f;

    private Rigidbody2D rb;

    private float horizontalInput;
    private bool jumpRequested;

    private float currentHorizontalSpeed;
    private bool isGrounded;

    private float currentMaxSpeed;
    private float currentAccelerationTime;
    private float currentDecelerationTime;
    private float currentJumpForce;
    private float currentGravityScale;
    private int currentMaxJumps;

    private int jumpsRemaining;

    private float footstepTimer;

    public event Action<bool> OnGroundedChanged;
    public event Action OnJumped;

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
        // No leer controles durante la pausa.
        if (Time.timeScale <= 0f)
            return;

        ReadMovementInput();
        ReadJumpInput();
    }

    private void FixedUpdate()
    {
        if (Time.timeScale <= 0f)
            return;

        CheckGround();

        // Los pasos se comprueban después de actualizar el suelo.
        HandleFootsteps();

        MovePlayer();
        HandleJump();
    }

    // ------------------------------------------------
    // ENTRADAS
    // ------------------------------------------------

    private void ReadMovementInput()
    {
        horizontalInput = 0f;

        if (Input.GetKey(KeyCode.A))
            horizontalInput = -1f;
        else if (Input.GetKey(KeyCode.D))
            horizontalInput = 1f;
    }

    private void ReadJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequested = true;

            // El sonido se reproduce inmediatamente al presionar SPACE.
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayJumpSound();
            }
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

        bool touchingGround = Physics2D.OverlapCircle(
            groundCheck.position,
            stats.groundCheckRadius,
            groundLayer
        );

        isGrounded =
            touchingGround && rb.linearVelocity.y <= 0.01f;

        if (isGrounded)
            jumpsRemaining = currentMaxJumps;

        if (previousGrounded != isGrounded)
            OnGroundedChanged?.Invoke(isGrounded);
    }

    // ------------------------------------------------
    // MOVIMIENTO
    // ------------------------------------------------

    private void MovePlayer()
    {
        float targetSpeed = horizontalInput * currentMaxSpeed;

        if (horizontalInput != 0f &&
            currentAccelerationTime <= 0f)
        {
            currentHorizontalSpeed = targetSpeed;
        }
        else if (horizontalInput == 0f &&
                 currentDecelerationTime <= 0f)
        {
            currentHorizontalSpeed = 0f;
        }
        else
        {
            bool changingDirection =
                Mathf.Abs(currentHorizontalSpeed) > 0.01f &&
                horizontalInput != 0f &&
                Mathf.Sign(currentHorizontalSpeed) != horizontalInput;

            if (changingDirection &&
                currentDecelerationTime <= 0f)
            {
                currentHorizontalSpeed = 0f;
            }
            else
            {
                float selectedTime =
                    horizontalInput == 0f || changingDirection
                    ? currentDecelerationTime
                    : currentAccelerationTime;

                selectedTime = Mathf.Max(0.0001f, selectedTime);

                float selectedRate =
                    currentMaxSpeed / selectedTime;

                currentHorizontalSpeed = Mathf.MoveTowards(
                    currentHorizontalSpeed,
                    targetSpeed,
                    selectedRate * Time.fixedDeltaTime
                );
            }
        }

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

        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            0f
        );

        rb.AddForce(
            Vector2.up * currentJumpForce,
            ForceMode2D.Impulse
        );

        isGrounded = false;

        OnGroundedChanged?.Invoke(false);
        OnJumped?.Invoke();
    }

    // ------------------------------------------------
    // PASOS
    // ------------------------------------------------

    private void HandleFootsteps()
    {
        bool isMoving =
            Mathf.Abs(horizontalInput) > movementSoundThreshold;

        // No hay pasos si está en el aire o no se mueve.
        if (!isGrounded || !isMoving)
        {
            footstepTimer = 0f;
            return;
        }

        footstepTimer -= Time.fixedDeltaTime;

        if (footstepTimer <= 0f)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayFootstepSound();
            }

            // Próximo paso dentro de 0,7 segundos.
            footstepTimer = footstepInterval;
        }
    }

    // ------------------------------------------------
    // MODIFICADORES DE ATRIBUTOS
    // ------------------------------------------------

    public void SetMaxSpeed(float newSpeed)
    {
        currentMaxSpeed = Mathf.Max(0f, newSpeed);
    }

    public void SetAccelerationTime(float newTime)
    {
        currentAccelerationTime = Mathf.Max(0f, newTime);
    }

    public void SetDecelerationTime(float newTime)
    {
        currentDecelerationTime = Mathf.Max(0f, newTime);
    }

    public void SetJumpForce(float newForce)
    {
        currentJumpForce = Mathf.Max(0f, newForce);
    }

    public void SetGravity(float newGravity)
    {
        currentGravityScale = Mathf.Max(0f, newGravity);
        rb.gravityScale = currentGravityScale;
    }

    public void SetMaxJumps(int newMaxJumps)
    {
        currentMaxJumps = Mathf.Max(1, newMaxJumps);
        jumpsRemaining = currentMaxJumps;
    }

    public float GetMaxSpeed() => currentMaxSpeed;
    public float GetAccelerationTime() => currentAccelerationTime;
    public float GetDecelerationTime() => currentDecelerationTime;
    public float GetJumpForce() => currentJumpForce;
    public float GetGravity() => currentGravityScale;
    public int GetMaxJumps() => currentMaxJumps;

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
