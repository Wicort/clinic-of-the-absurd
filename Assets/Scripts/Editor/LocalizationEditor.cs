using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

[CustomEditor(typeof(LocalizedText))]
public class LocalizedTextEditor : Editor
{
    private SerializedProperty localizationKeyProp;
    private SerializedProperty useUpperCaseProp;
    private SerializedProperty updateOnLanguageChangeProp;
    
    private void OnEnable()
    {
        localizationKeyProp = serializedObject.FindProperty("localizationKey");
        useUpperCaseProp = serializedObject.FindProperty("useUpperCase");
        updateOnLanguageChangeProp = serializedObject.FindProperty("updateOnLanguageChange");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(localizationKeyProp, new GUIContent("Ключ локализации", "Ключ для поиска текста в файлах локализации"));
        EditorGUILayout.PropertyField(useUpperCaseProp, new GUIContent("Верхний регистр", "Преобразовать текст в верхний регистр"));
        EditorGUILayout.PropertyField(updateOnLanguageChangeProp, new GUIContent("Обновлять при смене языка", "Автоматически обновлять текст при смене языка"));
        
        // Показываем текущий текст
        LocalizedText localizedText = (LocalizedText)target;
        Text textComponent = localizedText.GetComponent<Text>();
        if (textComponent != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Текущий текст:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(textComponent.text, EditorStyles.wordWrappedLabel);
        }
        
        // Кнопка для тестирования локализации
        if (!string.IsNullOrEmpty(localizationKeyProp.stringValue))
        {
            EditorGUILayout.Space();
            if (GUILayout.Button("Тестировать локализацию"))
            {
                TestLocalization(localizedText);
            }
        }
        
        // Отладочная информация
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Отладка:", EditorStyles.boldLabel);
        if (GUILayout.Button("Перезагрузить локализацию"))
        {
            LocalizationManager.ReloadLocalization();
        }
        
        // Прямая проверка JSON файлов
        if (GUILayout.Button("Проверить JSON файлы"))
        {
            CheckJSONFiles();
        }
        
        // Показываем текущее значение PressE для отладки
        try
        {
            var currentLanguage = LocalizationManager.CurrentLanguage;
            if (currentLanguage?.UITexts != null)
            {
                EditorGUILayout.LabelField($"PressE ({currentLanguage.LanguageCode}): {currentLanguage.UITexts.PressE}");
            }
            else
            {
                EditorGUILayout.LabelField("UITexts: null");
            }
        }
        catch (System.Exception e)
        {
            EditorGUILayout.LabelField($"Ошибка: {e.Message}");
        }
        
        // Кнопка для добавления常见 ключей
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Быстрые ключи:", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("UI тексты"))
        {
            ShowUIKeysMenu(localizedText);
        }
        if (GUILayout.Button("Пациенты"))
        {
            ShowPatientKeysMenu(localizedText);
        }
        EditorGUILayout.EndHorizontal();
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void TestLocalization(LocalizedText localizedText)
    {
        if (Application.isPlaying)
        {
            localizedText.UpdateText();
        }
        else
        {
            // В редакторе показываем превью
            string preview = GetPreviewText(localizedText.localizationKey);
            Text textComponent = localizedText.GetComponent<Text>();
            if (textComponent != null)
            {
                textComponent.text = preview;
            }
        }
    }
    
    private string GetPreviewText(string key)
    {
        // Пытаемся загрузить текст из текущего языка
        try
        {
            var currentLanguage = LocalizationManager.CurrentLanguage;
            if (currentLanguage?.UITexts != null)
            {
                var property = typeof(UITexts).GetProperty(key);
                if (property != null && property.PropertyType == typeof(string))
                {
                    return property.GetValue(currentLanguage.UITexts) as string;
                }
            }
        }
        catch
        {
            // Игнорируем ошибки в редакторе
        }
        
        return $"[{key}]";
    }
    
    private void ShowUIKeysMenu(LocalizedText localizedText)
    {
        GenericMenu menu = new GenericMenu();
        
        string[] uiKeys = {
            "RewardScreenTitle", "SelectReward", "LoadingFloor", "ReturningToMenu",
            "VictoryMessage", "WardDoorPrompt", "WardExitDoorPrompt", "WardExitDoorCuredPrompt",
            "MedicalRecordPrompt", "InfoDescPrompt", "StaircaseAvailablePrompt", "StaircaseBlockedPrompt",
            "MedicalRecordPatient", "MedicalRecordDiagnosis", "MedicalRecordAnamnesis", "PressE"
        };
        
        foreach (string key in uiKeys)
        {
            menu.AddItem(new GUIContent(key), false, () => {
                serializedObject.Update();
                localizationKeyProp.stringValue = key;
                serializedObject.ApplyModifiedProperties();
                TestLocalization(localizedText);
            });
        }
        
        menu.ShowAsContext();
    }
    
    private void ShowPatientKeysMenu(LocalizedText localizedText)
    {
        GenericMenu menu = new GenericMenu();
        
        // Имена пациентов
        menu.AddItem(new GUIContent("Пациенты/Имена/Анна Семёновна"), false, () => SetKey("patient_name_0", localizedText));
        menu.AddItem(new GUIContent("Пациенты/Имена/Аркадий Скептик"), false, () => SetKey("patient_name_1", localizedText));
        menu.AddItem(new GUIContent("Пациенты/Имена/Борис-Кувырок"), false, () => SetKey("patient_name_2", localizedText));
        menu.AddItem(new GUIContent("Пациенты/Имена/Вера Реалист"), false, () => SetKey("patient_name_3", localizedText));
        menu.AddItem(new GUIContent("Пациенты/Имена/Витя-Прыгун"), false, () => SetKey("patient_name_4", localizedText));
        menu.AddItem(new GUIContent("Пациенты/Имена/Главврач Грустин"), false, () => SetKey("patient_name_5", localizedText));
        menu.AddItem(new GUIContent("Пациенты/Имена/Дядя Миша"), false, () => SetKey("patient_name_6", localizedText));
        menu.AddItem(new GUIContent("Пациенты/Имена/Зинаида-Космос"), false, () => SetKey("patient_name_7", localizedText));
        menu.AddItem(new GUIContent("Пациенты/Имена/Лёва Облачко"), false, () => SetKey("patient_name_8", localizedText));
        menu.AddItem(new GUIContent("Пациенты/Имена/Профессор Буквоед"), false, () => SetKey("patient_name_9", localizedText));
        menu.AddItem(new GUIContent("Пациенты/Имена/Соня-Нос"), false, () => SetKey("patient_name_10", localizedText));
        
        menu.ShowAsContext();
    }
    
    private void SetKey(string key, LocalizedText localizedText)
    {
        serializedObject.Update();
        localizationKeyProp.stringValue = key;
        serializedObject.ApplyModifiedProperties();
        TestLocalization(localizedText);
    }
    
    private void CheckJSONFiles()
    {
        string localizationPath = Path.Combine(Application.streamingAssetsPath, "Localization");
        
        if (!Directory.Exists(localizationPath))
        {
            Debug.LogError($"Папка локализации не найдена: {localizationPath}");
            return;
        }
        
        string[] jsonFiles = Directory.GetFiles(localizationPath, "*.json");
        
        foreach (string filePath in jsonFiles)
        {
            try
            {
                string jsonContent = File.ReadAllText(filePath);
                var languageData = JsonUtility.FromJson<LanguageData>(jsonContent);
                
                if (languageData?.UITexts != null)
                {
                    Debug.Log($"{Path.GetFileNameWithoutExtension(filePath)}: PressE = '{languageData.UITexts.PressE}'");
                }
                else
                {
                    Debug.LogWarning($"{Path.GetFileNameWithoutExtension(filePath)}: UITexts = null");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Ошибка чтения {Path.GetFileName(filePath)}: {e.Message}");
            }
        }
    }
}

[CustomEditor(typeof(LocalizedButton))]
public class LocalizedButtonEditor : Editor
{
    private SerializedProperty localizationKeyProp;
    private SerializedProperty updateOnLanguageChangeProp;
    
    private void OnEnable()
    {
        localizationKeyProp = serializedObject.FindProperty("localizationKey");
        updateOnLanguageChangeProp = serializedObject.FindProperty("updateOnLanguageChange");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(localizationKeyProp, new GUIContent("Ключ локализации", "Ключ для поиска текста в файлах локализации"));
        EditorGUILayout.PropertyField(updateOnLanguageChangeProp, new GUIContent("Обновлять при смене языка", "Автоматически обновлять текст при смене языка"));
        
        // Показываем текущий текст на кнопке
        LocalizedButton localizedButton = (LocalizedButton)target;
        Text buttonText = localizedButton.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Текущий текст:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(buttonText.text, EditorStyles.wordWrappedLabel);
        }
        
        // Кнопка для тестирования локализации
        if (!string.IsNullOrEmpty(localizationKeyProp.stringValue))
        {
            EditorGUILayout.Space();
            if (GUILayout.Button("Тестировать локализацию"))
            {
                TestLocalization(localizedButton);
            }
        }
        
        // Кнопка для добавления быстрых ключей
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Быстрые ключи:", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("UI тексты"))
        {
            ShowUIKeysMenu(localizedButton);
        }
        if (GUILayout.Button("Пациенты"))
        {
            ShowPatientKeysMenu(localizedButton);
        }
        EditorGUILayout.EndHorizontal();
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void TestLocalization(LocalizedButton localizedButton)
    {
        if (Application.isPlaying)
        {
            localizedButton.UpdateButtonText();
        }
        else
        {
            // В редакторе показываем превью
            string preview = GetPreviewText(localizedButton.localizationKey);
            Text buttonText = localizedButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = preview;
            }
        }
    }
    
    private string GetPreviewText(string key)
    {
        // Пытаемся загрузить текст из текущего языка
        try
        {
            var currentLanguage = LocalizationManager.CurrentLanguage;
            if (currentLanguage?.UITexts != null)
            {
                var property = typeof(UITexts).GetProperty(key);
                if (property != null && property.PropertyType == typeof(string))
                {
                    return property.GetValue(currentLanguage.UITexts) as string;
                }
            }
        }
        catch
        {
            // Игнорируем ошибки в редакторе
        }
        
        return $"[{key}]";
    }
    
    private void ShowUIKeysMenu(LocalizedButton localizedButton)
    {
        GenericMenu menu = new GenericMenu();
        
        string[] uiKeys = {
            "RewardScreenTitle", "SelectReward", "LoadingFloor", "ReturningToMenu",
            "VictoryMessage", "WardDoorPrompt", "WardExitDoorPrompt", "WardExitDoorCuredPrompt",
            "MedicalRecordPrompt", "InfoDescPrompt", "StaircaseAvailablePrompt", "StaircaseBlockedPrompt",
            "MedicalRecordPatient", "MedicalRecordDiagnosis", "MedicalRecordAnamnesis", "PressE"
        };
        
        foreach (string key in uiKeys)
        {
            menu.AddItem(new GUIContent(key), false, () => {
                serializedObject.Update();
                localizationKeyProp.stringValue = key;
                serializedObject.ApplyModifiedProperties();
                TestLocalization(localizedButton);
            });
        }
        
        menu.ShowAsContext();
    }
    
    private void ShowPatientKeysMenu(LocalizedButton localizedButton)
    {
        GenericMenu menu = new GenericMenu();
        
        // Имена пациентов
        menu.AddItem(new GUIContent("Пациенты/Имена/Анна Семёновна"), false, () => SetKey("patient_name_0", localizedButton));
        menu.AddItem(new GUIContent("Пациенты/Имена/Аркадий Скептик"), false, () => SetKey("patient_name_1", localizedButton));
        menu.AddItem(new GUIContent("Пациенты/Имена/Борис-Кувырок"), false, () => SetKey("patient_name_2", localizedButton));
        menu.AddItem(new GUIContent("Пациенты/Имена/Вера Реалист"), false, () => SetKey("patient_name_3", localizedButton));
        menu.AddItem(new GUIContent("Пациенты/Имена/Витя-Прыгун"), false, () => SetKey("patient_name_4", localizedButton));
        menu.AddItem(new GUIContent("Пациенты/Имена/Главврач Грустин"), false, () => SetKey("patient_name_5", localizedButton));
        menu.AddItem(new GUIContent("Пациенты/Имена/Дядя Миша"), false, () => SetKey("patient_name_6", localizedButton));
        menu.AddItem(new GUIContent("Пациенты/Имена/Зинаида-Космос"), false, () => SetKey("patient_name_7", localizedButton));
        menu.AddItem(new GUIContent("Пациенты/Имена/Лёва Облачко"), false, () => SetKey("patient_name_8", localizedButton));
        menu.AddItem(new GUIContent("Пациенты/Имена/Профессор Буквоед"), false, () => SetKey("patient_name_9", localizedButton));
        menu.AddItem(new GUIContent("Пациенты/Имена/Соня-Нос"), false, () => SetKey("patient_name_10", localizedButton));
        
        menu.ShowAsContext();
    }
    
    private void SetKey(string key, LocalizedButton localizedButton)
    {
        serializedObject.Update();
        localizationKeyProp.stringValue = key;
        serializedObject.ApplyModifiedProperties();
        TestLocalization(localizedButton);
    }
    
    private void CheckJSONFiles()
    {
        string localizationPath = Path.Combine(Application.streamingAssetsPath, "Localization");
        
        if (!Directory.Exists(localizationPath))
        {
            Debug.LogError($"Папка локализации не найдена: {localizationPath}");
            return;
        }
        
        string[] jsonFiles = Directory.GetFiles(localizationPath, "*.json");
        
        foreach (string filePath in jsonFiles)
        {
            try
            {
                string jsonContent = File.ReadAllText(filePath);
                var languageData = JsonUtility.FromJson<LanguageData>(jsonContent);
                
                if (languageData?.UITexts != null)
                {
                    Debug.Log($"{Path.GetFileNameWithoutExtension(filePath)}: PressE = '{languageData.UITexts.PressE}'");
                }
                else
                {
                    Debug.LogWarning($"{Path.GetFileNameWithoutExtension(filePath)}: UITexts = null");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Ошибка чтения {Path.GetFileName(filePath)}: {e.Message}");
            }
        }
    }
}
