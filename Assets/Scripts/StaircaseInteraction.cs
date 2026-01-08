using Assets.Scripts.Core;
using System;
using UnityEngine;

public class StaircaseInteraction : MonoBehaviour, IInteractive
{
    [SerializeField] private int _targetFloor = 2;
    [SerializeField] private string _promptWhenBlocked = "Подняться по лестнице (недоступно)";
    [SerializeField] private string _prompt = "Подняться по лестнице";

    public string GetInteractionPrompt()
    {
        return IsCurrentFloorFullyCured()
            ? _prompt
            : _promptWhenBlocked;
    }

    public void Interact()
    {
        if (IsCurrentFloorFullyCured())
        {
            HospitalManager.Instance?.LoadFloor(_targetFloor);
        }
        else
        {
            string batmanLine =
                "Ещё не все пациенты здесь вылечены.\n" +
                "А ты хотел убежать? Настоящий герой — до конца.\n" +
                "Растяни их улыбки до ушей.";

            DialogueBoxUI.Instance?.ShowDialogueSequence(new string[] { batmanLine });
        }
    }

    private bool IsCurrentFloorFullyCured()
    {
        if (HospitalManager.Instance == null) return false;

        string floorPrefix = HospitalManager.Instance.CurrentFloorIndex + "_";

        Debug.Log($"Checking floor: {floorPrefix}");

        foreach (var kvp in HospitalManager.Instance.WardStates)
        {
            string doorId = kvp.Key;
            WardState state = kvp.Value;

            if (!doorId.StartsWith(floorPrefix)) continue;

            Debug.Log($"  Door {doorId}: hasBeenEntered={state.hasBeenEntered}, isCured={state.isCured}");

            // Если палата была посещена, но не вылечена — этаж не готов
            if (state.hasBeenEntered && !state.isCured)
            {
                Debug.Log("❌ Not cured yet!");
                return false;
            }
        }
        Debug.Log("✅ All visited wards are cured.");

        // Если нет ни одной посещённой палаты — тоже не готов (защита от "просто пройти")
        bool hasAnyVisitedPatient = false;
        foreach (var kvp in HospitalManager.Instance.WardStates)
        {
            if (kvp.Key.StartsWith(floorPrefix) && kvp.Value.hasBeenEntered)
            {
                hasAnyVisitedPatient = true;
                break;
            }
        }

        return hasAnyVisitedPatient;

    }
}
