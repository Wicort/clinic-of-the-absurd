using Assets.Scripts.Core;
using System;
using UnityEngine;

public class StaircaseInteraction : MonoBehaviour, IInteractive
{
    [SerializeField] private int _targetFloor = 2;
    [SerializeField] private string _prompt = "Подняться по лестнице";

    public string GetInteractionPrompt() => _prompt;

    public void Interact()
    {
        if (HospitalManager.Instance != null)
        {
            HospitalManager.Instance.LoadFloor(_targetFloor);
        }
        else
        {
            Debug.LogError("HospitalManager not found!");
        }
    }
}
