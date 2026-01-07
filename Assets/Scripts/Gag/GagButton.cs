using System;
using UnityEngine;
using UnityEngine.UI;

public class GagButton : MonoBehaviour
{
    [SerializeField] private Text _buttonText;
    private HumorType _gagType;
    private System.Action<HumorType> _onSelected;

    public void Setup(HumorType type, System.Action<HumorType> callback)
    {
        _gagType = type;
        _onSelected = callback;
        if (_buttonText != null)
            _buttonText.text = new GagCard(type).displayName;
    }

    public void OnClick()
    {
        _onSelected?.Invoke(_gagType);
    }
}
