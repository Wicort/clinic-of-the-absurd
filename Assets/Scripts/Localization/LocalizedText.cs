using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizedText : MonoBehaviour
{
    [Header("Настройки локализации")]
    [SerializeField] public string localizationKey = "";
    [SerializeField] private bool useUpperCase = false;
    [SerializeField] private bool updateOnLanguageChange = true;
    
    private Text _textComponent;
    private string _originalText;
    
    private void Awake()
    {
        _textComponent = GetComponent<Text>();
        _originalText = _textComponent.text;
        
        // Подписываемся на изменение языка если нужно
        if (updateOnLanguageChange)
        {
            LocalizationManager.OnLanguageChanged += UpdateText;
        }
    }
    
    private void Start()
    {
        UpdateText();
    }
    
    private void OnDestroy()
    {
        if (updateOnLanguageChange)
        {
            LocalizationManager.OnLanguageChanged -= UpdateText;
        }
    }
    
    public void UpdateText()
    {
        if (_textComponent == null) return;
        
        string localizedText = GetLocalizedText();
        
        if (useUpperCase)
        {
            localizedText = localizedText.ToUpper();
        }
        
        _textComponent.text = localizedText;
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
        
        // 3. Тексты гэгов
        string gagText = GetGagText(localizationKey);
        if (!string.IsNullOrEmpty(gagText) && gagText != localizationKey)
        {
            return gagText;
        }
        
        // Если ничего не найдено, возвращаем ключ или оригинальный текст
        return string.IsNullOrEmpty(_originalText) ? localizationKey : _originalText;
    }
    
    private string GetUIText(string key)
    {
        try
        {
            var uiTexts = LocalizationManager.CurrentLanguage?.UITexts;
            if (uiTexts == null) 
            {
                return key;
            }
            
            // Прямая проверка для PressE (обход проблемы с рефлексией)
            if (key == "PressE")
            {
                string pressEValue = uiTexts.PressE;
                if (!string.IsNullOrEmpty(pressEValue))
                {
                    return pressEValue;
                }
            }
            
            // Прямые проверки для других полей
            if (key == "MedicalRecordPatient")
            {
                return uiTexts.MedicalRecordPatient;
            }
            
            if (key == "MedicalRecordDiagnosis")
            {
                return uiTexts.MedicalRecordDiagnosis;
            }
            
            if (key == "MedicalRecordAnamnesis")
            {
                return uiTexts.MedicalRecordAnamnesis;
            }
            
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
            Debug.LogError($"Ошибка при получении UI текста для ключа '{key}': {e.Message}");
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
            Debug.LogWarning($"Ошибка при получении текста пациента для ключа '{key}': {e.Message}");
        }
        
        return key;
    }
    
    private string GetGagText(string key)
    {
        try
        {
            var gagTexts = LocalizationManager.CurrentLanguage?.GagTexts;
            if (gagTexts == null) return key;
            
            // Проверяем гэги по типу
            if (key.StartsWith("gag_"))
            {
                string gagType = key.Substring("gag_".Length);
                return LocalizationManager.GetGagText(gagType);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Ошибка при получении гэга для ключа '{key}': {e.Message}");
        }
        
        return key;
    }
    
    // Метод для установки ключа из кода
    public void SetLocalizationKey(string key)
    {
        localizationKey = key;
        UpdateText();
    }
}
