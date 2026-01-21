using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class PatientDataExtractor : EditorWindow
{
    private string extractedData = "";
    
    [MenuItem("Tools/Extract Patient Data")]
    public static void ShowWindow()
    {
        GetWindow<PatientDataExtractor>("Patient Data Extractor");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Patient Data Extractor", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("Extract Patient Data"))
        {
            ExtractData();
        }
        
        GUILayout.Space(10);
        GUILayout.Label("Extracted Data:");
        extractedData = EditorGUILayout.TextArea(extractedData, GUILayout.Height(400));
        
        if (GUILayout.Button("Copy to Clipboard"))
        {
            GUIUtility.systemCopyBuffer = extractedData;
            Debug.Log("Data copied to clipboard!");
        }
    }
    
    void ExtractData()
    {
        extractedData = "";
        string[] guids = AssetDatabase.FindAssets("t:PatientProfile");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            PatientProfile profile = AssetDatabase.LoadAssetAtPath<PatientProfile>(path);
            
            if (profile != null)
            {
                extractedData += $"=== {Path.GetFileNameWithoutExtension(path)} ===\n";
                extractedData += $"Name: {profile.patientName}\n";
                extractedData += $"Diagnosis: {profile.diagnosis}\n";
                extractedData += $"Anamnesis:\n";
                
                for (int i = 0; i < profile.anamnesisLines.Length; i++)
                {
                    extractedData += $"  {i + 1}. {profile.anamnesisLines[i]}\n";
                }
                
                extractedData += $"Humor Type: {profile.actualHumorType}\n";
                extractedData += $"Is Boss: {profile.isBoss}\n";
                extractedData += "\n";
            }
        }
        
        Debug.Log($"Extracted data from {guids.Length} patient profiles");
    }
}
