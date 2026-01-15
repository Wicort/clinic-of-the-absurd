using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageSelector : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Dropdown _languageDropdown;
    [SerializeField] private Button _applyButton;
    
    private void Start()
    {
        InitializeDropdown();
        UpdateCurrentLanguage();
    }
    
    private void InitializeDropdown()
    {
        if (_languageDropdown == null) return;
        
        // Получаем доступные языки из LocalizationManager
        string[] languages = LocalizationManager.GetAvailableLanguages();
        string[] languageCodes = LocalizationManager.GetAvailableLanguageCodes();
        
        // Очищаем и заполняем dropdown
        _languageDropdown.ClearOptions();
        
        // Создаем List<Dropdown.OptionData> из string[]
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach (string language in languages)
        {
            options.Add(new Dropdown.OptionData(language));
        }
        _languageDropdown.AddOptions(options);
        
        // Находим индекс текущего языка
        int currentIndex = System.Array.IndexOf(languageCodes, LocalizationManager.CurrentLanguageCode);
        if (currentIndex >= 0)
        {
            _languageDropdown.value = currentIndex;
        }
        
        // Добавляем обработчик
        _languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        
        // Кнопка применения
        if (_applyButton != null)
        {
            _applyButton.onClick.AddListener(ApplyLanguage);
        }
    }
    
    private void OnLanguageChanged(int index)
    {
        string[] languageCodes = LocalizationManager.GetAvailableLanguageCodes();
        if (index >= 0 && index < languageCodes.Length)
        {
            string languageCode = languageCodes[index];
            string languageName = LocalizationManager.GetAvailableLanguages()[index];
            Debug.Log($"Выбран язык: {languageName} (код: {languageCode})");
        }
    }
    
    public void ApplyLanguage()
    {
        int selectedIndex = _languageDropdown.value;
        string[] languageCodes = LocalizationManager.GetAvailableLanguageCodes();
        
        if (selectedIndex >= 0 && selectedIndex < languageCodes.Length)
        {
            string languageCode = languageCodes[selectedIndex];
            LocalizationManager.SetLanguage(languageCode);
            UpdateCurrentLanguage();
            
            Debug.Log($"Язык применен: {LocalizationManager.CurrentLanguageName} ({LocalizationManager.CurrentLanguageCode})");
        }
    }
    
    private void UpdateCurrentLanguage()
    {
        if (_languageDropdown != null)
        {
            string[] languageCodes = LocalizationManager.GetAvailableLanguageCodes();
            int currentIndex = System.Array.IndexOf(languageCodes, LocalizationManager.CurrentLanguageCode);
            if (currentIndex >= 0)
            {
                _languageDropdown.value = currentIndex;
            }
        }
    }
    
    // Для вызова из других скриптов
    public void SetLanguageWithoutNotify(string languageCode)
    {
        LocalizationManager.SetLanguage(languageCode);
        UpdateCurrentLanguage();
    }
    
    // Для тестирования через ContextMenu
    [ContextMenu("Switch to Russian")]
    private void SwitchToRussian() => SetLanguageWithoutNotify("ru");
    
    [ContextMenu("Switch to English")]
    private void SwitchToEnglish() => SetLanguageWithoutNotify("en");
    
    [ContextMenu("Switch to Turkish")]
    private void SwitchToTurkish() => SetLanguageWithoutNotify("tr");
    
    // Обновление списка языков (полезно для hot-reload)
    [ContextMenu("Refresh Language List")]
    private void RefreshLanguageList()
    {
        InitializeDropdown();
        Debug.Log("Список языков обновлен");
    }
}
