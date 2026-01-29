using UnityEngine;
using UnityEngine.UI;

public class RewardButton : MonoBehaviour
{
    [SerializeField] private Text _nameText;
    [SerializeField] private Text _levelText;

    private HumorType _gagType;
    private System.Action<HumorType> _onSelected;

    public void Setup(HumorType type, System.Action<HumorType> callback)
    {
        var uiTexts = LocalizationManager.CurrentLanguage?.UITexts;

        _gagType = type;
        _onSelected = callback;

        _nameText.text = new GagCard(type).displayName;

        GagCard existing = GagDeck.Instance.GetCardByType(type);
        if (existing != null)
        {
            //_levelText.text = $"Уровень: {existing.level + 1}"; 
            _levelText.text = $"{uiTexts.CardLevel}: {existing.level + 1}"; 
        }
        else
        {
            _levelText.text = "Новый!";
        }
    }

    public void OnClick()
    {
        _onSelected?.Invoke(_gagType);
    }
    
    public void RefreshText()
    {
        _nameText.text = new GagCard(_gagType).displayName;
    }
}
