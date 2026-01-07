using Assets.Scripts.Core;
using UnityEngine;

public class WardExitDoor : MonoBehaviour, IInteractive
{
    private string sourceDoorId; // ID двери, через которую вошли
    private bool isCured = false;

    public void Initialize(string doorId, bool patientCured)
    {
        sourceDoorId = doorId;
        isCured = patientCured;
    }

    public string GetInteractionPrompt()
    {
        return isCured
            ? "¬ернутьс€ в холл (пациент вылечен!)"
            : "¬ернутьс€ в холл";
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
