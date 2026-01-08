using Assets.Scripts.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPromptUI: MonoBehaviour
{
    public static InteractionPromptUI Instance { get; private set; }

    public Text PromptText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            gameObject.SetActive(false);
        }
    }

    public void Show(string prompt)
    {
        if (PromptText != null)
            PromptText.text = prompt;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void SetPrompt(string prompt)
    {
    }

}
