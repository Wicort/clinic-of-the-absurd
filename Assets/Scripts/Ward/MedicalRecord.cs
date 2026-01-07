using Assets.Scripts.Core;
using UnityEngine;

public class MedicalRecord : MonoBehaviour, IInteractive
{
    private PatientProfile patient;

    public void Initialize(PatientProfile profile)
    {
        patient = profile;
    }

    public string GetInteractionPrompt() => "Прочитать медицинскую карту";

    public void Interact()
    {
        Debug.Log($"Medical Interact {patient != null}");
        if (patient == null) return;

        // Показываем анамнез
        string fullText = $"Диагноз: {patient.diagnosis}\n\nАнамнез:\n";
        foreach (string line in patient.anamnesisLines)
        {
            fullText += $"• {line}\n";
        }

        DialogueBoxUI.Instance.ShowDialogueSequence(new string[] { fullText });
    }
}
