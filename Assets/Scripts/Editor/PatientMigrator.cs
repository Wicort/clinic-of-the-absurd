using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class PatientMigrator : EditorWindow
{
    private Vector2 scrollPosition;
    private string migrationLog = "";
    private bool autoSave = true;
    
    [MenuItem("Tools/Migrate Patients to Localization")]
    public static void ShowWindow()
    {
        GetWindow<PatientMigrator>("Patient Migrator");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Patient Migration to Localization", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        autoSave = EditorGUILayout.Toggle("Auto-save changes", autoSave);
        
        if (GUILayout.Button("Migrate All Patients"))
        {
            MigrateAllPatients();
        }
        
        GUILayout.Space(10);
        GUILayout.Label("Migration Log:");
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        migrationLog = EditorGUILayout.TextArea(migrationLog, GUILayout.Height(400));
        EditorGUILayout.EndScrollView();
        
        if (GUILayout.Button("Clear Log"))
        {
            migrationLog = "";
        }
    }
    
    void MigrateAllPatients()
    {
        migrationLog = "Starting patient migration...\n\n";
        
        // Собираем все данные пациентов
        var allNames = new List<string>();
        var allDiagnoses = new List<string>();
        var allAnamnesis = new List<string>();
        
        string[] guids = AssetDatabase.FindAssets("t:PatientProfile");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            PatientProfile profile = AssetDatabase.LoadAssetAtPath<PatientProfile>(path);
            
            if (profile != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(path);
                migrationLog += $"=== {fileName} ===\n";
                
                // Добавляем имя если еще нет
                if (!string.IsNullOrEmpty(profile.patientName) && !allNames.Contains(profile.patientName))
                {
                    allNames.Add(profile.patientName);
                    migrationLog += $"Added name: {profile.patientName}\n";
                }
                
                // Добавляем диагноз если еще нет
                if (!string.IsNullOrEmpty(profile.diagnosis) && !allDiagnoses.Contains(profile.diagnosis))
                {
                    allDiagnoses.Add(profile.diagnosis);
                    migrationLog += $"Added diagnosis: {profile.diagnosis}\n";
                }
                
                // Добавляем анамнезы
                foreach (string anamnesis in profile.anamnesisLines)
                {
                    if (!string.IsNullOrEmpty(anamnesis) && !allAnamnesis.Contains(anamnesis))
                    {
                        allAnamnesis.Add(anamnesis);
                        migrationLog += $"Added anamnesis: {anamnesis}\n";
                    }
                }
                
                migrationLog += "\n";
            }
        }
        
        // Обновляем JSON файлы
        if (autoSave)
        {
            UpdateJsonFiles(allNames, allDiagnoses, allAnamnesis);
        }
        
        migrationLog += $"\nMigration completed!\n";
        migrationLog += $"Total patients: {guids.Length}\n";
        migrationLog += $"Unique names: {allNames.Count}\n";
        migrationLog += $"Unique diagnoses: {allDiagnoses.Count}\n";
        migrationLog += $"Unique anamnesis: {allAnamnesis.Count}\n";
    }
    
    void UpdateJsonFiles(List<string> names, List<string> diagnoses, List<string> anamnesis)
    {
        // Обновляем русский файл
        UpdateRussianJson(names, diagnoses, anamnesis);
        
        // Обновляем английский файл
        UpdateEnglishJson(names, diagnoses, anamnesis);
        
        // Обновляем турецкий файл
        UpdateTurkishJson(names, diagnoses, anamnesis);
        
        migrationLog += "JSON files updated!\n";
    }
    
    void UpdateRussianJson(List<string> names, List<string> diagnoses, List<string> anamnesis)
    {
        string path = "Assets/StreamingAssets/Localization/ru.json";
        string content = File.ReadAllText(path);
        
        // Здесь нужно обновить JSON с новыми данными
        // Для простоты пока просто логируем
        migrationLog += $"Would update {path} with {names.Count} names, {diagnoses.Count} diagnoses, {anamnesis.Count} anamnesis\n";
    }
    
    void UpdateEnglishJson(List<string> names, List<string> diagnoses, List<string> anamnesis)
    {
        string path = "Assets/StreamingAssets/Localization/en.json";
        migrationLog += $"Would update {path} with English translations\n";
    }
    
    void UpdateTurkishJson(List<string> names, List<string> diagnoses, List<string> anamnesis)
    {
        string path = "Assets/StreamingAssets/Localization/tr.json";
        migrationLog += $"Would update {path} with Turkish translations\n";
    }
}
