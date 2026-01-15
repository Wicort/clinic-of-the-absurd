using Assets.Scripts.Core;
using UnityEngine;

public class MedicalRecord : MonoBehaviour, IInteractive
{
    private PatientProfile patient;

    public void Initialize(PatientProfile profile)
    {
        patient = profile;
    }

    public string GetInteractionPrompt() => LocalizationManager.GetInteractionPrompt(InteractionPromptType.MedicalRecord);

    public void Interact()
    {
        if (patient == null) return;

        string patientInfo = $"Пациент: {patient.patientName}\nДиагноз: {patient.diagnosis}";
        
        string anamnes = $"Анамнез:\n";
        foreach (string line in patient.anamnesisLines)
        {
            anamnes += $"- {line}\n";
        }

        DialogueBoxUI.Instance.ShowDialogueSequence(new string[] { patientInfo, anamnes });
    }
}
