
using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("CONFIGURACION DEL JUGADOR")]
    [SerializeField] private PlayerStats stats;

    [Header("DETECCION DEL SUELO")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [Header("SONIDOS")]
    [SerializeField, Min(0.05f)]
    private float footstepInterval = 0.7f;

    [SerializeField, Min(0f)]
    private float movementSoundThreshold = 0.1f;

    [Header("SPRITE DEL JUGADOR")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("ANIMACION IDLE")]
    [SerializeField] private Sprite[] idleSprites;

    [SerializeField, Min(0.01f)]
    private float idleFrameRate = 0.2f;

    [Header("ANIMACION WALK")]
    [SerializeField] private Sprite[] walkSprites;

    [Tooltip("Intervalo entre imagenes al caminar normalmente.")]
    [SerializeField, Min(0.01f)]
    private float walkFrameRate = 0.12f;

    [Tooltip("Intervalo entre imagenes al caminar rapido.")]
    [SerializeField, Min(0.01f)]
    private float fastWalkFrameRate = 0.06f;

    [Tooltip("Velocidad horizontal a partir de la cual acelera la animacion.")]
    [SerializeField, Min(0f)]
    private float fastSpeedThreshold = 6f;

    [Header("ANIMACION DE SALTO Y CAIDA")]
    [SerializeField] private Sprite jumpSprite;
    [SerializeField] private Sprite fallSprite;

    [Tooltip("Margen vertical para decidir si sube o baja.")]
    [SerializeField, Min(0f)]
    private float verticalAnimationThreshold = 0.1f;

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

    // Datos de animacion.
    private float animationTimer;
    private int currentFrame;
    private Sprite currentDisplayedSprite;

    // Permite inicializar correctamente la primera animacion.
    private bool animationInitialized;

    private enum PlayerAnimation
    {
        Idle,
        Walk,
        Jump,
        Fall
    }

    private PlayerAnimation currentAnimation;

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

        if (spriteRenderer == null)
        {
            Debug.LogError(
                "PlayerController2D: falta asignar el SpriteRenderer.",
                this
            );
        }
    }

    private void Update()
    {
        if (Time.timeScale <= 0f)
            return;

        ReadMovementInput();
        ReadJumpInput();

        UpdatePlayerAnimation();
    }

    private void FixedUpdate()
    {
        if (Time.timeScale <= 0f)
            return;

        CheckGround();
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

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayJumpSound();
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

        if (!isGrounded || !isMoving)
        {
            footstepTimer = 0f;
            return;
        }

        footstepTimer -= Time.fixedDeltaTime;

        if (footstepTimer <= 0f)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayFootstepSound();

            footstepTimer = footstepInterval;
        }
    }

    // ------------------------------------------------
    // ANIMACIONES
    // ------------------------------------------------

    private void UpdatePlayerAnimation()
    {
        if (spriteRenderer == null)
            return;

        // Invertir el sprite segun la direccion.
        if (Mathf.Abs(rb.linearVelocity.x) > 0.01f)
        {
            spriteRenderer.flipX = rb.linearVelocity.x < 0f;
        }
        else if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            spriteRenderer.flipX = horizontalInput < 0f;
        }

        float verticalSpeed = rb.linearVelocity.y;
        float horizontalSpeed = Mathf.Abs(rb.linearVelocity.x);

        // SALTO: el jugador esta subiendo.
        if (!isGrounded &&
            verticalSpeed > verticalAnimationThreshold)
        {
            SetSingleSprite(
                PlayerAnimation.Jump,
                jumpSprite
            );

            return;
        }

        // CAIDA: el jugador esta bajando.
        if (!isGrounded &&
            verticalSpeed < -verticalAnimationThreshold)
        {
            SetSingleSprite(
                PlayerAnimation.Fall,
                fallSprite
            );

            return;
        }

        // CAMINAR: jugador en el suelo y con movimiento horizontal.
        if (isGrounded &&
            horizontalSpeed > movementSoundThreshold)
        {
            float selectedFrameRate =
                horizontalSpeed > fastSpeedThreshold
                ? fastWalkFrameRate
                : walkFrameRate;

            PlaySpriteAnimation(
                PlayerAnimation.Walk,
                walkSprites,
                selectedFrameRate
            );

            return;
        }

        // IDLE: jugador quieto en el suelo.
        if (isGrounded)
        {
            PlaySpriteAnimation(
                PlayerAnimation.Idle,
                idleSprites,
                idleFrameRate
            );
        }
    }

    // ------------------------------------------------
    // ANIMACION DE VARIOS FOTOGRAMAS
    // ------------------------------------------------

    private void PlaySpriteAnimation(
        PlayerAnimation animation,
        Sprite[] sprites,
        float frameRate)
    {
        if (sprites == null || sprites.Length == 0)
            return;

        // Inicializar o reiniciar cuando cambia la animacion.
        if (!animationInitialized || currentAnimation != animation)
        {
            animationInitialized = true;
            currentAnimation = animation;
            currentFrame = 0;
            animationTimer = 0f;

            ApplySprite(sprites[currentFrame]);
        }

        animationTimer += Time.deltaTime;

        if (animationTimer >= frameRate)
        {
            animationTimer = 0f;

            currentFrame++;

            if (currentFrame >= sprites.Length)
                currentFrame = 0;

            ApplySprite(sprites[currentFrame]);
        }
    }

    // ------------------------------------------------
    // ANIMACION DE UN SOLO FOTOGRAMA
    // ------------------------------------------------

    private void SetSingleSprite(
        PlayerAnimation animation,
        Sprite sprite)
    {
        if (sprite == null)
            return;

        // Inicializar o reiniciar cuando cambia la animacion.
        if (!animationInitialized || currentAnimation != animation)
        {
            animationInitialized = true;
            currentAnimation = animation;
            currentFrame = 0;
            animationTimer = 0f;

            ApplySprite(sprite);
        }
    }

    // ------------------------------------------------
    // CAMBIAR SPRITE
    // ------------------------------------------------

    private void ApplySprite(Sprite sprite)
    {
        if (sprite == null || spriteRenderer == null)
            return;

        if (currentDisplayedSprite == sprite)
            return;

        spriteRenderer.sprite = sprite;
        currentDisplayedSprite = sprite;
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
