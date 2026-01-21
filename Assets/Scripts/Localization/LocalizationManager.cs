using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Random = UnityEngine.Random;

// Enum типы для локализации
public enum HumorType
{
    Clownish,
    Verbal,
    Absurdist,
    Ironic
}

public enum PatientReactionType
{
    Angry,
    Happy,
    Neutral,
    BossContinue,
    BossFail
}

public enum UIKeyType
{
    RewardScreenTitle,
    SelectReward,
    LoadingFloor,
    ReturningToMenu,
    VictoryMessage
}

public enum InteractionPromptType
{
    WardDoor,
    WardExitDoor,
    WardExitDoorCured,
    MedicalRecord,
    InfoDesc,
    StaircaseAvailable,
    StaircaseBlocked
}

public enum StaircaseTextType
{
    NotCured,
    NotVisited
}

public enum InfoDescType
{
    Tutorial
}

public enum PatientDataType
{
    Name,
    Diagnosis,
    Anamnesis
}

public enum GagAnimationType
{
    Verbal,
    Ironic
}

// Классы данных для локализации
[Serializable]
public class LanguageData
{
    public string LanguageName;
    public string LanguageCode;
    
    [Header("Гэги по типам юмора")]
    public GagTexts GagTexts;
    
    [Header("Реакции пациентов")]
    public PatientReactionTexts PatientReactions;
    
    [Header("Интерфейс и системные сообщения")]
    public UITexts UITexts;
    
    [Header("Диалоги и сюжетные тексты")]
    public DialogueTexts DialogueTexts;
    
    [Header("Информация о пациентах")]
    public PatientTexts PatientTexts;
}

[Serializable]
public class GagTexts
{
    [Header("Клоунские гэги")]
    public string[] Clownish;
    
    [Header("Словесные гэги")]
    public string[] Verbal;
    
    [Header("Абсурдные гэги")]
    public string[] Absurdist;
    
    [Header("Ироничные гэги")]
    public string[] Ironic;
    
    [Header("Гэги для анимаций")]
    public string[] VerbalGag;
    public string[] IronicGag;
}

[Serializable]
public class PatientReactionTexts
{
    [Header("Злые реакции")]
    public string[] Angry;
    
    [Header("Радостные реакции")]
    public string[] Happy;
    
    [Header("Нейтральные реакции")]
    public string[] Neutral;
    
    [Header("Реакции босса")]
    public string[] BossContinue;
    public string[] BossFail;
}

[Serializable]
public class UITexts
{
    [Header("Экран наград")]
    public string RewardScreenTitle;
    public string SelectReward;
    
    [Header("Загрузка")]
    public string LoadingFloor;
    public string ReturningToMenu;
    
    [Header("Победа")]
    public string VictoryMessage;
    
    [Header("Лестница")]
    public string[] StaircaseNotCured;
    public string[] StaircaseNotVisited;
    
    [Header("Промпты интерактивных объектов")]
    public string WardDoorPrompt;
    public string WardExitDoorPrompt;
    public string WardExitDoorCuredPrompt;
    public string MedicalRecordPrompt;
    public string InfoDescPrompt;
    public string StaircaseAvailablePrompt;
    public string StaircaseBlockedPrompt;
}

[Serializable]
public class DialogueTexts
{
    public string DefaultGag;
    
    [Header("Вступление Бэтмена")]
    public string[] BatmanIntro;
    
    [Header("Завершение Бэтмена")]
    public string[] BatmanOutro;
    
    [Header("Подсказки")]
    public string[] Hints;
    
    [Header("Обучающий диалог InfoDesc")]
    public string[] Tutorial;
}

[Serializable]
public class PatientTexts
{
    [Header("Имена пациентов")]
    public string[] PatientNames;
    
    [Header("Диагнозы")]
    public string[] Diagnoses;
    
    [Header("Анамнезы")]
    public string[] Anamnesis;
}

public class LocalizationManager : MonoBehaviour
{
    private static LocalizationManager _instance;
    public static LocalizationManager Instance 
    { 
        get { return _instance; } 
    }
    
