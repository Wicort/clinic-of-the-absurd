using Assets.Scripts.Core;
using UnityEngine;

public class WardExitDoor : MonoBehaviour, IInteractive
{
    private string sourceDoorId; 
    private bool isCured = false;

    public void Initialize(string doorId, bool patientCured)
    {
        sourceDoorId = doorId;
        isCured = patientCured;
    }

    public string GetInteractionPrompt()
    {
        return isCured
            ? LocalizationManager.GetInteractionPrompt(InteractionPromptType.WardExitDoorCured)
            : LocalizationManager.GetInteractionPrompt(InteractionPromptType.WardExitDoor);
    }

    public void Interact()
    {
        if (WardManager.Instance != null)
        {
            WardManager.Instance.ExitWard(sourceDoorId, isCured);
        }
        else
        {
            Debug.LogError("WardManager not found!");
        }
    }
}
