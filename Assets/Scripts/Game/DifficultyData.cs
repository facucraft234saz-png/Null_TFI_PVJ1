
using UnityEngine;

[CreateAssetMenu(
    fileName = "DifficultyData",
    menuName = "Game/Difficulty Data"
)]
public class DifficultyData : ScriptableObject
{
    [Header("Identificacion")]
    public string difficultyName;

    [TextArea(3, 6)]
    public string description;

    [Header("Atributos del jugador")]
    [Min(1)]
    public int startingLives = 3;

    [Min(0f)]
    public float goldMultiplier = 2f;

    [Header("Frecuencia de eventos")]
    [Min(1)]
    public int enemySelectionEveryLevels = 2;

    [Min(1)]
    public int curseSelectionEveryLevels = 3;

    [Min(1)]
    public int shopEveryLevels = 1;

    [Header("Multiplicador de dificultad para enemigos")]
    [Min(0.1f)]
    public float enemyDifficultyMultiplier = 1f;
}
