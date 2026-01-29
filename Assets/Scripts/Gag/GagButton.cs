using UnityEngine;
using UnityEngine.UI;

public class GagButton : MonoBehaviour
{
    [SerializeField] private Text _buttonText;
    [SerializeField] private Text _labelText;
    private HumorType _gagType;
    private System.Action<HumorType> _onSelected;

    public void Setup(HumorType type, System.Action<HumorType> callback)
    {
        var uiTexts = LocalizationManager.CurrentLanguage?.UITexts;

        _gagType = type;
        _onSelected = callback;

        if (_buttonText != null)
            _buttonText.text = new GagCard(type).displayName;

        GagCard existing = GagDeck.Instance.GetCardByType(type);

        if (existing != null && _labelText != null)
        {
            _labelText.text = $"{uiTexts.CardLevel}: {existing.level}";
        }
    }

    public void OnClick()
    {
        _onSelected?.Invoke(_gagType);
    }
    
    public void RefreshText()
    {
        if (_buttonText != null)
            _buttonText.text = new GagCard(_gagType).displayName;
    }
}
