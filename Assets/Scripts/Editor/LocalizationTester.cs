using UnityEngine;
using UnityEditor;

public class LocalizationTester : EditorWindow
{
    [MenuItem("Tools/Test Patient Localization")]
    public static void ShowWindow()
    {
        GetWindow<LocalizationTester>("Patient Localization Tester");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Patient Localization Tester", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("Test All Patients Localization"))
        {
            TestPatientLocalization();
        }
        
        if (GUILayout.Button("Test Current Language"))
        {
            TestCurrentLanguage();
        }
        
        if (GUILayout.Button("Test Language Switching"))
        {
            TestLanguageSwitching();
        }
        
        GUILayout.Space(10);
        GUILayout.Label("This will test if patient localization works correctly.");
    }
    
    void TestPatientLocalization()
    {
        Debug.Log("=== TESTING PATIENT LOCALIZATION ===");
        
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
                bool hasLocalization = false;
                
                Debug.Log($"=== Testing {fileName} ===");
                
                // Тестируем имя
                string localizedName = profile.GetLocalizedName();
                if (!string.IsNullOrEmpty(localizedName))
                {
                    if (localizedName != profile.patientName)
                    {
                        Debug.Log($"✓ Name localized: '{profile.patientName}' -> '{localizedName}'");
                        hasLocalization = true;
                    }
                    else
                    {
                        Debug.Log($"○ Name unchanged: '{profile.patientName}'");
                    }
                }
                else
                {
                    Debug.LogError($"✗ Name localization failed: '{profile.patientName}' -> NULL");
                    failCount++;
                    continue;
                }
                
                // Тестируем диагноз
                string localizedDiagnosis = profile.GetLocalizedDiagnosis();
                if (!string.IsNullOrEmpty(localizedDiagnosis))
                {
                    if (localizedDiagnosis != profile.diagnosis)
                    {
                        Debug.Log($"✓ Diagnosis localized: '{profile.diagnosis}' -> '{localizedDiagnosis}'");
                        hasLocalization = true;
                    }
                    else
                    {
                        Debug.Log($"○ Diagnosis unchanged: '{profile.diagnosis}'");
                    }
                }
                else
                {
                    Debug.LogError($"✗ Diagnosis localization failed: '{profile.diagnosis}' -> NULL");
                    failCount++;
                    continue;
                }
                
                // Тестируем анамнез
                string[] localizedAnamnesis = profile.GetLocalizedAnamnesis();
                if (localizedAnamnesis != null && localizedAnamnesis.Length > 0)
                {
                    bool anamnesisLocalized = false;
                    for (int i = 0; i < localizedAnamnesis.Length; i++)
                    {
                        if (i < profile.anamnesisLines.Length)
                        {
                            string original = profile.anamnesisLines[i];
                            string localized = localizedAnamnesis[i];
                            
                            if (!string.IsNullOrEmpty(localized) && localized != original)
                            {
                                Debug.Log($"✓ Anamnesis {i+1} localized: '{original}' -> '{localized}'");
                                anamnesisLocalized = true;
                            }
                            else if (string.IsNullOrEmpty(localized))
                            {
                                Debug.LogWarning($"○ Anamnesis {i+1} unchanged: '{original}'");
                            }
                            else
                            {
                                Debug.LogError($"✗ Anamnesis {i+1} failed: '{original}' -> NULL");
                            }
                        }
                    }
                    
                    if (anamnesisLocalized)
                    {
                        hasLocalization = true;
                    }
                }
                else
                {
                    Debug.LogError($"✗ Anamnesis localization failed: NULL or empty");
                    failCount++;
                    continue;
                }
                
                if (hasLocalization)
                {
                    successCount++;
                    Debug.Log($"✓ {fileName} has working localization");
                }
                else
                {
                    Debug.LogWarning($"○ {fileName} has no localization (using original data)");
                }
                
                Debug.Log("");
            }
        }
        
        Debug.Log($"=== LOCALIZATION TEST COMPLETE ===");
        Debug.Log($"Success: {successCount}, Failed: {failCount}, Total: {guids.Length}");
        
        EditorUtility.DisplayDialog("Localization Test Complete", 
            $"Test Results:\n✓ Success: {successCount}\n✗ Failed: {failCount}\n○ No Localization: {guids.Length - successCount - failCount}", "OK");
    }
    
    void TestCurrentLanguage()
    {
        Debug.Log($"=== CURRENT LANGUAGE: {LocalizationManager.CurrentLanguageCode} ===");
        Debug.Log($"Language Name: {LocalizationManager.CurrentLanguage?.LanguageName}");
        
        if (LocalizationManager.CurrentLanguage?.PatientTexts != null)
        {
            Debug.Log($"Patient Names Available: {LocalizationManager.CurrentLanguage.PatientTexts.PatientNames.Length}");
            Debug.Log($"Diagnoses Available: {LocalizationManager.CurrentLanguage.PatientTexts.Diagnoses.Length}");
            Debug.Log($"Anamnesis Available: {LocalizationManager.CurrentLanguage.PatientTexts.Anamnesis.Length}");
        }
        else
        {
            Debug.LogError("PatientTexts not loaded!");
        }
    }
    
    void TestLanguageSwitching()
    {
        Debug.Log("=== TESTING LANGUAGE SWITCHING ===");
        
        string[] languages = { "ru", "en", "tr" };
        
        foreach (string lang in languages)
        {
            Debug.Log($"Testing switch to {lang}...");
            LocalizationManager.SetLanguage(lang);
            TestCurrentLanguage();
            Debug.Log("");
        }
        
        // Возвращаем русский
        LocalizationManager.SetLanguage("ru");
        Debug.Log("=== LANGUAGE SWITCHING TEST COMPLETE ===");
        
        EditorUtility.DisplayDialog("Language Switching Test Complete", 
            "Tested switching between Russian, English, and Turkish.\nCheck Unity Console for details.", "OK");
    }
}
