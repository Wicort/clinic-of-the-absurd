using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PatientDatabase", menuName = "Game/Patient Database")]
public class PatientDatabase : ScriptableObject
{
    [Header("Easy Patients (Floor 1)")]
    [SerializeField] private PatientProfile[] easyPatients;

    [Header("Hard Patients (Floor 2)")]
    [SerializeField] private PatientProfile[] hardPatients;

    [Header("Boss (Floor 3)")]
    [SerializeField] private PatientProfile bossPatient;

    [Header("На будущее")]
    [SerializeField] private PatientProfile[] rarePatients;

    // Кэш для быстрого доступа
    private Dictionary<DifficultyLevel, PatientProfile[]> patientPools;

    private void OnEnable()
    {
        // Инициализируем пул (для редактора и игры)
        InitializePools();
    }

    private void InitializePools()
    {
        patientPools = new Dictionary<DifficultyLevel, PatientProfile[]>
        {
            { DifficultyLevel.Easy, easyPatients != null ? easyPatients : new PatientProfile[0] },
            { DifficultyLevel.Hard, hardPatients != null ? hardPatients : new PatientProfile[0] }
        };
    }

    public PatientProfile GetRandomPatient(DifficultyLevel level)
    {
        if (level == DifficultyLevel.Boss)
        {
            if (bossPatient == null)
            {
                Debug.LogWarning("Boss patient not assigned in PatientDatabase!");
                return null;
            }
            return bossPatient;
        }

        if (!patientPools.TryGetValue(level, out PatientProfile[] pool))
        {
            Debug.LogError($"No patient pool for level {level}");
            return null;
        }

        if (pool.Length == 0)
        {
            Debug.LogWarning($"Patient pool for {level} is empty!");
            return null;
        }

        int index = Random.Range(0, pool.Length);
        return pool[index];
    }

    public void LogStats()
    {
        Debug.Log($"PatientDatabase: Easy={easyPatients?.Length ?? 0}, Hard={hardPatients?.Length ?? 0}, Boss={(bossPatient ? 1 : 0)}");
    }

    public PatientProfile FindPatientByName(string name)
    {
        if (name == null) return null;

        foreach (var profile in easyPatients)
            if (profile != null && profile.name == name) return profile;

        foreach (var profile in hardPatients)
            if (profile != null && profile.name == name) return profile;

        if (bossPatient != null && bossPatient.name == name)
            return bossPatient;

        return null;
    }
}
