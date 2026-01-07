using Assets.Scripts.Core;
using UnityEngine;

public class InfoDesc : MonoBehaviour, IInteractive
{
    [Header("Dialogue")]
    [TextArea(2, 4)]
    [SerializeField] private string[] dialogueLines;

    [Header("References")]
    [SerializeField] private DialogueBoxUI dialogueBox; // UI для "облачка"

    public string GetInteractionPrompt() => "Подойти к справочной";

    public void Interact()
    {
        if (dialogueBox != null)
        {
            dialogueBox.ShowDialogueSequence(dialogueLines);
        }
        else
        {
            Debug.LogWarning("DialogueBoxUI not assigned to InfoDesk!");
        }
    }
}
