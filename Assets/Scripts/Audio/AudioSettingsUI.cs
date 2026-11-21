
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("CONTROLES")]
    [SerializeField] private Slider musicVolumeSlider;

    private void Start()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("No se encontró AudioManager.");
            return;
        }

        if (musicVolumeSlider == null)
        {
            Debug.LogError("No se asignó MusicVolumeSlider.");
            return;
        }

        // Cargar el volumen guardado.
        musicVolumeSlider.SetValueWithoutNotify(
            AudioManager.Instance.MusicVolume
        );

        // Escuchar cambios del Slider.
        musicVolumeSlider.onValueChanged.AddListener(
            ChangeMusicVolume
        );
    }

    public void ChangeMusicVolume(float volume)
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.SetMusicVolume(volume);
    }

    private void OnDestroy()
    {
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.RemoveListener(
                ChangeMusicVolume
            );
        }
    }
}
