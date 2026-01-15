using Assets.Scripts.Core;
using UnityEngine;

public class InfoDesc : MonoBehaviour, IInteractive
{
    [SerializeField] private string[] _dialogueLines;
    [SerializeField] private string _prompt = "��������� ����������";

    public string GetInteractionPrompt() => LocalizationManager.GetInteractionPrompt(InteractionPromptType.InfoDesc);

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
