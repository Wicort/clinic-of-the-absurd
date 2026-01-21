using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class RobustPatientMigrator : EditorWindow
{
    private Dictionary<string, string> nameToKey = new Dictionary<string, string>();
    private Dictionary<string, string> diagnosisToKey = new Dictionary<string, string>();
    private Dictionary<string, string> anamnesisToKey = new Dictionary<string, string>();
    
    [MenuItem("Tools/Robust Patient Migration")]
    public static void ShowWindow()
    {
        GetWindow<RobustPatientMigrator>("Robust Patient Migration");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Robust Patient Migration (Fixed)", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("Initialize Key Mappings"))
        {
            InitializeKeyMappings();
        }
        
        if (GUILayout.Button("Update All Patient Profiles"))
        {
            UpdateAllProfiles();
        }
        
        if (GUILayout.Button("Debug Patient Data"))
        {
            DebugPatientData();
        }
        
        GUILayout.Space(10);
        GUILayout.Label("This version handles Unicode and whitespace issues better.");
    }
    
    void InitializeKeyMappings()
    {
        // Имена пациентов
        nameToKey["Анна Семёновна"] = "patient_anna_semenova";
        nameToKey["Витя-Прыгун"] = "patient_vitya_prygun";
        nameToKey["Лёва Облачко"] = "patient_lev_oblachko";
        nameToKey["Дядя Миша"] = "patient_uncle_misha";
        nameToKey["Соня-Нос"] = "patient_sonya_nos";
        nameToKey["Аркадий Скептик"] = "patient_arkady_skeptic";
        nameToKey["Зинаида-Космос"] = "patient_zinaida_cosmos";
        nameToKey["Профессор Буквоед"] = "patient_professor_bukvoed";
        nameToKey["Вера Реалист"] = "patient_vera_realist";
        nameToKey["Борис-Кувырок"] = "patient_boris_kuvyrok";
        nameToKey["Главврач Грустин"] = "patient_chief_doctor_grustin";
        
        // Диагнозы
        diagnosisToKey["Недостаток каламбуров"] = "diagnosis_pun_deficiency";
        diagnosisToKey["Подавленная потребность в падениях"] = "diagnosis_fall_suppression";
        diagnosisToKey["Синдром говорящего чайника"] = "diagnosis_talking_teapot";
        diagnosisToKey["Острая нехватка \"мурзилок\""] = "diagnosis_murzilka_deficiency";
        diagnosisToKey["Застойный весёлый рожиц"] = "diagnosis_stagnant_faces";
        diagnosisToKey["Циничная апатия"] = "diagnosis_cynical_apathy";
        diagnosisToKey["Вербальный контакт с луной"] = "diagnosis_moon_contact";
        diagnosisToKey["Гипертрофия каламбуров"] = "diagnosis_pun_hypertrophy";
        diagnosisToKey["Пессимистический прагматизм"] = "diagnosis_pessimistic_pragmatism";
        diagnosisToKey["Подавленный рефлекс смеха при падении"] = "diagnosis_suppressed_laughter_reflex";
        diagnosisToKey["Абсолютная аура серьёзности (иммунитет к одиночным гэгам)"] = "diagnosis_absolute_aura";
        
        // Анамнезы (только самые важные для теста)
        anamnesisToKey["Постоянно спрашивает: \"А вы слыхали анекдот про тёщу и холодильник?\""] = "anamnesis_anna_jokes";
        anamnesisToKey["Вчера смеялась над надписью \"Туалет\" на двери столовой."] = "anamnesis_anna_toilet_sign";
        anamnesisToKey["Пыталась придумать, как сказать \"хлеб\" на языке инопланетян."] = "anamnesis_anna_alien_bread";
        anamnesisToKey["Улыбнулся, когда медсестра уронила поднос с тарелками."] = "anamnesis_vitya_tray_fall";
        anamnesisToKey["Пытался повторить падение с табурета, но упал неудачно."] = "anamnesis_vitya_stool_fall";
        anamnesisToKey["Смеётся, когда кто-то чихает громко."] = "anamnesis_vitya_sneeze";
        anamnesisToKey["Утверждает, что его тапок - шпион из будущего."] = "anamnesis_lev_spy_slipper";
        anamnesisToKey["Пытался накормить цветок супом из телевизора."] = "anamnesis_lev_tv_soup";
        anamnesisToKey["Говорит, что облака — это вата, которую забыли убирать боги."] = "anamnesis_lev_cloud_cotton";
        
        Debug.Log("Robust key mappings initialized!");
        EditorUtility.DisplayDialog("Initialization Complete", 
            $"Initialized {nameToKey.Count} names, {diagnosisToKey.Count} diagnoses, and {anamnesisToKey.Count} anamnesis keys!", "OK");
    }
    
    void UpdateAllProfiles()
    {
        string[] guids = AssetDatabase.FindAssets("t:PatientProfile");
        int updatedCount = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            PatientProfile profile = AssetDatabase.LoadAssetAtPath<PatientProfile>(path);
            
            if (profile != null)
            {
                bool needsUpdate = false;
                string fileName = Path.GetFileNameWithoutExtension(path);
                
                Debug.Log($"=== Processing {fileName} ===");
                Debug.Log($"Name: '{profile.patientName}'");
                Debug.Log($"Diagnosis: '{profile.diagnosis}'");
                
                // Добавляем ключ для имени с более надежным поиском
                string foundNameKey = FindBestMatch(profile.patientName, nameToKey);
                if (!string.IsNullOrEmpty(foundNameKey))
                {
                    profile.patientNameKey = foundNameKey;
                    needsUpdate = true;
                    Debug.Log($"✓ Found name key: {foundNameKey}");
                }
                else
                {
                    Debug.LogWarning($"✗ No key found for name: '{profile.patientName}'");
                }
                
                // Добавляем ключ для диагноза
                string foundDiagnosisKey = FindBestMatch(profile.diagnosis, diagnosisToKey);
                if (!string.IsNullOrEmpty(foundDiagnosisKey))
                {
                    profile.diagnosisKey = foundDiagnosisKey;
                    needsUpdate = true;
                    Debug.Log($"✓ Found diagnosis key: {foundDiagnosisKey}");
                }
                else
                {
                    Debug.LogWarning($"✗ No key found for diagnosis: '{profile.diagnosis}'");
                }
                
                // Добавляем ключи для анамнезов
                var anamnesisKeys = new List<string>();
                for (int i = 0; i < profile.anamnesisLines.Length; i++)
                {
                    string anamnesis = profile.anamnesisLines[i];
                    if (!string.IsNullOrEmpty(anamnesis))
                    {
                        string foundAnamnesisKey = FindBestMatch(anamnesis, anamnesisToKey);
                        if (!string.IsNullOrEmpty(foundAnamnesisKey))
                        {
                            anamnesisKeys.Add(foundAnamnesisKey);
                            Debug.Log($"✓ Found anamnesis key {i}: {foundAnamnesisKey}");
                        }
                        else
                        {
                            anamnesisKeys.Add("");
                            Debug.LogWarning($"✗ No key found for anamnesis {i}: '{anamnesis}'");
                        }
                    }
                    else
                    {
                        anamnesisKeys.Add("");
                    }
                }
                
                if (anamnesisKeys.Count > 0)
                {
                    profile.anamnesisKeys = anamnesisKeys.ToArray();
                    needsUpdate = true;
                }
                
                if (needsUpdate)
                {
                    EditorUtility.SetDirty(profile);
                    updatedCount++;
                    Debug.Log($"✓ Updated profile: {fileName}");
                }
                
                Debug.Log(""); // Пустая строка для разделения
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"Updated {updatedCount} patient profiles!");
        EditorUtility.DisplayDialog("Migration Complete", 
            $"Successfully updated {updatedCount} patient profiles!\n\nCheck Unity Console for details.", "OK");
    }
    
    string FindBestMatch(string input, Dictionary<string, string> keyMap)
    {
        if (string.IsNullOrEmpty(input))
            return null;
            
        // Прямое совпадение
        if (keyMap.ContainsKey(input))
            return keyMap[input];
            
        // Убираем лишние пробелы
        string trimmed = input.Trim();
        if (keyMap.ContainsKey(trimmed))
            return keyMap[trimmed];
            
        // Нормализуем Unicode
        string normalized = NormalizeString(input);
        foreach (var kvp in keyMap)
        {
            if (NormalizeString(kvp.Key) == normalized)
                return kvp.Value;
        }
            
        return null;
    }
    
    string NormalizeString(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
            
        // Нормализация Unicode и пробелов
        string normalized = input.Trim();
        normalized = Regex.Replace(normalized, @"\s+", " "); // Множественные пробелы
        normalized = normalized.Replace("\u00A0", " "); // Non-breaking space
        normalized = normalized.Replace("\u200B", ""); // Zero-width space
        
        return normalized;
    }
    
    void DebugPatientData()
    {
        string[] guids = AssetDatabase.FindAssets("t:PatientProfile");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            PatientProfile profile = AssetDatabase.LoadAssetAtPath<PatientProfile>(path);
            
            if (profile != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                Debug.Log($"=== {fileName} ===");
                Debug.Log($"Name: '{profile.patientName}'");
                Debug.Log($"Diagnosis: '{profile.diagnosis}'");
                Debug.Log($"Anamnesis count: {profile.anamnesisLines.Length}");
                
                for (int i = 0; i < profile.anamnesisLines.Length; i++)
                {
                    Debug.Log($"  {i + 1}. '{profile.anamnesisLines[i]}'");
                }
                
                Debug.Log($"Current name key: '{profile.patientNameKey}'");
                Debug.Log($"Current diagnosis key: '{profile.diagnosisKey}'");
                Debug.Log("");
            }
        }
    }
}
