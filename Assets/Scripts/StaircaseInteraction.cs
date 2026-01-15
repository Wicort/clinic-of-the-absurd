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
            // Получаем информацию о количестве пациентов через тот же метод, что и в проверке
            if (HospitalManager.Instance != null)
            {
                string floorPrefix = HospitalManager.Instance.CurrentFloorIndex + "_";
                
                // Находим все двери на текущем этаже
                WardDoor[] allWardDoors = FindObjectsByType<WardDoor>(FindObjectsSortMode.None);
                int totalWardsOnFloor = 0;
                int curedWardsOnFloor = 0;

                foreach (WardDoor door in allWardDoors)
                {
                    if (!door.GetDoorId().StartsWith(floorPrefix)) continue;

                    totalWardsOnFloor++;
                    
                    // Проверяем состояние палаты в WardStates
                    if (HospitalManager.Instance.WardStates.TryGetValue(door.GetDoorId(), out WardState state))
                    {
                        // Палата считается вылеченной, если она была посещена и пациент вылечен
                        if (state.hasBeenEntered && state.isCured)
                        {
                            curedWardsOnFloor++;
                        }
                    }
                }

                int remainingPatients = totalWardsOnFloor - curedWardsOnFloor;
                
                StaircaseTextType textType = remainingPatients <= 0 ? 
                    StaircaseTextType.NotVisited : 
                    StaircaseTextType.NotCured;
                
                string batmanLine = LocalizationManager.GetStaircaseText(textType, remainingPatients);

                DialogueBoxUI.Instance?.ShowDialogueSequence(new string[] { batmanLine });
            }
        }
    }

    private bool IsCurrentFloorFullyCured()
    {
        if (HospitalManager.Instance == null) return false;

        string floorPrefix = HospitalManager.Instance.CurrentFloorIndex + "_";

        Debug.Log($"Checking floor: {floorPrefix}");

        // Находим все двери на текущем этаже
        WardDoor[] allWardDoors = FindObjectsByType<WardDoor>(FindObjectsSortMode.None);
        int totalWardsOnFloor = 0;
        int curedWardsOnFloor = 0;

        foreach (WardDoor door in allWardDoors)
        {
            if (!door.GetDoorId().StartsWith(floorPrefix)) continue;

            totalWardsOnFloor++;
            
            // Проверяем состояние палаты в WardStates
            if (HospitalManager.Instance.WardStates.TryGetValue(door.GetDoorId(), out WardState state))
            {
                // Палата считается вылеченной, если она была посещена и пациент вылечен
                if (state.hasBeenEntered && state.isCured)
                {
                    curedWardsOnFloor++;
                }
            }
        }

        Debug.Log($"Floor stats: {curedWardsOnFloor}/{totalWardsOnFloor} wards cured");

        // Лестница доступна только если все палаты на этаже вылечены
        // И есть хотя бы одна палата на этаже
        return totalWardsOnFloor > 0 && curedWardsOnFloor >= totalWardsOnFloor;
    }
}
