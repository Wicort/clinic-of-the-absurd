using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LocalizedButton : MonoBehaviour
{
    [Header("Настройки локализации")]
    [SerializeField] public string localizationKey = "";
    [SerializeField] private bool updateOnLanguageChange = true;
    
    private Button _button;
    private Text _buttonText;
    private string _originalText;
    
    private void Awake()
    {
        _button = GetComponent<Button>();
        _buttonText = _button.GetComponentInChildren<Text>();
        
        if (_buttonText != null)
        {
            _originalText = _buttonText.text;
        }
        
        // Подписываемся на изменение языка если нужно
        if (updateOnLanguageChange)
        {
            LocalizationManager.OnLanguageChanged += UpdateButtonText;
        }
    }
    
    private void Start()
    {
        UpdateButtonText();
    }
    
    private void OnDestroy()
    {
        if (updateOnLanguageChange)
        {
            LocalizationManager.OnLanguageChanged -= UpdateButtonText;
        }
    }
    
    public void UpdateButtonText()
    {
        if (_buttonText == null) return;
        
        string localizedText = GetLocalizedText();
        _buttonText.text = localizedText;
    }
    
    private string GetLocalizedText()
    {
        // Если ключ не задан, используем оригинальный текст
        if (string.IsNullOrEmpty(localizationKey))
        {
            return _originalText;
        }
        
        // Пытаемся получить локализованный текст из разных источников
        
        // 1. UI тексты
        string uiText = GetUIText(localizationKey);
        if (!string.IsNullOrEmpty(uiText) && uiText != localizationKey)
        {
            return uiText;
        }
        
        // 2. Тексты пациентов
        string patientText = GetPatientText(localizationKey);
        if (!string.IsNullOrEmpty(patientText) && patientText != localizationKey)
        {
            return patientText;
        }
        
        // Если ничего не найдено, возвращаем ключ или оригинальный текст
        return string.IsNullOrEmpty(_originalText) ? localizationKey : _originalText;
    }
    
    private string GetUIText(string key)
    {
        try
        {
            var uiTexts = LocalizationManager.CurrentLanguage?.UITexts;
            if (uiTexts == null) return key;
            
            // Используем рефлексию для получения свойства по имени
            var property = typeof(UITexts).GetProperty(key);
            if (property != null && property.PropertyType == typeof(string))
            {
                return property.GetValue(uiTexts) as string;
            }
            
            // Для массивов используем индекс (например "StaircaseNotCured_0")
            if (key.Contains("_"))
            {
                string[] parts = key.Split('_');
                if (parts.Length == 2)
                {
                    string arrayName = parts[0];
                    if (int.TryParse(parts[1], out int index))
                    {
                        var arrayProperty = typeof(UITexts).GetProperty(arrayName);
                        if (arrayProperty != null && arrayProperty.PropertyType == typeof(string[]))
                        {
                            string[] array = arrayProperty.GetValue(uiTexts) as string[];
                            if (array != null && index >= 0 && index < array.Length)
                            {
                                return array[index];
                            }
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Ошибка при получении UI текста для кнопки с ключом '{key}': {e.Message}");
        }
        
        return key;
    }
    
    private string GetPatientText(string key)
    {
        try
        {
            var patientTexts = LocalizationManager.CurrentLanguage?.PatientTexts;
            if (patientTexts == null) return key;
            
            // Проверяем имена пациентов
            if (key.StartsWith("patient_name_"))
            {
                if (int.TryParse(key.Substring("patient_name_".Length), out int index))
                {
                    if (index >= 0 && index < patientTexts.PatientNames.Length)
                    {
                        return patientTexts.PatientNames[index];
                    }
                }
            }
            
            // Проверяем диагнозы
            if (key.StartsWith("diagnosis_"))
            {
                if (int.TryParse(key.Substring("diagnosis_".Length), out int index))
                {
                    if (index >= 0 && index < patientTexts.Diagnoses.Length)
                    {
                        return patientTexts.Diagnoses[index];
                    }
                }
            }
            
            // Проверяем анамнезы
            if (key.StartsWith("anamnesis_"))
            {
                if (int.TryParse(key.Substring("anamnesis_".Length), out int index))
                {
                    if (index >= 0 && index < patientTexts.Anamnesis.Length)
                    {
                        return patientTexts.Anamnesis[index];
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Ошибка при получении текста пациента для кнопки с ключом '{key}': {e.Message}");
        }
        
        return key;
    }
    
    // Метод для установки ключа из кода
    public void SetLocalizationKey(string key)
    {
        localizationKey = key;
        UpdateButtonText();
    }
}
