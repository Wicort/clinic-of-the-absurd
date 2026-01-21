using UnityEngine;
using UnityEditor;

public class DiagnosisFixer : EditorWindow
{
    [MenuItem("Tools/Fix Patient Diagnosis")]
    public static void ShowWindow()
    {
        GetWindow<DiagnosisFixer>("Fix Patient Diagnosis");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Fix Patient Diagnosis", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("Debug Patient Diagnosis Keys"))
        {
            DebugPatientDiagnosisKeys();
        }
        
        if (GUILayout.Button("Fix All Diagnosis Keys"))
        {
            FixAllDiagnosisKeys();
        }
        
        if (GUILayout.Button("Test Diagnosis Localization"))
        {
            TestDiagnosisLocalization();
        }
        
        GUILayout.Space(10);
        GUILayout.Label("This will fix missing diagnosis localization.");
    }
    
    void DebugPatientDiagnosisKeys()
    {
        Debug.Log("=== DEBUGGING PATIENT DIAGNOSIS KEYS ===");
        
        string[] guids = AssetDatabase.FindAssets("t:PatientProfile");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            PatientProfile profile = AssetDatabase.LoadAssetAtPath<PatientProfile>(path);
            
            if (profile != null)
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                Debug.Log($"=== {fileName} ===");
                Debug.Log($"Name: '{profile.GetLocalizedName()}'");
                Debug.Log($"Original Diagnosis: '{profile.diagnosis}'");
                Debug.Log($"Diagnosis Key: '{profile.diagnosisKey}'");
                Debug.Log($"Localized Diagnosis: '{profile.GetLocalizedDiagnosis()}'");
                Debug.Log("");
            }
        }
    }
    
    void FixAllDiagnosisKeys()
    {
        Debug.Log("=== FIXING ALL DIAGNOSIS KEYS ===");
        
        // Создаем точные сопоставления для всех диагнозов
        var diagnosisMappings = new System.Collections.Generic.Dictionary<string, string>();
        
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
        
        string[] guids = AssetDatabase.FindAssets("t:PatientProfile");
        int fixedCount = 0;
        int notFoundCount = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            PatientProfile profile = AssetDatabase.LoadAssetAtPath<PatientProfile>(path);
            
            if (profile != null)
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                
                if (!string.IsNullOrEmpty(profile.diagnosis))
                {
                    if (diagnosisMappings.ContainsKey(profile.diagnosis))
                    {
                        string correctKey = diagnosisMappings[profile.diagnosis];
                        if (profile.diagnosisKey != correctKey)
                        {
                            profile.diagnosisKey = correctKey;
                            EditorUtility.SetDirty(profile);
                            fixedCount++;
                            Debug.Log($"✓ Fixed diagnosis for {fileName}: '{profile.diagnosis}' -> '{correctKey}'");
                        }
                        else
                        {
                            Debug.Log($"○ Diagnosis already correct for {fileName}: '{correctKey}'");
                        }
                    }
                    else
                    {
                        notFoundCount++;
                        Debug.LogError($"✗ No mapping found for diagnosis: '{profile.diagnosis}' in {fileName}");
                    }
                }
                else
                {
                    Debug.LogWarning($"○ Empty diagnosis in {fileName}");
                }
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"Fixed diagnosis keys for {fixedCount} patients!");
        Debug.Log($"No mapping found for {notFoundCount} diagnoses!");
        
        EditorUtility.DisplayDialog("Diagnosis Fix Complete", 
            $"Fixed: {fixedCount}\nNot found: {notFoundCount}\n\nCheck Unity Console for details.", "OK");
    }
    
    void TestDiagnosisLocalization()
    {
        Debug.Log("=== TESTING DIAGNOSIS LOCALIZATION ===");
        
        string[] guids = AssetDatabase.FindAssets("t:PatientProfile");
        int successCount = 0;
        int failCount = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            PatientProfile profile = AssetDatabase.LoadAssetAtPath<PatientProfile>(path);
            
            if (profile != null)
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
                string localizedDiagnosis = profile.GetLocalizedDiagnosis();
                
                Debug.Log($"=== {fileName} ===");
                Debug.Log($"Original: '{profile.diagnosis}'");
                Debug.Log($"Key: '{profile.diagnosisKey}'");
                Debug.Log($"Localized: '{localizedDiagnosis}'");
                
                if (!string.IsNullOrEmpty(localizedDiagnosis) && localizedDiagnosis != profile.diagnosis)
                {
                    Debug.Log($"✓ SUCCESS: Diagnosis is localized");
                    successCount++;
                }
                else if (string.IsNullOrEmpty(profile.diagnosisKey))
                {
                    Debug.LogWarning($"○ NO KEY: No diagnosis key assigned");
                }
                else if (localizedDiagnosis == profile.diagnosis)
                {
                    Debug.LogWarning($"○ NOT LOCALIZED: Same as original");
                }
                else
                {
                    Debug.LogError($"✗ FAILED: Localized diagnosis is empty or null");
                    failCount++;
                }
                
                Debug.Log("");
            }
        }
        
        Debug.Log($"=== DIAGNOSIS LOCALIZATION TEST RESULTS ===");
        Debug.Log($"Success: {successCount}");
        Debug.Log($"Failed: {failCount}");
        Debug.Log($"No key: {guids.Length - successCount - failCount}");
        
        EditorUtility.DisplayDialog("Diagnosis Localization Test", 
            $"Results:\n✓ Success: {successCount}\n✗ Failed: {failCount}\n○ No key: {guids.Length - successCount - failCount}", "OK");
    }
}
