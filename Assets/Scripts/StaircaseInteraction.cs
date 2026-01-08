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
                "Товарищь игрок, если вы не обратили внимание, то на этом этаже еще остались грустные пациенты\n" +
                "Вон, внизу двери открытые. Что, непонятно, что это двери? \n" +
                "Ну, простите, времени было мало, чтоб нарисовать понятнее...";

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

            if (state.hasBeenEntered && !state.isCured)
            {
                return false;
            }
        }

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
