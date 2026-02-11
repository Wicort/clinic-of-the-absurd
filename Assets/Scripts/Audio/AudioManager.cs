using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<AudioManager>();
                if (_instance == null)
                {
                    Debug.LogError("AudioManager not found in scene!");
                }
            }
            return _instance;
        }
    }
    
    [Header("Аудиомикшеры")]
    [SerializeField] private AudioMixerGroup _musicMixerGroup;
    [SerializeField] private AudioMixerGroup _sfxMixerGroup;
    
    [Header("Фоновая музыка")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioClip _mainMenuMusic;
    [SerializeField] private AudioClip _gameplayMusic;
    [SerializeField] private AudioClip _bossMusic;
    
    [Header("Звуковые эффекты")]
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioClip[] _gagSounds;
    [SerializeField] private AudioClip[] _patientReactionSounds;
    [SerializeField] private AudioClip _buttonClickSound;
    [SerializeField] private AudioClip _doorOpenSound;
    [SerializeField] private AudioClip _rewardSound;
    [SerializeField] private AudioClip _victorySound;
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Настраиваем аудио источники
        if (_musicSource != null)
        {
            _musicSource.outputAudioMixerGroup = _musicMixerGroup;
            _musicSource.loop = true;
            _musicSource.playOnAwake = false;
        }
        
        if (_sfxSource != null)
        {
            _sfxSource.outputAudioMixerGroup = _sfxMixerGroup;
            _sfxSource.loop = false;
            _sfxSource.playOnAwake = false;
        }
        
        // Автоматически запускаем музыку в зависимости от сцены
        StartMusicForCurrentScene();
    }
    
    private void StartMusicForCurrentScene()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Debug.Log($"AudioManager: Starting music for scene '{sceneName}'");
        
        switch (sceneName.ToLower())
        {
            case "menu":
            case "mainmenu":
                PlayMainMenuMusic();
                break;
            case "game":
            case "gameplay":
            case "hospital":
            case "ward":
                PlayGameplayMusic();
                break;
            default:
                // Для других сцен можно запустить музыку по умолчанию
                if (_gameplayMusic != null)
                    PlayGameplayMusic();
                else
                    Debug.LogWarning("No gameplay music assigned to AudioManager");
                break;
        }
    }
    
    #region Музыка
    
    public static void PlayMainMenuMusic()
    {
        if (Instance?._musicSource != null && Instance._mainMenuMusic != null)
        {
            Instance._musicSource.clip = Instance._mainMenuMusic;
            Instance._musicSource.Play();
            Debug.Log("Playing main menu music");
        }
        else
        {
            Debug.LogWarning("Cannot play main menu music - missing AudioSource or AudioClip");
        }
    }
    
    public static void PlayGameplayMusic()
    {
        if (Instance?._musicSource != null && Instance._gameplayMusic != null)
        {
            Instance._musicSource.clip = Instance._gameplayMusic;
            Instance._musicSource.Play();
            Debug.Log("Playing gameplay music");
        }
        else
        {
            Debug.LogWarning("Cannot play gameplay music - missing AudioSource or AudioClip");
        }
    }
    
    public static void PlayBossMusic()
    {
        if (Instance?._musicSource != null && Instance._bossMusic != null)
        {
            Instance._musicSource.clip = Instance._bossMusic;
            Instance._musicSource.Play();
            Debug.Log("Playing boss music");
        }
        else
        {
            Debug.LogWarning("Cannot play boss music - missing AudioSource or AudioClip");
        }
    }
    
    public static void StopMusic()
    {
        if (Instance?._musicSource != null)
        {
            Instance._musicSource.Stop();
        }
    }
    
    public static void SetMusicVolume(float volume)
    {
        if (Instance?._musicMixerGroup != null)
        {
            Instance._musicMixerGroup.audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        }
    }
    
    #endregion
    
    #region Звуковые эффекты
    
    public static void PlayGagSound(HumorType gagType)
    {
        if (Instance?._sfxSource == null || Instance._gagSounds == null) return;
        
        int index = (int)gagType;
        if (index >= 0 && index < Instance._gagSounds.Length && Instance._gagSounds[index] != null)
        {
            Instance._sfxSource.PlayOneShot(Instance._gagSounds[index]);
        }
    }
    
    public static void PlayPatientReactionSound(PatientReactionType reactionType)
    {
        if (Instance?._sfxSource == null || Instance._patientReactionSounds == null) return;
        
        int index = (int)reactionType;
        if (index >= 0 && index < Instance._patientReactionSounds.Length && Instance._patientReactionSounds[index] != null)
        {
            Instance._sfxSource.PlayOneShot(Instance._patientReactionSounds[index]);
        }
    }
    
    public static void PlayButtonClickSound()
    {
        if (Instance?._sfxSource != null && Instance._buttonClickSound != null)
        {
            Instance._sfxSource.PlayOneShot(Instance._buttonClickSound);
        }
    }
    
    public static void PlayDoorOpenSound()
    {
        if (Instance?._sfxSource != null && Instance._doorOpenSound != null)
        {
            Instance._sfxSource.PlayOneShot(Instance._doorOpenSound);
        }
    }
    
    public static void PlayRewardSound()
    {
        if (Instance?._sfxSource != null && Instance._rewardSound != null)
        {
            Instance._sfxSource.PlayOneShot(Instance._rewardSound);
        }
    }
    
    public static void PlayVictorySound()
    {
        if (Instance?._sfxSource != null && Instance._victorySound != null)
        {
            Instance._sfxSource.PlayOneShot(Instance._victorySound);
        }
    }
    
    public static void SetSFXVolume(float volume)
    {
        if (Instance?._sfxMixerGroup != null)
        {
            Instance._sfxMixerGroup.audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        }
    }
    
    #endregion
    
    #region Утилиты
    
    public static void PauseMusic()
    {
        if (Instance?._musicSource != null)
        {
            Instance._musicSource.Pause();
        }
    }
    
    public static void ResumeMusic()
    {
        if (Instance?._musicSource != null)
        {
            Instance._musicSource.UnPause();
        }
    }
    
    public static bool IsMusicPlaying()
    {
        return Instance?._musicSource != null && Instance._musicSource.isPlaying;
    }
    
    #endregion
}
