using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer y Fuentes")]
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Musica por Pantalla / Estado")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip shopMusic;
    [SerializeField] private AudioClip gameOverMusic;

    [Header("Efectos de Sonido (SFX)")]
    [SerializeField] private AudioClip buttonClickSFX;
    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private AudioClip collectGiftSFX;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        LoadSavedVolumes();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
            HandleGameStateChanged(GameManager.Instance.CurrentState);
        }
        else
        {
            PlayBGM(menuMusic);
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }

        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Tienda")
        {
            PlayShopMusic();
        }
        else if (scene.name == "MainMenu")
        {
            PlayBGM(menuMusic);
        }
    }

    private void HandleGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Start:
                PlayBGM(menuMusic);
                break;
            case GameState.Playing:
                if (bgmSource != null) bgmSource.UnPause();
                PlayBGM(gameplayMusic);
                break;
            case GameState.Paused:
                if (bgmSource != null) bgmSource.Pause();
                break;
            case GameState.GameOver:
                PlayBGM(gameOverMusic, false);
                break;
        }
    }

    // ------------------------------------------------
    // REPRODUCCION DE MUSICA Y SFX
    // ------------------------------------------------

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (bgmSource == null || clip == null)
            return;

        if (bgmSource.clip == clip && bgmSource.isPlaying)
            return;

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void PlayShopMusic() => PlayBGM(shopMusic);
    public void PlayMenuMusic() => PlayBGM(menuMusic);
    public void PlayGameplayMusic() => PlayBGM(gameplayMusic);

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    public void PlayButtonClick() => PlaySFX(buttonClickSFX);
    public void PlayJump() => PlaySFX(jumpSFX);
    public void PlayCollectGift() => PlaySFX(collectGiftSFX);

    // ------------------------------------------------
    // CONTROL DE VOLUMEN (SLIDERS 0.0001 a 1.0)
    // ------------------------------------------------

    public void SetMasterVolume(float sliderValue)
    {
        SetMixerVolume("MasterVolume", sliderValue);
        PlayerPrefs.SetFloat("MasterVolume", sliderValue);
    }

    public void SetBGMVolume(float sliderValue)
    {
        SetMixerVolume("BGMVolume", sliderValue);
        PlayerPrefs.SetFloat("BGMVolume", sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        SetMixerVolume("SFXVolume", sliderValue);
        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
    }

    private void SetMixerVolume(string parameterName, float sliderValue)
    {
        if (mainMixer == null)
            return;

        float clampedValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        mainMixer.SetFloat(parameterName, Mathf.Log10(clampedValue) * 20f);
    }

    private void LoadSavedVolumes()
    {
        SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 0.8f));
        SetBGMVolume(PlayerPrefs.GetFloat("BGMVolume", 0.8f));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 0.8f));
    }
}