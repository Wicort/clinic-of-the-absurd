using Assets.Scripts.Core;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class PlayerInteraction : MonoBehaviour
{
    private IInteractive currentInteractive = null;
    private PlayerControls inputActions;

    private void Awake()
    {
        inputActions = new PlayerControls();
    }

    private void OnEnable()
    {
        inputActions.Player.Interact.performed += OnInteract;
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Interact.performed -= OnInteract;
        inputActions.Disable();
    }

    private void OnDestroy()
    {
        inputActions?.Dispose();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var interactive = other.GetComponent<IInteractive>();
        if (interactive != null)
        {
            currentInteractive = interactive;
            InteractionPromptUI.Instance.Show(interactive.GetInteractionPrompt());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<IInteractive>() == currentInteractive)
        {
            currentInteractive = null;
            InteractionPromptUI.Instance.Hide();
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        // Ѕлокируем взаимодействие, если открыт диалог
        if (DialogueBoxUI.Instance != null && DialogueBoxUI.Instance.IsShowing)
            return;

        if (currentInteractive != null)
        {
            currentInteractive.Interact();
        }
    }
}
