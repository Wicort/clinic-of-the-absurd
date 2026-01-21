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

        string patientInfo = $"Пациент: {patient.GetLocalizedName()}\nДиагноз: {patient.GetLocalizedDiagnosis()}";
        
        string anamnes = $"Анамнез:\n";
        string[] localizedAnamnesis = patient.GetLocalizedAnamnesis();
        foreach (string line in localizedAnamnesis)
        {
            anamnes += $"- {line}\n";
        }

        DialogueBoxUI.Instance.ShowDialogueSequence(new string[] { patientInfo, anamnes });
    }
}