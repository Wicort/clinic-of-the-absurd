using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueBoxUI : MonoBehaviour
{
    public static DialogueBoxUI Instance { get; private set; }

    [SerializeField] private Text dialogueText;
    private PlayerControls inputActions;

    private string[] currentLines;
    private int currentLineIndex = 0;
    private bool isShowingDialogue = false;

    public bool IsShowing => isShowingDialogue;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        gameObject.SetActive(false);
        inputActions = new PlayerControls();
    }

    private void OnEnable()
    {
        if (inputActions == null) return;

        inputActions.Player.Interact.Enable();
        inputActions.Player.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        if (inputActions == null) return;

        inputActions.Player.Interact.performed -= OnInteract;
        inputActions.Player.Interact.Disable();
    }

    private void OnDestroy()
    {
        if (inputActions != null)
        {
            inputActions.Dispose();
            inputActions = null;
        }

        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void ShowDialogueSequence(string[] lines)
    {
        if (lines == null || lines.Length == 0) return;

        currentLines = lines;
        currentLineIndex = 0;
        isShowingDialogue = true;
        gameObject.SetActive(true);
        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (currentLineIndex < currentLines.Length)
        {
            dialogueText.text = currentLines[currentLineIndex];
        }
        else
        {
            CloseDialogue();
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (!isShowingDialogue) return;

        currentLineIndex++;
        ShowCurrentLine();
    }

    private void CloseDialogue()
    {
        isShowingDialogue = false;
        currentLines = null;
        currentLineIndex = 0;
        gameObject.SetActive(false);
    }
}
