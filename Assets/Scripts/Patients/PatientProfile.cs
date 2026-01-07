using UnityEngine;

[CreateAssetMenu(fileName = "PatientProfile", menuName = "Game/Patient Profile")]
public class PatientProfile : ScriptableObject
{
    [Header("Общее")]
    public string patientName = "Пациент";
    public string diagnosis = "Диагноз неизвестен";

    [Header("Анамнез (намёки на тип юмора)")]
    [TextArea(3, 5)]
    public string[] anamnesisLines = new string[0];

    [Header("Механика")]
    public HumorType actualHumorType;
    public HumorType[] forbiddenTypes = new HumorType[0];

    [Header("Босс")]
    public bool isBoss;
    public HumorType[] bossSequence = new HumorType[0];
}
