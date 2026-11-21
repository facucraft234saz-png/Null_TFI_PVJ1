using UnityEngine;

public class FallZone : MonoBehaviour
{
    [Header("Configuracion de Caida")]
    [Tooltip("Altura en Y por debajo de la cual el jugador pierde automaticamente.")]
    [SerializeField] private float fallYThreshold = -15f;

    private Transform playerTransform;
    private bool hasTriggered = false;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (hasTriggered || playerTransform == null)
            return;

        if (playerTransform.position.y < fallYThreshold)
        {
            TriggerGameOver();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered)
            return;

        if (other.CompareTag("Player"))
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        hasTriggered = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameState.GameOver);
        }
        else
        {
            Debug.LogWarning("FallZone: No se encontro GameManager en la escena.");
        }
    }
}