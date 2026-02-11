using UnityEngine;
using UnityEngine.UI;

public class RewardButton : MonoBehaviour
{
    [SerializeField] private Text _nameText;
    [SerializeField] private Text _levelText;
    [SerializeField] private Image _cardImage;

    private HumorType _gagType;
    private System.Action<HumorType> _onSelected;

    public void Setup(HumorType type, System.Action<HumorType> callback)
    {
        var uiTexts = LocalizationManager.CurrentLanguage?.UITexts;

        _gagType = type;
        _onSelected = callback;

        _nameText.text = new GagCard(type).displayName;

        // Устанавливаем иконку в зависимости от типа гэга
        if (_cardImage != null)
        {
            Sprite gagIcon = GagIconManager.GetGagIcon(type);
            _cardImage.sprite = gagIcon;
        }

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
        AudioUtils.PlayClick();
        _onSelected?.Invoke(_gagType);
    }
    
    public void RefreshText()
    {
        _nameText.text = new GagCard(_gagType).displayName;
    }
}
