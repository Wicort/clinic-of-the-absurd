using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class LocalizationRebuilder : EditorWindow
{
    [MenuItem("Tools/Rebuild Patient Localization")]
    public static void ShowWindow()
    {
        GetWindow<LocalizationRebuilder>("Rebuild Patient Localization");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Rebuild Patient Localization", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        GUILayout.Label("This will extract REAL patient data and rebuild localization files.");
        GUILayout.Label("Step 1: Extract patient data");
        GUILayout.Label("Step 2: Update JSON files");
        GUILayout.Label("Step 3: Update patient keys");
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Extract Patient Data"))
        {
            ExtractPatientData();
        }
        
        if (GUILayout.Button("Rebuild Localization Files"))
        {
            RebuildLocalizationFiles();
        }
        
        if (GUILayout.Button("Update Patient Keys"))
        {
            UpdatePatientKeys();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Complete Rebuild (All Steps)"))
        {
            CompleteRebuild();
        }
    }
    
    private Dictionary<string, PatientData> extractedPatients = new Dictionary<string, PatientData>();
    private HashSet<string> uniqueNames = new HashSet<string>();
    private HashSet<string> uniqueDiagnoses = new HashSet<string>();
    private HashSet<string> uniqueAnamnesis = new HashSet<string>();
    
    void ExtractPatientData()
    {
        Debug.Log("=== EXTRACTING REAL PATIENT DATA ===");
        
        extractedPatients.Clear();
        uniqueNames.Clear();
        uniqueDiagnoses.Clear();
        uniqueAnamnesis.Clear();
        
        string[] guids = AssetDatabase.FindAssets("t:PatientProfile");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            PatientProfile profile = AssetDatabase.LoadAssetAtPath<PatientProfile>(path);
            
            if (profile != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                var data = new PatientData();
                
                data.fileName = fileName;
                data.patientName = profile.patientName?.Trim() ?? "";
                data.diagnosis = profile.diagnosis?.Trim() ?? "";
                data.anamnesisLines = profile.anamnesisLines?.Select(line => line?.Trim() ?? "").ToArray() ?? new string[0];
                
                extractedPatients[fileName] = data;
                
                // Собираем уникальные значения
                if (!string.IsNullOrEmpty(data.patientName))
                    uniqueNames.Add(data.patientName);
                
                if (!string.IsNullOrEmpty(data.diagnosis))
                    uniqueDiagnoses.Add(data.diagnosis);
                
                foreach (string anamnesis in data.anamnesisLines)
                {
                    if (!string.IsNullOrEmpty(anamnesis))
                        uniqueAnamnesis.Add(anamnesis);
                }
                
                Debug.Log($"=== {fileName} ===");
                Debug.Log($"Name: '{data.patientName}'");
                Debug.Log($"Diagnosis: '{data.diagnosis}'");
                Debug.Log($"Anamnesis lines: {data.anamnesisLines.Length}");
                
                for (int i = 0; i < data.anamnesisLines.Length; i++)
                {
                    Debug.Log($"  {i+1}: '{data.anamnesisLines[i]}'");
                }
                Debug.Log("");
            }
        }
        
        Debug.Log($"Extracted {extractedPatients.Count} patients");
        Debug.Log($"Found {uniqueNames.Count} unique names");
        Debug.Log($"Found {uniqueDiagnoses.Count} unique diagnoses");
        Debug.Log($"Found {uniqueAnamnesis.Count} unique anamnesis lines");
        
        EditorUtility.DisplayDialog("Data Extracted", 
            $"Extracted data from {extractedPatients.Count} patients:\n" +
            $"{uniqueNames.Count} names\n" +
            $"{uniqueDiagnoses.Count} diagnoses\n" +
            $"{uniqueAnamnesis.Count} anamnesis lines", "OK");
    }
    
    void RebuildLocalizationFiles()
    {
        if (extractedPatients.Count == 0)
        {
            EditorUtility.DisplayDialog("No Data", "Please extract patient data first!", "OK");
            return;
        }
        
        Debug.Log("=== REBUILDING LOCALIZATION FILES ===");
        
        // Обновляем ru.json
        UpdateRussianLocalization();
        
        // Обновляем en.json
        UpdateEnglishLocalization();
        
        // Обновляем tr.json
        UpdateTurkishLocalization();
        
        Debug.Log("Localization files rebuilt!");
        EditorUtility.DisplayDialog("Files Rebuilt", 
            "Successfully rebuilt all localization files!", "OK");
    }
    
    void UpdateRussianLocalization()
    {
        string ruPath = "Assets/StreamingAssets/Localization/ru.json";
        
        // Создаем базовую структуру если файл пустой
        LocalizationData ruData;
        
        if (File.Exists(ruPath))
        {
            string jsonContent = File.ReadAllText(ruPath);
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                // Создаем новую структуру если файл пустой
                ruData = new LocalizationData();
                ruData.LanguageName = "Русский";
                ruData.LanguageCode = "ru";
            }
            else
            {
                ruData = JsonUtility.FromJson<LocalizationData>(jsonContent);
                if (ruData == null)
                {
                    ruData = new LocalizationData();
                    ruData.LanguageName = "Русский";
                    ruData.LanguageCode = "ru";
                }
            }
        }
        else
        {
            ruData = new LocalizationData();
            ruData.LanguageName = "Русский";
            ruData.LanguageCode = "ru";
        }
        
        // Создаем новые PatientTexts
        var patientTexts = new
        {
            PatientNames = uniqueNames.OrderBy(x => x).ToArray(),
            Diagnoses = uniqueDiagnoses.OrderBy(x => x).ToArray(),
            Anamnesis = uniqueAnamnesis.OrderBy(x => x).ToArray()
        };
        
        // Обновляем только PatientTexts, сохраняя остальные данные
        ruData.PatientTexts = new PatientTexts
        {
            PatientNames = patientTexts.PatientNames,
            Diagnoses = patientTexts.Diagnoses,
            Anamnesis = patientTexts.Anamnesis
        };
        
        // Сохраняем с форматированием
        string newJson = JsonUtility.ToJson(ruData, true);
        File.WriteAllText(ruPath, newJson);
        
        Debug.Log($"Updated ru.json with {patientTexts.PatientNames.Length} names, {patientTexts.Diagnoses.Length} diagnoses, {patientTexts.Anamnesis.Length} anamnesis lines");
    }
    
    void UpdateEnglishLocalization()
    {
        string enPath = "Assets/StreamingAssets/Localization/en.json";
        
        // Создаем базовую структуру если файл пустой
        LocalizationData enData;
        
        if (File.Exists(enPath))
        {
            string jsonContent = File.ReadAllText(enPath);
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                // Создаем новую структуру если файл пустой
                enData = new LocalizationData();
                enData.LanguageName = "English";
                enData.LanguageCode = "en";
            }
            else
            {
                enData = JsonUtility.FromJson<LocalizationData>(jsonContent);
                if (enData == null)
                {
                    enData = new LocalizationData();
                    enData.LanguageName = "English";
                    enData.LanguageCode = "en";
                }
            }
        }
        else
        {
            enData = new LocalizationData();
            enData.LanguageName = "English";
            enData.LanguageCode = "en";
        }
        
        // Создаем английские переводы (заглушки для замены)
        var englishNames = new List<string>();
        var englishDiagnoses = new List<string>();
        var englishAnamnesis = new List<string>();
        
        // Имена
        foreach (string name in uniqueNames.OrderBy(x => x))
        {
            englishNames.Add(GetEnglishName(name));
        }
        
        // Диагнозы
        foreach (string diagnosis in uniqueDiagnoses.OrderBy(x => x))
        {
            englishDiagnoses.Add(GetEnglishDiagnosis(diagnosis));
        }
        
        // Анамнезы
        foreach (string anamnesis in uniqueAnamnesis.OrderBy(x => x))
        {
            englishAnamnesis.Add(GetEnglishAnamnesis(anamnesis));
        }
        
        enData.PatientTexts = new PatientTexts
        {
            PatientNames = englishNames.ToArray(),
            Diagnoses = englishDiagnoses.ToArray(),
            Anamnesis = englishAnamnesis.ToArray()
        };
        
        string newJson = JsonUtility.ToJson(enData, true);
        File.WriteAllText(enPath, newJson);
        
        Debug.Log($"Updated en.json with {englishNames.Count} names, {englishDiagnoses.Count} diagnoses, {englishAnamnesis.Count} anamnesis lines");
    }
    
    void UpdateTurkishLocalization()
    {
        string trPath = "Assets/StreamingAssets/Localization/tr.json";
        
        // Создаем базовую структуру если файл пустой
        LocalizationData trData;
        
        if (File.Exists(trPath))
        {
            string jsonContent = File.ReadAllText(trPath);
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                // Создаем новую структуру если файл пустой
                trData = new LocalizationData();
                trData.LanguageName = "Türkçe";
                trData.LanguageCode = "tr";
            }
            else
            {
                trData = JsonUtility.FromJson<LocalizationData>(jsonContent);
                if (trData == null)
                {
                    trData = new LocalizationData();
                    trData.LanguageName = "Türkçe";
                    trData.LanguageCode = "tr";
                }
            }
        }
        else
        {
            trData = new LocalizationData();
            trData.LanguageName = "Türkçe";
            trData.LanguageCode = "tr";
        }
        
        // Создаем турецкие переводы (заглушки для замены)
        var turkishNames = new List<string>();
        var turkishDiagnoses = new List<string>();
        var turkishAnamnesis = new List<string>();
        
        // Имена
        foreach (string name in uniqueNames.OrderBy(x => x))
        {
            turkishNames.Add(GetTurkishName(name));
        }
        
        // Диагнозы
        foreach (string diagnosis in uniqueDiagnoses.OrderBy(x => x))
        {
            turkishDiagnoses.Add(GetTurkishDiagnosis(diagnosis));
        }
        
        // Анамнезы
        foreach (string anamnesis in uniqueAnamnesis.OrderBy(x => x))
        {
            turkishAnamnesis.Add(GetTurkishAnamnesis(anamnesis));
        }
        
        trData.PatientTexts = new PatientTexts
        {
            PatientNames = turkishNames.ToArray(),
            Diagnoses = turkishDiagnoses.ToArray(),
            Anamnesis = turkishAnamnesis.ToArray()
        };
        
        string newJson = JsonUtility.ToJson(trData, true);
        File.WriteAllText(trPath, newJson);
        
        Debug.Log($"Updated tr.json with {turkishNames.Count} names, {turkishDiagnoses.Count} diagnoses, {turkishAnamnesis.Count} anamnesis lines");
    }
    
    void UpdatePatientKeys()
    {
        Debug.Log("=== UPDATING PATIENT KEYS ===");
        
        // Создаем сопоставления на основе новых данных
        var nameMappings = new Dictionary<string, string>();
        var diagnosisMappings = new Dictionary<string, string>();
        var anamnesisMappings = new Dictionary<string, string>();
        
        // Имена
        int nameIndex = 0;
        foreach (string name in uniqueNames.OrderBy(x => x))
        {
            nameMappings[name] = $"patient_name_{nameIndex++}";
        }
        
        // Диагнозы
        int diagnosisIndex = 0;
        foreach (string diagnosis in uniqueDiagnoses.OrderBy(x => x))
        {
            diagnosisMappings[diagnosis] = $"diagnosis_{diagnosisIndex++}";
        }
        
        // Анамнезы
        int anamnesisIndex = 0;
        foreach (string anamnesis in uniqueAnamnesis.OrderBy(x => x))
        {
            anamnesisMappings[anamnesis] = $"anamnesis_{anamnesisIndex++}";
        }
        
        // Обновляем профили пациентов
        string[] guids = AssetDatabase.FindAssets("t:PatientProfile");
        int updatedCount = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            PatientProfile profile = AssetDatabase.LoadAssetAtPath<PatientProfile>(path);
            
            if (profile != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                bool needsUpdate = false;
                
                // Обновляем ключ имени
                if (!string.IsNullOrEmpty(profile.patientName) && nameMappings.ContainsKey(profile.patientName))
                {
                    profile.patientNameKey = nameMappings[profile.patientName];
                    needsUpdate = true;
                }
                
                // Обновляем ключ диагноза
                if (!string.IsNullOrEmpty(profile.diagnosis) && diagnosisMappings.ContainsKey(profile.diagnosis))
                {
                    profile.diagnosisKey = diagnosisMappings[profile.diagnosis];
                    needsUpdate = true;
                }
                
                // Обновляем ключи анамнезов
                var anamnesisKeys = new List<string>();
                for (int i = 0; i < profile.anamnesisLines.Length; i++)
                {
                    string anamnesis = profile.anamnesisLines[i];
                    if (!string.IsNullOrEmpty(anamnesis) && anamnesisMappings.ContainsKey(anamnesis))
                    {
                        anamnesisKeys.Add(anamnesisMappings[anamnesis]);
                    }
                    else
                    {
                        anamnesisKeys.Add("");
                    }
                }
                
                profile.anamnesisKeys = anamnesisKeys.ToArray();
                needsUpdate = true;
                
                if (needsUpdate)
                {
                    EditorUtility.SetDirty(profile);
                    updatedCount++;
                    Debug.Log($"Updated keys for: {fileName}");
                }
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"Updated keys for {updatedCount} patients!");
        EditorUtility.DisplayDialog("Keys Updated", 
            $"Successfully updated localization keys for {updatedCount} patients!", "OK");
    }
    
    void CompleteRebuild()
    {
        ExtractPatientData();
        RebuildLocalizationFiles();
        UpdatePatientKeys();
        
        EditorUtility.DisplayDialog("Complete Rebuild", 
            "Successfully completed full localization rebuild!\n\n" +
            "All patient data has been extracted and localization files updated.\n" +
            "Patient keys have been reassigned based on new data.", "OK");
    }
    
    // Вспомогательные методы для перевода (заглушки)
    private string GetEnglishName(string russianName)
    {
        // Простые заглушки - вы можете заменить их на реальные переводы
        if (russianName.Contains("Анна")) return "Anna Semenova";
        if (russianName.Contains("Витя")) return "Vitya the Jumper";
        if (russianName.Contains("Лёва")) return "Leva the Cloud";
        if (russianName.Contains("Дядя")) return "Uncle Misha";
        if (russianName.Contains("Соня")) return "Sonya the Nose";
        if (russianName.Contains("Аркадий")) return "Arkady the Skeptic";
        if (russianName.Contains("Зинаида")) return "Zinaida the Cosmos";
        if (russianName.Contains("Профессор")) return "Professor Bookworm";
        if (russianName.Contains("Вера")) return "Vera the Realist";
        if (russianName.Contains("Борис")) return "Boris the Somersault";
        if (russianName.Contains("Главврач")) return "Chief Doctor Grustin";
        
        return "[EN] " + russianName;
    }
    
    private string GetEnglishDiagnosis(string russianDiagnosis)
    {
        if (russianDiagnosis.Contains("каламбур")) return "Pun Deficiency";
        if (russianDiagnosis.Contains("паден")) return "Suppressed Need for Falls";
        if (russianDiagnosis.Contains("чайник")) return "Talking Teapot Syndrome";
        if (russianDiagnosis.Contains("мурзил")) return "Acute \"Murzilka\" Deficiency";
        if (russianDiagnosis.Contains("рожиц")) return "Stagnant Funny Faces";
        if (russianDiagnosis.Contains("апати")) return "Cynical Apathy";
        if (russianDiagnosis.Contains("лун")) return "Verbal Contact with the Moon";
        if (russianDiagnosis.Contains("гипертр")) return "Pun Hypertrophy";
        if (russianDiagnosis.Contains("прагмат")) return "Pessimistic Pragmatism";
        if (russianDiagnosis.Contains("рефлекс")) return "Suppressed Laughter Reflex";
        if (russianDiagnosis.Contains("аура")) return "Absolute Aura of Seriousness";
        
        return "[EN] " + russianDiagnosis;
    }
    
    private string GetEnglishAnamnesis(string russianAnamnesis)
    {
        return "[EN] " + russianAnamnesis;
    }
    
    private string GetTurkishName(string russianName)
    {
        if (russianName.Contains("Анна")) return "Anna Semyonova";
        if (russianName.Contains("Витя")) return "Vitya-Zıplayıcı";
        if (russianName.Contains("Лёва")) return "Leva Bulut";
        if (russianName.Contains("Дядя")) return "Misha Amca";
        if (russianName.Contains("Соня")) return "Sonya-Burun";
        if (russianName.Contains("Аркадий")) return "Arkady Şüpheci";
        if (russianName.Contains("Зинаида")) return "Zinaida-Kozmos";
        if (russianName.Contains("Профессор")) return "Profesör Kitapkurdu";
        if (russianName.Contains("Вера")) return "Vera Gerçekçi";
        if (russianName.Contains("Борис")) return "Boris Takla";
        if (russianName.Contains("Главврач")) return "Başhekim Grustin";
        
        return "[TR] " + russianName;
    }
    
    private string GetTurkishDiagnosis(string russianDiagnosis)
    {
        if (russianDiagnosis.Contains("каламбур")) return "Kelime Eksikliği";
        if (russianDiagnosis.Contains("паден")) return "Düşme Baskısı Altında";
        if (russianDiagnosis.Contains("чайник")) return "Konuşan Çaydanlık Sendromu";
        if (russianDiagnosis.Contains("мурзил")) return "Akut \"Murzilka\" Eksikliği";
        if (russianDiagnosis.Contains("рожиц")) return "Durgun Esnek Yüzler";
        if (russianDiagnosis.Contains("апати")) return "Sinik Apati";
        if (russianDiagnosis.Contains("лун")) return "Ay ile Sözel İletişim";
        if (russianDiagnosis.Contains("гипертр")) return "Kelame Hipertrofisi";
        if (russianDiagnosis.Contains("прагмат")) return "Kötümser Pragmatizm";
        if (russianDiagnosis.Contains("рефлекс")) return "Düşerken Baskılanmış Kahkaha Refleksi";
        if (russianDiagnosis.Contains("аура")) return "Mutlak Ciddiyet Halesi";
        
        return "[TR] " + russianDiagnosis;
    }
    
    private string GetTurkishAnamnesis(string russianAnamnesis)
    {
        return "[TR] " + russianAnamnesis;
    }
    
    [System.Serializable]
    public class PatientData
    {
        public string fileName;
        public string patientName;
        public string diagnosis;
        public string[] anamnesisLines;
    }
    
    [System.Serializable]
    public class LocalizationData
    {
        public string LanguageName;
        public string LanguageCode;
        public GagTexts GagTexts;
        public PatientReactions PatientReactions;
        public UITexts UITexts;
        public DialogueTexts DialogueTexts;
        public PatientTexts PatientTexts;
    }
    
    [System.Serializable]
    public class GagTexts
    {
        public string[] Clownish;
        public string[] Verbal;
        public string[] Absurdist;
        public string[] Ironic;
        public string[] VerbalGag;
        public string[] IronicGag;
    }
    
    [System.Serializable]
    public class PatientReactions
    {
        public string[] Angry;
        public string[] Happy;
        public string[] Neutral;
        public string[] BossContinue;
        public string[] BossFail;
    }
    
    [System.Serializable]
    public class UITexts
    {
        public string RewardScreenTitle;
        public string SelectReward;
        public string LoadingFloor;
        public string ReturningToMenu;
        public string VictoryMessage;
        public string[] StaircaseNotCured;
        public string[] StaircaseNotVisited;
        public string WardDoorPrompt;
        public string WardExitDoorPrompt;
        public string WardExitDoorCuredPrompt;
        public string MedicalRecordPrompt;
        public string InfoDescPrompt;
        public string StaircaseAvailablePrompt;
        public string StaircaseBlockedPrompt;
    }
    
    [System.Serializable]
    public class DialogueTexts
    {
        public string DefaultGag;
        public string[] BatmanIntro;
        public string[] BatmanOutro;
        public string[] Hints;
        public string[] Tutorial;
    }
    
    [System.Serializable]
    public class PatientTexts
    {
        public string[] PatientNames;
        public string[] Diagnoses;
        public string[] Anamnesis;
    }
}
