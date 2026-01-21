using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class SimplePatientMigrator : EditorWindow
{
    [MenuItem("Tools/Simple Patient Migration")]
    public static void ShowWindow()
    {
        GetWindow<SimplePatientMigrator>("Simple Patient Migration");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Simple Patient Migration", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        GUILayout.Label("This will extract REAL data from asset files and create proper mappings.");
        
        if (GUILayout.Button("Extract and Map Patient Data"))
        {
            ExtractAndMapPatients();
        }
        
        if (GUILayout.Button("Apply Mappings to Patients"))
        {
            ApplyMappingsToPatients();
        }
        
        GUILayout.Space(10);
        GUILayout.Label("Step 1: Extract data from assets");
        GUILayout.Label("Step 2: Apply mappings back to assets");
    }
    
    void ExtractAndMapPatients()
    {
        Debug.Log("=== EXTRACTING REAL PATIENT DATA ===");
        
        string[] guids = AssetDatabase.FindAssets("t:PatientProfile");
        var mappings = new Dictionary<string, PatientData>();
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            PatientProfile profile = AssetDatabase.LoadAssetAtPath<PatientProfile>(path);
            
            if (profile != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                var data = new PatientData();
                
                data.fileName = fileName;
                data.patientName = profile.patientName;
                data.diagnosis = profile.diagnosis;
                data.anamnesisLines = new List<string>(profile.anamnesisLines);
                
                mappings[fileName] = data;
                
                Debug.Log($"=== {fileName} ===");
                Debug.Log($"Name: '{data.patientName}'");
                Debug.Log($"Diagnosis: '{data.diagnosis}'");
                Debug.Log($"Anamnesis lines: {data.anamnesisLines.Count}");
                
                for (int i = 0; i < data.anamnesisLines.Count; i++)
                {
                    Debug.Log($"  {i+1}: '{data.anamnesisLines[i]}'");
                }
                Debug.Log("");
            }
        }
        
        // Создаем сопоставления на основе реальных данных
        CreateMappingsFromRealData(mappings);
        
        Debug.Log($"Extracted data from {mappings.Count} patients");
        EditorUtility.DisplayDialog("Data Extracted", 
            $"Extracted real data from {mappings.Count} patient profiles.\n\nCheck Unity Console for exact strings.", "OK");
    }
    
    void CreateMappingsFromRealData(Dictionary<string, PatientData> patients)
    {
        Debug.Log("=== CREATING MAPPINGS FROM REAL DATA ===");
        
        // Создаем словари с реальными данными
        var nameMappings = new Dictionary<string, string>();
        var diagnosisMappings = new Dictionary<string, string>();
        var anamnesisMappings = new Dictionary<string, string>();
        
        // Имена (на основе реальных данных)
        nameMappings["Анна Семёновна"] = "patient_anna_semenova";
        nameMappings["Витя-Прыгун"] = "patient_vitya_prygun";
        nameMappings["Лёва Облачко"] = "patient_lev_oblachko";
        nameMappings["Дядя Миша"] = "patient_uncle_misha";
        nameMappings["Соня-Нос"] = "patient_sonya_nos";
        nameMappings["Аркадий Скептик"] = "patient_arkady_skeptic";
        nameMappings["Зинаида-Космос"] = "patient_zinaida_cosmos";
        nameMappings["Профессор Буквоед"] = "patient_professor_bukvoed";
        nameMappings["Вера Реалист"] = "patient_vera_realist";
        nameMappings["Борис-Кувырок"] = "patient_boris_kuvyrok";
        nameMappings["Главврач Грустин"] = "patient_chief_doctor_grustin";
        
        // Диагнозы (на основе реальных данных)
        diagnosisMappings["Недостаток каламбуров"] = "diagnosis_pun_deficiency";
        diagnosisMappings["Подавленная потребность в падениях"] = "diagnosis_fall_suppression";
        diagnosisMappings["Синдром говорящего чайника"] = "diagnosis_talking_teapot";
        diagnosisMappings["Острая нехватка \"мурзилок\""] = "diagnosis_murzilka_deficiency";
        diagnosisMappings["Застойный весёлый рожиц"] = "diagnosis_stagnant_faces";
        diagnosisMappings["Циничная апатия"] = "diagnosis_cynical_apathy";
        diagnosisMappings["Вербальный контакт с луной"] = "diagnosis_moon_contact";
        diagnosisMappings["Гипертрофия каламбуров"] = "diagnosis_pun_hypertrophy";
        diagnosisMappings["Пессимистический прагматизм"] = "diagnosis_pessimistic_pragmatism";
        diagnosisMappings["Подавленный рефлекс смеха при падении"] = "diagnosis_suppressed_laughter_reflex";
        diagnosisMappings["Абсолютная аура серьёзности (иммунитет к одиночным гэгам)"] = "diagnosis_absolute_aura";
        
        // Анамнезы (создаем на основе реальных данных)
        int anamnesisCounter = 0;
        foreach (var patient in patients.Values)
        {
            foreach (string anamnesis in patient.anamnesisLines)
            {
                if (!string.IsNullOrEmpty(anamnesis) && !anamnesisMappings.ContainsKey(anamnesis))
                {
                    string key = $"anamnesis_{anamnesisCounter++}";
                    anamnesisMappings[anamnesis] = key;
                    Debug.Log($"Created mapping: '{anamnesis}' -> {key}");
                }
            }
        }
        
        // Сохраняем сопоставления для следующего шага
        EditorPrefs.SetString("PatientNameMappings", JsonUtility.ToJson(new SerializableDictionary(nameMappings)));
        EditorPrefs.SetString("PatientDiagnosisMappings", JsonUtility.ToJson(new SerializableDictionary(diagnosisMappings)));
        EditorPrefs.SetString("PatientAnamnesisMappings", JsonUtility.ToJson(new SerializableDictionary(anamnesisMappings)));
        
        Debug.Log($"Created {nameMappings.Count} name mappings, {diagnosisMappings.Count} diagnosis mappings, {anamnesisMappings.Count} anamnesis mappings");
    }
    
    void ApplyMappingsToPatients()
    {
        Debug.Log("=== APPLYING MAPPINGS TO PATIENTS ===");
        
        // Загружаем сохраненные сопоставления
        var nameMappings = JsonUtility.FromJson<SerializableDictionary>(EditorPrefs.GetString("PatientNameMappings", "{}")).ToDictionary();
        var diagnosisMappings = JsonUtility.FromJson<SerializableDictionary>(EditorPrefs.GetString("PatientDiagnosisMappings", "{}")).ToDictionary();
        var anamnesisMappings = JsonUtility.FromJson<SerializableDictionary>(EditorPrefs.GetString("PatientAnamnesisMappings", "{}")).ToDictionary();
        
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
                
                Debug.Log($"=== {fileName} ===");
                
                // Применяем ключ для имени
                if (nameMappings.ContainsKey(profile.patientName))
                {
                    profile.patientNameKey = nameMappings[profile.patientName];
                    needsUpdate = true;
                    Debug.Log($"✓ Name key: {profile.patientNameKey}");
                }
                else
                {
                    Debug.LogWarning($"✗ No name key for: '{profile.patientName}'");
                }
                
                // Применяем ключ для диагноза
                if (diagnosisMappings.ContainsKey(profile.diagnosis))
                {
                    profile.diagnosisKey = diagnosisMappings[profile.diagnosis];
                    needsUpdate = true;
                    Debug.Log($"✓ Diagnosis key: {profile.diagnosisKey}");
                }
                else
                {
                    Debug.LogWarning($"✗ No diagnosis key for: '{profile.diagnosis}'");
                }
                
                // Применяем ключи для анамнезов
                var anamnesisKeys = new List<string>();
                for (int i = 0; i < profile.anamnesisLines.Length; i++)
                {
                    string anamnesis = profile.anamnesisLines[i];
                    if (!string.IsNullOrEmpty(anamnesis) && anamnesisMappings.ContainsKey(anamnesis))
                    {
                        anamnesisKeys.Add(anamnesisMappings[anamnesis]);
                        Debug.Log($"✓ Anamnesis key {i}: {anamnesisMappings[anamnesis]}");
                    }
                    else
                    {
                        anamnesisKeys.Add("");
                        if (!string.IsNullOrEmpty(anamnesis))
                            Debug.LogWarning($"✗ No anamnesis key for: '{anamnesis}'");
                    }
                }
                
                profile.anamnesisKeys = anamnesisKeys.ToArray();
                needsUpdate = true;
                
                if (needsUpdate)
                {
                    EditorUtility.SetDirty(profile);
                    updatedCount++;
                    Debug.Log($"✓ Updated profile: {fileName}");
                }
                
                Debug.Log("");
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"Updated {updatedCount} patient profiles!");
        EditorUtility.DisplayDialog("Migration Complete", 
            $"Successfully updated {updatedCount} patient profiles with localization keys!", "OK");
    }
    
    [System.Serializable]
    public class PatientData
    {
        public string fileName;
        public string patientName;
        public string diagnosis;
        public List<string> anamnesisLines;
    }
    
    [System.Serializable]
    public class SerializableDictionary
    {
        public List<string> keys;
        public List<string> values;
        
        public SerializableDictionary() { keys = new List<string>(); values = new List<string>(); }
        public SerializableDictionary(Dictionary<string, string> dict) 
        { 
            keys = new List<string>(dict.Keys);
            values = new List<string>(dict.Values);
        }
        
        public Dictionary<string, string> ToDictionary()
        {
            var result = new Dictionary<string, string>();
            for (int i = 0; i < keys.Count && i < values.Count; i++)
            {
                result[keys[i]] = values[i];
            }
            return result;
        }
    }
}
