using Assets.Scripts.Core;
using UnityEngine;

public class InfoDesc : MonoBehaviour, IInteractive
{
    [SerializeField] private InfoDescType _infoType = InfoDescType.Tutorial;
    [SerializeField] private string _prompt = "Получить информацию";

    public string GetInteractionPrompt() => LocalizationManager.GetInteractionPrompt(InteractionPromptType.InfoDesc);

    public void Interact()
    {
        if (DialogueBoxUI.Instance != null)
        {
            string[] localizedDialogue = LocalizationManager.GetInfoDescTexts(_infoType);
            DialogueBoxUI.Instance.ShowDialogueSequence(localizedDialogue);
        }
        else
        {
            Debug.LogError("DialogueBoxUI not found on scene!");
        }
    }
}
