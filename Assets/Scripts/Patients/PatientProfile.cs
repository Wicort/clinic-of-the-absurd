using UnityEngine;

[CreateAssetMenu(fileName = "PatientProfile", menuName = "Game/Patient Profile")]
public class PatientProfile : ScriptableObject
{
    [Header("Общее")]
    public string patientName = "Пациент";
    public string diagnosis = "Диагноз неизвестен";
    
    // Поля для локализации (для будущего использования)
    [Header("Локализация (ключи)")]
    public string patientNameKey = "";
    public string diagnosisKey = "";

    [Header("Анамнез (намёки на тип юмора)")]
    [TextArea(3, 5)]
    public string[] anamnesisLines = new string[0];
    
    // Поля для локализации анамнеза
    [Header("Локализация анамнеза (ключи)")]
    public string[] anamnesisKeys = new string[0];

    [Header("Механика")]
    public HumorType actualHumorType;
    public HumorType[] forbiddenTypes = new HumorType[0];

    [Header("Босс")]
    public bool isBoss;
    public HumorType[] bossSequence = new HumorType[0];
    
    // Методы для получения локализованных данных
    public string GetLocalizedName()
    {
        if (!string.IsNullOrEmpty(patientNameKey))
            return LocalizationManager.GetPatientName(patientNameKey) ?? patientName;
            
        return patientName; // Возвращаем жестко закодированное имя если ключ не задан
    }
    
    public string GetLocalizedDiagnosis()
    {
        if (!string.IsNullOrEmpty(diagnosisKey))
            return LocalizationManager.GetPatientDiagnosis(diagnosisKey) ?? diagnosis;
            
        return diagnosis; // Возвращаем жестко закодированный диагноз если ключ не задан
    }
    
    public string[] GetLocalizedAnamnesis()
    {
        if (anamnesisKeys == null || anamnesisKeys.Length == 0)
            return anamnesisLines; // Возвращаем жестко закодированные строки если ключи не заданы
            
        var localizedLines = new string[anamnesisKeys.Length];
        for (int i = 0; i < anamnesisKeys.Length; i++)
        {
            localizedLines[i] = LocalizationManager.GetPatientAnamnesis(anamnesisKeys[i]) ?? anamnesisLines[i];
        }
        return localizedLines;
    }
}
