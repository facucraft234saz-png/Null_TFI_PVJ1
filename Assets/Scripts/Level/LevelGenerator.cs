using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Configuracion del Nivel")]
    [SerializeField] private int numberOfMiddleRooms = 5;
    [SerializeField] private float defaultRoomWidth = 20f;

    [Header("Prefabs de Salas")]
    [SerializeField] private GameObject startRoomPrefab;
    [SerializeField] private List<GameObject> middleRoomPrefabs;
    [SerializeField] private GameObject endRoomPrefab;

    [Header("Jugador en Escena")]
    [SerializeField] private Transform playerTransform;

    public event Action<Transform> OnLevelGenerated;

    private Vector3 currentSpawnPosition = Vector3.zero;

    private void Start()
    {
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        // 1. Sala Inicial
        if (startRoomPrefab != null)
        {
            GameObject startRoom = Instantiate(startRoomPrefab, currentSpawnPosition, Quaternion.identity, transform);
            MovePlayerToStart(startRoom);
            currentSpawnPosition = GetNextSpawnPosition(startRoom);
        }

        // 2. Salas Intermedias elegidas al azar
        for (int i = 0; i < numberOfMiddleRooms; i++)
        {
            if (middleRoomPrefabs == null || middleRoomPrefabs.Count == 0) break;

            int randomIndex = UnityEngine.Random.Range(0, middleRoomPrefabs.Count);
            GameObject roomPrefab = middleRoomPrefabs[randomIndex];

            GameObject newRoom = Instantiate(roomPrefab, currentSpawnPosition, Quaternion.identity, transform);
            currentSpawnPosition = GetNextSpawnPosition(newRoom);
        }

        // 3. Sala Final (meta del nivel)
        if (endRoomPrefab != null)
        {
            Instantiate(endRoomPrefab, currentSpawnPosition, Quaternion.identity, transform);
        }

        OnLevelGenerated?.Invoke(playerTransform);
    }

    private Vector3 GetNextSpawnPosition(GameObject room)
    {
        // Busca un hijo llamado "ExitPoint" para saber exactamente donde termina esta sala
        Transform exitPoint = room.transform.Find("ExitPoint");
        if (exitPoint != null)
        {
            return exitPoint.position;
        }

        // Si no tiene ExitPoint, avanza el ancho por defecto
        return currentSpawnPosition + new Vector3(defaultRoomWidth, 0f, 0f);
    }

    private void MovePlayerToStart(GameObject room)
    {
        if (playerTransform == null) return;

        Transform playerSpawn = room.transform.Find("PlayerSpawn");
        Vector3 spawnPos = playerSpawn != null ? playerSpawn.position : room.transform.position + Vector3.up * 2f;

        playerTransform.position = spawnPos;
    }
}