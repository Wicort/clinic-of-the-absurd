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

        string patientInfo = $"{LocalizationManager.GetMedicalRecordPatient()} {patient.GetLocalizedName()}\n{LocalizationManager.GetMedicalRecordDiagnosis()} {patient.GetLocalizedDiagnosis()}";
        
        string anamnes = $"{LocalizationManager.GetMedicalRecordAnamnesis()}\n";
        string[] localizedAnamnesis = patient.GetLocalizedAnamnesis();
        foreach (string line in localizedAnamnesis)
        {
            anamnes += $"- {line}\n";
        }

        DialogueBoxUI.Instance.ShowDialogueSequence(new string[] { patientInfo, anamnes });
    }
}