using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UILocalizer : MonoBehaviour
{
    [Header("Настройки автоматической локализации")]
    [SerializeField] private bool localizeOnStart = true;
    [SerializeField] private bool localizeOnEnable = true;
    [SerializeField] private bool findChildrenRecursively = true;
    
    [Header("Ключи для автоматической локализации")]
    [SerializeField] private List<UIElementMapping> elementMappings = new List<UIElementMapping>();
    
    [System.Serializable]
    public class UIElementMapping
    {
        public string elementName;
        public string localizationKey;
        public bool useUpperCase = false;
    }
    
    private Dictionary<string, string> _nameToKeyMap = new Dictionary<string, string>();
    
    private void Awake()
    {
        // Создаем словарь для быстрого поиска
        foreach (var mapping in elementMappings)
        {
            if (!string.IsNullOrEmpty(mapping.elementName))
            {
                _nameToKeyMap[mapping.elementName] = mapping.localizationKey;
            }
        }
    }
    
    private void Start()
    {
        if (localizeOnStart)
        {
            LocalizeAllUI();
        }
    }
    
    private void OnEnable()
    {
        if (localizeOnEnable)
        {
            LocalizeAllUI();
        }
        
        // Подписываемся на изменение языка
        LocalizationManager.OnLanguageChanged += LocalizeAllUI;
    }
    
    private void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= LocalizeAllUI;
    }
    
    public void LocalizeAllUI()
    {
        // Находим все Text компоненты
        LocalizeTextComponents();
        
        // Находим все Button компоненты
        LocalizeButtonComponents();
        
        // Применяем маппинги
        ApplyElementMappings();
    }
    
    private void LocalizeTextComponents()
    {
        Text[] textComponents = findChildrenRecursively ? 
            GetComponentsInChildren<Text>(true) : 
            GetComponents<Text>();
        
        foreach (Text textComponent in textComponents)
        {
            LocalizedText localizedText = textComponent.GetComponent<LocalizedText>();
            if (localizedText != null)
            {
                // Если уже есть компонент локализации, просто обновляем его
                localizedText.UpdateText();
            }
            else
            {
                // Пытаемся локализовать по имени объекта
                string localizationKey = GetLocalizationKeyByName(textComponent.gameObject.name);
                if (!string.IsNullOrEmpty(localizationKey))
                {
                    string localizedTextValue = GetLocalizedText(localizationKey);
                    if (!string.IsNullOrEmpty(localizedTextValue) && localizedTextValue != localizationKey)
                    {
                        textComponent.text = localizedTextValue;
                    }
                }
            }
        }
    }
    
    private void LocalizeButtonComponents()
    {
        Button[] buttonComponents = findChildrenRecursively ? 
            GetComponentsInChildren<Button>(true) : 
            GetComponents<Button>();
        
        foreach (Button buttonComponent in buttonComponents)
        {
            LocalizedButton localizedButton = buttonComponent.GetComponent<LocalizedButton>();
            if (localizedButton != null)
            {
                // Если уже есть компонент локализации, просто обновляем его
                localizedButton.UpdateButtonText();
            }
            else
            {
                // Пытаемся локализовать текст на кнопке по имени объекта
                Text buttonText = buttonComponent.GetComponentInChildren<Text>();
                if (buttonText != null)
                {
                    string localizationKey = GetLocalizationKeyByName(buttonComponent.gameObject.name);
                    if (!string.IsNullOrEmpty(localizationKey))
                    {
                        string localizedTextValue = GetLocalizedText(localizationKey);
                        if (!string.IsNullOrEmpty(localizedTextValue) && localizedTextValue != localizationKey)
                        {
                            buttonText.text = localizedTextValue;
                        }
                    }
                }
            }
        }
    }
    
    private void ApplyElementMappings()
    {
        foreach (var mapping in elementMappings)
        {
            if (string.IsNullOrEmpty(mapping.elementName) || string.IsNullOrEmpty(mapping.localizationKey))
                continue;
            
            GameObject target = findChildrenRecursively ? 
                FindChildRecursive(transform, mapping.elementName) : 
                transform.Find(mapping.elementName)?.gameObject;
            
            if (target != null)
            {
                string localizedText = GetLocalizedText(mapping.localizationKey);
                if (!string.IsNullOrEmpty(localizedText) && localizedText != mapping.localizationKey)
                {
                    if (mapping.useUpperCase)
                    {
                        localizedText = localizedText.ToUpper();
                    }
                    
                    // Пытаемся найти Text компонент
                    Text textComponent = target.GetComponent<Text>();
                    if (textComponent != null)
                    {
                        textComponent.text = localizedText;
                        continue;
                    }
                    
                    // Пытаемся найти Text на дочерних объектах (для кнопок)
                    Text childText = target.GetComponentInChildren<Text>();
                    if (childText != null)
                    {
                        childText.text = localizedText;
                    }
                }
            }
        }
    }
    
    private string GetLocalizationKeyByName(string objectName)
    {
        // Пробуем найти в маппингах
        if (_nameToKeyMap.ContainsKey(objectName))
        {
            return _nameToKeyMap[objectName];
        }
        
        // Генерируем ключ на основе имени объекта
        // Например: "StartButton" -> "StartButton"
        // "TitleText" -> "TitleText"
        return objectName;
    }
    
    private string GetLocalizedText(string key)
    {
        // Используем ту же логику что и в LocalizedText
        if (string.IsNullOrEmpty(key))
            return key;
        
        // 1. UI тексты
        string uiText = GetUIText(key);
        if (!string.IsNullOrEmpty(uiText) && uiText != key)
        {
            return uiText;
        }
        
        // 2. Тексты пациентов
        string patientText = GetPatientText(key);
        if (!string.IsNullOrEmpty(patientText) && patientText != key)
        {
            return patientText;
        }
        
        return key;
    }
    
    private string GetUIText(string key)
    {
        try
        {
            var uiTexts = LocalizationManager.CurrentLanguage?.UITexts;
            if (uiTexts == null) return key;
            
            var property = typeof(UITexts).GetProperty(key);
            if (property != null && property.PropertyType == typeof(string))
            {
                return property.GetValue(uiTexts) as string;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Ошибка при получении UI текста для ключа '{key}': {e.Message}");
        }
        
        return key;
    }
    
    private string GetPatientText(string key)
    {
        try
        {
            var patientTexts = LocalizationManager.CurrentLanguage?.PatientTexts;
            if (patientTexts == null) return key;
            
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
    
    private GameObject FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child.gameObject;
            
            GameObject found = FindChildRecursive(child, name);
            if (found != null)
                return found;
        }
        return null;
    }
    
    // Публичный метод для добавления маппинга во время выполнения
    public void AddElementMapping(string elementName, string localizationKey, bool useUpperCase = false)
    {
        elementMappings.Add(new UIElementMapping 
        { 
            elementName = elementName, 
            localizationKey = localizationKey,
            useUpperCase = useUpperCase
        });
        
        _nameToKeyMap[elementName] = localizationKey;
    }
}
