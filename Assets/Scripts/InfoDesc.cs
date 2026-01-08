using Assets.Scripts.Core;
using UnityEngine;

public class InfoDesc : MonoBehaviour, IInteractive
{
    [SerializeField] private string[] _dialogueLines;
    [SerializeField] private string _prompt = "Прочитать инструкцию";

    public string GetInteractionPrompt() => _prompt;

    public void Interact()
    {
        if (DialogueBoxUI.Instance != null)
        {
            DialogueBoxUI.Instance.ShowDialogueSequence(_dialogueLines);
        }
        else
        {
            Debug.LogError("DialogueBoxUI not found on scene!");
        }
    }
}
