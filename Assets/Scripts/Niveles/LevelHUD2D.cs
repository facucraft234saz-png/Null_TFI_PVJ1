using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelHUD2D : MonoBehaviour
{
    [Header("Texto del nivel")]
    [SerializeField] private TMP_Text levelText;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void Start()
    {
        UpdateLevelText();
    }

    private void Update()
    {
        // Permite detectar el nuevo nivel aunque el administrador
        // haya avanzado sin cambiar de escena.
        UpdateLevelText();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateLevelText();
    }

    private void UpdateLevelText()
    {
        if (levelText == null)
            return;

        LevelProgressManager2D manager =
            LevelProgressManager2D.Instance;

        if (manager == null)
            return;

        levelText.text = "NIVEL " + manager.CurrentLevel;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }
}