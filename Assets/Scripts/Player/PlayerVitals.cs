
using System;
using UnityEngine;

public class PlayerVitals : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField] private PlayerStats stats;

    private int currentHearts;
    private int maxHearts;
    private int currentShields;

    private float invulnerabilityTimer;
    private bool isGameOver;

    public int CurrentHearts => currentHearts;
    public int MaxHearts => maxHearts;
    public int CurrentShields => currentShields;
    public bool IsInvulnerable => invulnerabilityTimer > 0f;
    public bool IsGameOver => isGameOver;

    public event Action<int, int> OnHeartsChanged;
    public event Action<int> OnShieldsChanged;
    public event Action OnGameOver;

    private void Awake()
    {
        if (stats == null)
        {
            Debug.LogError("PlayerVitals: falta asignar PlayerStats.", this);
            enabled = false;
            return;
        }

        maxHearts = Mathf.Max(1, stats.startingHearts);
        currentHearts = maxHearts;
        currentShields = Mathf.Max(0, stats.startingShields);

        OnHeartsChanged?.Invoke(currentHearts, maxHearts);
        OnShieldsChanged?.Invoke(currentShields);
    }

    private void Update()
    {
        if (invulnerabilityTimer > 0f)
        {
            invulnerabilityTimer -= Time.deltaTime;
        }
    }

    // Daño normal: utiliza la inmunidad configurada en PlayerStats.
    public void TakeDamage(int amount = 1)
    {
        TakeDamage(amount, stats != null
            ? stats.invulnerabilityDuration
            : 0f);
    }

    // Daño con tiempo de inmunidad personalizado.
    public void TakeDamage(int amount, float immunityDuration)
    {
        if (isGameOver || IsInvulnerable || amount <= 0)
            return;

        int remainingDamage = amount;

        // Los escudos absorben primero el daño.
        if (currentShields > 0)
        {
            int absorbed = Mathf.Min(currentShields, remainingDamage);

            currentShields -= absorbed;
            remainingDamage -= absorbed;

            OnShieldsChanged?.Invoke(currentShields);
        }

        if (remainingDamage > 0)
        {
            currentHearts = Mathf.Max(0, currentHearts - remainingDamage);
            OnHeartsChanged?.Invoke(currentHearts, maxHearts);
        }

        Debug.Log(
            "Daño recibido. Corazones: " + currentHearts +
            " | Escudos: " + currentShields
        );

        if (currentHearts <= 0)
        {
            TriggerGameOver();
            return;
        }

        invulnerabilityTimer = Mathf.Max(0f, immunityDuration);
    }

    public void AddHearts(int amount)
    {
        if (isGameOver || amount <= 0)
            return;

        maxHearts += amount;
        currentHearts += amount;

        OnHeartsChanged?.Invoke(currentHearts, maxHearts);

        Debug.Log("Corazones aumentados: " + maxHearts);
    }

    public void AddShields(int amount)
    {
        if (isGameOver || amount <= 0)
            return;

        currentShields += amount;
        OnShieldsChanged?.Invoke(currentShields);

        Debug.Log("Escudos actuales: " + currentShields);
    }

    public void Heal(int amount)
    {
        if (isGameOver || amount <= 0)
            return;

        currentHearts = Mathf.Min(maxHearts, currentHearts + amount);
        OnHeartsChanged?.Invoke(currentHearts, maxHearts);
    }

    private void TriggerGameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        Debug.Log("GAME OVER: el jugador perdió todos sus corazones.");

        OnGameOver?.Invoke();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameState.GameOver);
        }
        else
        {
            Debug.LogWarning(
                "No se encontró GameManager. El evento Game Over sí fue emitido."
            );
        }
    }
}
