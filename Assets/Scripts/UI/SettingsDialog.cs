using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsDialog : MonoBehaviour
{
    private static SettingsDialog _instance;

    [SerializeField] private GameObject _root;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;
    [SerializeField] private Dropdown _languageDropdown;

    private bool _initialized;

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
        Initialize();
        Close();
    }

    private void OnDestroy()
    {
        if (_musicVolumeSlider != null)
            _musicVolumeSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);

        if (_sfxVolumeSlider != null)
            _sfxVolumeSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);

        if (_languageDropdown != null)
            _languageDropdown.onValueChanged.RemoveListener(OnLanguageChanged);
    }

    private void Initialize()
    {
        if (_initialized) return;
        _initialized = true;

        if (_musicVolumeSlider != null)
        {
            _musicVolumeSlider.minValue = 0f;
            _musicVolumeSlider.maxValue = 1f;
            _musicVolumeSlider.wholeNumbers = false;
            _musicVolumeSlider.SetValueWithoutNotify(AudioManager.GetMusicVolume01());
            _musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (_sfxVolumeSlider != null)
        {
            _sfxVolumeSlider.minValue = 0f;
            _sfxVolumeSlider.maxValue = 1f;
            _sfxVolumeSlider.wholeNumbers = false;
            _sfxVolumeSlider.SetValueWithoutNotify(AudioManager.GetSfxVolume01());
            _sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        }

        if (_languageDropdown != null)
        {
            string[] languages = LocalizationManager.GetAvailableLanguages();
            string[] codes = LocalizationManager.GetAvailableLanguageCodes();

            _languageDropdown.ClearOptions();
            var options = new List<Dropdown.OptionData>(languages.Length);
            foreach (string lang in languages)
                options.Add(new Dropdown.OptionData(lang));
            _languageDropdown.AddOptions(options);

            int currentIndex = System.Array.IndexOf(codes, LocalizationManager.CurrentLanguageCode);
            if (currentIndex < 0) currentIndex = 0;
            _languageDropdown.SetValueWithoutNotify(currentIndex);

            _languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        }
    }

    public void Open()
    {
        Initialize();

        if (_musicVolumeSlider != null)
            _musicVolumeSlider.SetValueWithoutNotify(AudioManager.GetMusicVolume01());

        if (_sfxVolumeSlider != null)
            _sfxVolumeSlider.SetValueWithoutNotify(AudioManager.GetSfxVolume01());

        if (_languageDropdown != null)
        {
            string[] codes = LocalizationManager.GetAvailableLanguageCodes();
            int currentIndex = System.Array.IndexOf(codes, LocalizationManager.CurrentLanguageCode);
            if (currentIndex < 0) currentIndex = 0;
            _languageDropdown.SetValueWithoutNotify(currentIndex);
        }

        if (_root != null) _root.SetActive(true);
        else gameObject.SetActive(true);
    }

    public bool IsOpen()
    {
        if (_root != null) return _root.activeSelf;
        return gameObject.activeSelf;
    }

    public void Close()
    {
        if (_root != null) _root.SetActive(false);
        else gameObject.SetActive(false);
    }

    private void OnMusicVolumeChanged(float value)
    {
        AudioManager.SetMusicVolume(value);
    }

    private void OnSfxVolumeChanged(float value)
    {
        AudioManager.SetSFXVolume(value);
    }

    private void OnLanguageChanged(int index)
    {
        string[] codes = LocalizationManager.GetAvailableLanguageCodes();
        if (index < 0 || index >= codes.Length) return;

        LocalizationManager.SetLanguage(codes[index]);
    }
}
