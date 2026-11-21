
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("AUDIO SOURCES")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Tooltip("AudioSource exclusivo para el sonido en loop de la puerta.")]
    [SerializeField] private AudioSource proximityLoopSource;

    [Header("MUSICA DEL MENU")]
    [SerializeField] private AudioClip mainMenuMusic;

    [Header("MUSICA DE LAS TIENDAS")]
    [SerializeField] private AudioClip normalShopMusic;
    [SerializeField] private AudioClip enemyShopMusic;
    [SerializeField] private AudioClip curseShopMusic;

    [Header("MUSICA GAMEPLAY - NIVELES 1 A 9")]
    [SerializeField] private AudioClip[] earlyLevelMusic;

    [Header("MUSICA GAMEPLAY - NIVEL 10 EN ADELANTE")]
    [SerializeField] private AudioClip[] advancedLevelMusic;

    [Header("CONFIGURACION GAMEPLAY")]
    [Min(1)]
    [SerializeField] private int advancedMusicStartingLevel = 10;

    [Header("MUSICA DE PAUSA Y GAME OVER")]
    [SerializeField] private AudioClip pauseMusic;
    [SerializeField] private AudioClip gameOverMusic;

    [Header("SONIDOS DE INTERFAZ")]
    [SerializeField] private AudioClip buttonClickSound;

    [Header("SONIDOS DEL JUGADOR")]
    [SerializeField] private AudioClip playerDamageSound;
    [SerializeField] private AudioClip blackGiftSound;
    [SerializeField] private AudioClip goldenGiftSound;
    [SerializeField] private AudioClip footstepSound;
    [SerializeField] private AudioClip jumpSound;

    [Header("SONIDOS DE LA PUERTA")]
    [SerializeField] private AudioClip exitDoorSound;
    [SerializeField] private AudioClip exitPromptLoopSound;
    [SerializeField] private AudioClip goldenGiftsAppearSound;

    private const string MUSIC_VOLUME_KEY = "MusicVolume";

    public float MusicVolume { get; private set; } = 1f;

    private AudioClip lastGameplayClip;
    private AudioClip currentGameplayClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        MusicVolume = PlayerPrefs.GetFloat(
            MUSIC_VOLUME_KEY, 1f
        );

        if (musicSource != null)
            musicSource.volume = MusicVolume;

        if (proximityLoopSource != null)
        {
            proximityLoopSource.playOnAwake = false;
            proximityLoopSource.loop = true;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        PlayMusicForScene(
            SceneManager.GetActiveScene().name
        );
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopExitPromptLoop();
        PlayMusicForScene(scene.name);
    }

    // ------------------------------------------------
    // MUSICA SEGUN LA ESCENA
    // ------------------------------------------------

    private void PlayMusicForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "MainMenu":
                PlayMusic(mainMenuMusic);
                break;

            case "SampleScene":
                PlayRandomGameplayMusic();
                break;

            case "Tienda":
                PlayMusic(normalShopMusic);
                break;

            case "TiendaEnemigos":
                PlayMusic(enemyShopMusic);
                break;

            case "TiendaMaldiciones":
            case "SceneMaldiciones":
                PlayMusic(curseShopMusic);
                break;

            default:
                StopMusic();
                break;
        }
    }

    // ------------------------------------------------
    // MUSICA ALEATORIA DEL GAMEPLAY
    // ------------------------------------------------

    private void PlayRandomGameplayMusic()
    {
        int currentLevel = 1;

        if (PlayerScenePersistence2D.Instance != null)
        {
            currentLevel =
                PlayerScenePersistence2D.Instance.CurrentLevel;
        }

        AudioClip[] selectedCollection;

        if (currentLevel >= advancedMusicStartingLevel)
        {
            selectedCollection = advancedLevelMusic;

            Debug.Log(
                "Gameplay: música avanzada. Nivel " +
                currentLevel
            );
        }
        else
        {
            selectedCollection = earlyLevelMusic;

            Debug.Log(
                "Gameplay: música inicial. Nivel " +
                currentLevel
            );
        }

        AudioClip selectedClip =
            GetRandomMusic(selectedCollection);

        if (selectedClip == null)
        {
            Debug.LogWarning(
                "No hay músicas asignadas para el nivel " +
                currentLevel
            );

            StopMusic();
            return;
        }

        currentGameplayClip = selectedClip;
        lastGameplayClip = selectedClip;

        PlayMusic(selectedClip);
    }

    private AudioClip GetRandomMusic(AudioClip[] collection)
    {
        if (collection == null || collection.Length == 0)
            return null;

        List<AudioClip> validClips =
            new List<AudioClip>();

        foreach (AudioClip clip in collection)
        {
            if (clip != null)
                validClips.Add(clip);
        }

        if (validClips.Count == 0)
            return null;

        if (validClips.Count > 1)
            validClips.Remove(lastGameplayClip);

        int randomIndex = Random.Range(
            0, validClips.Count
        );

        return validClips[randomIndex];
    }

    // ------------------------------------------------
    // PAUSA Y GAME OVER
    // ------------------------------------------------

    public void PlayPauseMusic()
    {
        if (pauseMusic != null)
            PlayMusic(pauseMusic);
    }

    public void ResumeGameplayMusic()
    {
        if (currentGameplayClip != null)
        {
            PlayMusic(currentGameplayClip);
        }
        else
        {
            PlayRandomGameplayMusic();
        }
    }

    public void PlayGameOverMusic()
    {
        if (gameOverMusic != null)
            PlayMusic(gameOverMusic);
    }

    // ------------------------------------------------
    // REPRODUCIR MUSICA
    // ------------------------------------------------

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null)
            return;

        if (musicSource.clip == clip &&
            musicSource.isPlaying)
        {
            return;
        }

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = MusicVolume;
        musicSource.Play();

        Debug.Log("Reproduciendo música: " + clip.name);
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    // ------------------------------------------------
    // VOLUMEN GLOBAL
    // ------------------------------------------------

    public void SetMusicVolume(float volume)
    {
        MusicVolume = Mathf.Clamp01(volume);

        if (musicSource != null)
            musicSource.volume = MusicVolume;

        PlayerPrefs.SetFloat(
            MUSIC_VOLUME_KEY, MusicVolume
        );

        PlayerPrefs.Save();
    }

    // ------------------------------------------------
    // EFECTOS DE SONIDO
    // ------------------------------------------------

    public void PlayButtonSound()
    {
        PlaySFX(buttonClickSound);
    }

    public void PlayPlayerDamageSound()
    {
        PlaySFX(playerDamageSound);
    }

    public void PlayBlackGiftSound()
    {
        PlaySFX(blackGiftSound);
    }

    public void PlayGoldenGiftSound()
    {
        PlaySFX(goldenGiftSound);
    }

    public void PlayFootstepSound()
    {
        PlaySFX(footstepSound);
    }

    public void PlayJumpSound()
    {
        PlaySFX(jumpSound);
    }

    public void PlayExitDoorSound()
    {
        PlaySFX(exitDoorSound);
    }

    public void PlayGoldenGiftsAppearSound()
    {
        PlaySFX(goldenGiftsAppearSound);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    // ------------------------------------------------
    // LOOP DEL AVISO DE LA PUERTA
    // ------------------------------------------------

    public void StartExitPromptLoop()
    {
        if (proximityLoopSource == null ||
            exitPromptLoopSound == null)
        {
            return;
        }

        if (proximityLoopSource.isPlaying &&
            proximityLoopSource.clip == exitPromptLoopSound)
        {
            return;
        }

        proximityLoopSource.Stop();
        proximityLoopSource.clip = exitPromptLoopSound;
        proximityLoopSource.loop = true;
        proximityLoopSource.Play();
    }

    public void StopExitPromptLoop()
    {
        if (proximityLoopSource != null)
            proximityLoopSource.Stop();
    }

    // ------------------------------------------------
    // DESTRUCCION
    // ------------------------------------------------

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }
}
