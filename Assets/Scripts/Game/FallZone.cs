
using UnityEngine;

public class FallZone : MonoBehaviour
{
    [Header("Configuracion de Caida")]
    [Tooltip("El jugador pierde al bajar de esta altura.")]
    [SerializeField] private float fallYThreshold = -15f;

    private Transform playerTransform;
    private PlayerVitals playerVitals;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("FallZone: no se encontró un objeto con tag Player.");
            return;
        }

        playerTransform = player.transform;
        playerVitals = player.GetComponent<PlayerVitals>();

        if (playerVitals == null)
        {
            Debug.LogError("FallZone: el jugador no tiene PlayerVitals.");
        }
    }

    private void Update()
    {
        if (playerTransform == null || playerVitals == null)
            return;

        if (playerVitals.IsGameOver)
            return;

        if (playerTransform.position.y <= fallYThreshold)
        {
            Debug.Log("El jugador cayó fuera del nivel.");
            playerVitals.ForceGameOver();
        }
    }
}