    private static Dictionary<string, LanguageData> _loadedLanguages = new Dictionary<string, LanguageData>();
    private static string _currentLanguageCode = "ru"; // Временно, изменится после загрузки
    
    public static string CurrentLanguageCode => _currentLanguageCode;
    public static string CurrentLanguageName => _loadedLanguages.ContainsKey(_currentLanguageCode) ? 
        _loadedLanguages[_currentLanguageCode].LanguageName : "Русский";
    
    public static LanguageData CurrentLanguage => _loadedLanguages.ContainsKey(_currentLanguageCode) ? 
        _loadedLanguages[_currentLanguageCode] : null;
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllLanguages();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void LoadAllLanguages()
    {
        _loadedLanguages.Clear();
        
        string languagesPath = Path.Combine(Application.streamingAssetsPath, "Localization");
        Debug.Log($"Попытка загрузки локализаций из: {languagesPath}");
        
        if (!Directory.Exists(languagesPath))
        {
            Debug.LogError($"Папка локализаций не найдена: {languagesPath}");
            Debug.LogError($"Application.streamingAssetsPath: {Application.streamingAssetsPath}");
            return;
        }
        
        string[] jsonFiles = Directory.GetFiles(languagesPath, "*.json");
        Debug.Log($"Найдено JSON файлов: {jsonFiles.Length}");
        
        foreach (string filePath in jsonFiles)
        {
            try
            {
                string jsonContent = File.ReadAllText(filePath);
                LanguageData languageData = JsonUtility.FromJson<LanguageData>(jsonContent);
                
                if (languageData != null && !string.IsNullOrEmpty(languageData.LanguageCode))
                {
                    _loadedLanguages[languageData.LanguageCode] = languageData;
                    Debug.Log($"Загружен язык: {languageData.LanguageName} ({languageData.LanguageCode}) из {Path.GetFileName(filePath)}");
                }
                else
                {
                    Debug.LogError($"Ошибка парсинга JSON из файла: {filePath}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Ошибка загрузки файла {filePath}: {e.Message}");
            }
        }
        
        Debug.Log($"Всего загружено языков: {_loadedLanguages.Count}");
        
        if (_loadedLanguages.Count == 0)
        {
            Debug.LogWarning("Не найдено ни одного файла локализации!");
        }
        else
        {
            // Устанавливаем язык по умолчанию после загрузки
            if (!_loadedLanguages.ContainsKey(_currentLanguageCode))
            {
                _currentLanguageCode = _loadedLanguages.Keys.First();
            }
            // Переключаем на английский если доступен
            if (_loadedLanguages.ContainsKey("en"))
            {
                _currentLanguageCode = "en";
            }
            Debug.Log($"Установлен язык по умолчанию: {CurrentLanguageName} ({CurrentLanguageCode})");
        }
    }
    
    public static void SetLanguage(string languageCode)
    {
        if (_loadedLanguages.ContainsKey(languageCode))
        {
            _currentLanguageCode = languageCode;
            Debug.Log($"Язык изменен на: {CurrentLanguageName} ({CurrentLanguageCode})");
        }
        else
        {
            Debug.LogWarning($"Язык с кодом '{languageCode}' не найден");
        }
    }
    
    public static string[] GetAvailableLanguages()
    {
        return _loadedLanguages.Values.Select(lang => lang.LanguageName).ToArray();
    }
    
    public static string[] GetAvailableLanguageCodes()
    {
        return _loadedLanguages.Keys.ToArray();
    }
    
    // Методы для получения текстов
    public static string GetGagText(HumorType gagType)
    {
        if (CurrentLanguage?.GagTexts == null) 
        {
            Debug.LogWarning("Тексты гэгов не загружены!");
            return "Текст не найден";
        }
        
        string[] gagArray;
        switch (gagType)
        {
            case HumorType.Clownish:
                gagArray = CurrentLanguage.GagTexts.Clownish;
                break;
            case HumorType.Verbal:
                gagArray = CurrentLanguage.GagTexts.Verbal;
                break;
            case HumorType.Absurdist:
                gagArray = CurrentLanguage.GagTexts.Absurdist;
                break;
            case HumorType.Ironic:
                gagArray = CurrentLanguage.GagTexts.Ironic;
                break;
            default:
                gagArray = new[] { CurrentLanguage.DialogueTexts.DefaultGag };
                break;
        }
        
        return gagArray[Random.Range(0, gagArray.Length)];
    }
    
    public static string GetGagAnimationText(GagAnimationType animationType)
    {
        if (CurrentLanguage?.GagTexts == null) 
        {
            Debug.LogWarning("Тексты гэгов не загружены!");
            return "Текст не найден";
        }
        
        string[] gagArray;
        switch (animationType)
        {
            case GagAnimationType.Verbal:
                gagArray = CurrentLanguage.GagTexts.VerbalGag;
                break;
            case GagAnimationType.Ironic:
                gagArray = CurrentLanguage.GagTexts.IronicGag;
                break;
            default:
                gagArray = new[] { CurrentLanguage.DialogueTexts.DefaultGag };
                break;
        }
        
        return gagArray[Random.Range(0, gagArray.Length)];
    }
    
    public static string GetPatientReaction(PatientReactionType reactionType)
    {
        if (CurrentLanguage?.PatientReactions == null) 
        {
            Debug.LogWarning("Тексты реакций пациентов не загружены!");
            return "Текст не найден";
        }
        
        string[] reactionArray;
        switch (reactionType)
        {
            case PatientReactionType.Angry:
                reactionArray = CurrentLanguage.PatientReactions.Angry;
                break;
            case PatientReactionType.Happy:
                reactionArray = CurrentLanguage.PatientReactions.Happy;
                break;
            case PatientReactionType.Neutral:
                reactionArray = CurrentLanguage.PatientReactions.Neutral;
                break;
            case PatientReactionType.BossContinue:
                reactionArray = CurrentLanguage.PatientReactions.BossContinue;
                break;
            case PatientReactionType.BossFail:
                reactionArray = CurrentLanguage.PatientReactions.BossFail;
                break;
            default:
                reactionArray = new[] { "..." };
                break;
        }
        
        return reactionArray[Random.Range(0, reactionArray.Length)];
    }
    
    public static string GetUIText(UIKeyType keyType)
    {
        if (CurrentLanguage?.UITexts == null) 
        {
            Debug.LogWarning("UI тексты не загружены!");
            return "Текст не найден";
        }
        
        switch (keyType)
        {
            case UIKeyType.RewardScreenTitle:
                return CurrentLanguage.UITexts.RewardScreenTitle;
            case UIKeyType.SelectReward:
                return CurrentLanguage.UITexts.SelectReward;
            case UIKeyType.LoadingFloor:
                return CurrentLanguage.UITexts.LoadingFloor;
            case UIKeyType.ReturningToMenu:
                return CurrentLanguage.UITexts.ReturningToMenu;
            case UIKeyType.VictoryMessage:
                return CurrentLanguage.UITexts.VictoryMessage;
            default:
                return "Текст не найден";
        }
    }
    
    public static string GetInteractionPrompt(InteractionPromptType promptType)
    {
        if (CurrentLanguage?.UITexts == null) 
        {
            Debug.LogWarning("UI тексты не загружены!");
            return "Текст не найден";
        }
        
        string result;
        switch (promptType)
        {
            case InteractionPromptType.WardDoor:
                result = CurrentLanguage.UITexts.WardDoorPrompt;
                break;
            case InteractionPromptType.WardExitDoor:
                result = CurrentLanguage.UITexts.WardExitDoorPrompt;
                break;
            case InteractionPromptType.WardExitDoorCured:
                result = CurrentLanguage.UITexts.WardExitDoorCuredPrompt;
                break;
            case InteractionPromptType.MedicalRecord:
                result = CurrentLanguage.UITexts.MedicalRecordPrompt;
                break;
            case InteractionPromptType.InfoDesc:
                result = CurrentLanguage.UITexts.InfoDescPrompt;
                break;
            case InteractionPromptType.StaircaseAvailable:
                result = CurrentLanguage.UITexts.StaircaseAvailablePrompt;
                break;
            case InteractionPromptType.StaircaseBlocked:
                result = CurrentLanguage.UITexts.StaircaseBlockedPrompt;
                break;
            default:
                result = "Текст не найден";
                break;
        }
        
        Debug.Log($"GetInteractionPrompt({promptType}) -> '{result}' (текущий язык: {CurrentLanguageCode})");
        return result;
    }
    
    public static string GetStaircaseText(StaircaseTextType textType, int remainingPatients = 0)
    {
        if (CurrentLanguage?.UITexts == null) 
        {
            Debug.LogWarning("UI тексты не загружены!");
            return "Лестница недоступна";
        }
        
        switch (textType)
        {
            case StaircaseTextType.NotCured:
                string[] notCuredTexts = CurrentLanguage.UITexts.StaircaseNotCured;
                if (notCuredTexts.Length > 0)
                {
                    string template = notCuredTexts[Random.Range(0, notCuredTexts.Length)];
                    return template.Replace("{count}", remainingPatients.ToString());
                }
                break;
                
            case StaircaseTextType.NotVisited:
                string[] notVisitedTexts = CurrentLanguage.UITexts.StaircaseNotVisited;
                if (notVisitedTexts.Length > 0)
                    return notVisitedTexts[Random.Range(0, notVisitedTexts.Length)];
                break;
        }
        
        return "Лестница недоступна";
    }
    
    public static string[] GetDialogueTexts(string dialogueType)
    {
        if (CurrentLanguage?.DialogueTexts == null) 
        {
            Debug.LogWarning("Диалоговые тексты не загружены!");
            return new[] { "Текст не найден" };
        }
        
        switch (dialogueType.ToLower())
        {
            case "batmanintro":
                return CurrentLanguage.DialogueTexts.BatmanIntro;
            case "batmanoutro":
                return CurrentLanguage.DialogueTexts.BatmanOutro;
            case "hints":
                return CurrentLanguage.DialogueTexts.Hints;
            default:
                return new[] { CurrentLanguage.DialogueTexts.DefaultGag };
        }
    }
    
    public static string[] GetInfoDescTexts(InfoDescType infoType)
    {
        if (CurrentLanguage?.DialogueTexts == null) 
        {
            Debug.LogWarning("Диалоговые тексты не загружены!");
            return new[] { "Текст не найден" };
        }
        
        switch (infoType)
        {
            case InfoDescType.Tutorial:
                return CurrentLanguage.DialogueTexts.Tutorial ?? new[] { "Информация недоступна" };
            default:
                return new[] { "Информация недоступна" };
        }
    }
    
    public static string GetPatientName(string key)
    {
        if (CurrentLanguage?.PatientTexts == null) 
        {
            Debug.LogWarning("Тексты пациентов не загружены!");
            return "Имя не найдено";
        }
        
        // Ищем по ключу в массиве имен
        // Для простоты используем индексацию по первым символам ключа
        int index = Mathf.Abs(key.GetHashCode()) % CurrentLanguage.PatientTexts.PatientNames.Length;
        return CurrentLanguage.PatientTexts.PatientNames[index];
    }
    
    public static string GetPatientDiagnosis(string key)
    {
        if (CurrentLanguage?.PatientTexts == null) 
        {
            Debug.LogWarning("Тексты пациентов не загружены!");
            return "Диагноз не найден";
        }
        
        // Ищем по ключу в массиве диагнозов
        int index = Mathf.Abs(key.GetHashCode()) % CurrentLanguage.PatientTexts.Diagnoses.Length;
        return CurrentLanguage.PatientTexts.Diagnoses[index];
    }
    
    public static string GetPatientAnamnesis(string key)
    {
        if (CurrentLanguage?.PatientTexts == null) 
        {
            Debug.LogWarning("Тексты пациентов не загружены!");
            return "Анамнез не найден";
        }
        
        // Ищем по ключу в массиве анамнезов
        int index = Mathf.Abs(key.GetHashCode()) % CurrentLanguage.PatientTexts.Anamnesis.Length;
        return CurrentLanguage.PatientTexts.Anamnesis[index];
    }
}
