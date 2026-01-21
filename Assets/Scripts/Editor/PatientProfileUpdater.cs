using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PatientProfileUpdater : EditorWindow
{
    private Dictionary<string, string> nameToKey = new Dictionary<string, string>();
    private Dictionary<string, string> diagnosisToKey = new Dictionary<string, string>();
    private Dictionary<string, string> anamnesisToKey = new Dictionary<string, string>();
    
    [MenuItem("Tools/Update Patient Profiles with Localization Keys")]
    public static void ShowWindow()
    {
        GetWindow<PatientProfileUpdater>("Patient Profile Updater");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Update Patient Profiles with Localization Keys", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("Initialize Key Mappings"))
        {
            InitializeKeyMappings();
        }
        
        if (GUILayout.Button("Update All Patient Profiles"))
        {
            UpdateAllProfiles();
        }
        
        GUILayout.Space(10);
        GUILayout.Label("This will add localization keys to existing patient profiles");
        GUILayout.Label("without losing current data (backward compatibility).");
    }
    
    void InitializeKeyMappings()
    {
        // Имена пациентов
        nameToKey.Add("Анна Семёновна", "patient_anna_semenova");
        nameToKey.Add("Витя-Прыгун", "patient_vitya_prygun");
        nameToKey.Add("Лёва Облачко", "patient_lev_oblachko");
        
        // Диагнозы
        diagnosisToKey.Add("Недостаток каламбуров", "diagnosis_pun_deficiency");
        diagnosisToKey.Add("Подавленная потребность в падениях", "diagnosis_fall_suppression");
        diagnosisToKey.Add("Синдром говорящего чайника", "diagnosis_talking_teapot");
        
        // Анамнезы
        anamnesisToKey.Add("Постоянно спрашивает: \"А вы слыхали анекдот про тёщу и холодильник?\"", "anamnesis_anna_jokes");
        anamnesisToKey.Add("Вчера смеялась над надписью \"Туалет\" на двери столовой.", "anamnesis_anna_toilet_sign");
        anamnesisToKey.Add("Пыталась придумать, как сказать \"хлеб\" на языке инопланетян.", "anamnesis_anna_alien_bread");
        
        anamnesisToKey.Add("Улыбнулся, когда медсестра уронила поднос с тарелками.", "anamnesis_vitya_tray_fall");
        anamnesisToKey.Add("Пытался повторить падение с табурета, но упал неудачно.", "anamnesis_vitya_stool_fall");
        anamnesisToKey.Add("Смеётся, когда кто-то чихает громко.", "anamnesis_vitya_sneeze");
        
        anamnesisToKey.Add("Утверждает, что его тапок - шпион из будущего.", "anamnesis_lev_spy_slipper");
        anamnesisToKey.Add("Пытался накормить цветок супом из телевизора.", "anamnesis_lev_tv_soup");
        anamnesisToKey.Add("Говорит, что облака — это вата, которую забыли убирать боги.", "anamnesis_lev_cloud_cotton");
        
        Debug.Log("Key mappings initialized!");
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
                
                // Добавляем ключ для имени
                if (nameToKey.ContainsKey(profile.patientName))
                {
                    profile.patientNameKey = nameToKey[profile.patientName];
                    needsUpdate = true;
                }
                
                // Добавляем ключ для диагноза
                if (diagnosisToKey.ContainsKey(profile.diagnosis))
                {
                    profile.diagnosisKey = diagnosisToKey[profile.diagnosis];
                    needsUpdate = true;
                }
                
                // Добавляем ключи для анамнезов
                var anamnesisKeys = new List<string>();
                foreach (string anamnesis in profile.anamnesisLines)
                {
                    if (anamnesisToKey.ContainsKey(anamnesis))
                    {
                        anamnesisKeys.Add(anamnesisToKey[anamnesis]);
                    }
                    else
                    {
                        anamnesisKeys.Add(""); // Пустой ключ для неизвестных анамнезов
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
                    Debug.Log($"Updated profile: {path}");
                }
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"Updated {updatedCount} patient profiles!");
        EditorUtility.DisplayDialog("Update Complete", 
            $"Successfully updated {updatedCount} patient profiles with localization keys!", "OK");
    }
}
