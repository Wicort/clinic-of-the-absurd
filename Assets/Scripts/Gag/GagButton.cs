using UnityEngine;
using UnityEngine.UI;

public class GagButton : MonoBehaviour
{
    [SerializeField] private Text _buttonText;
    [SerializeField] private Text _labelText;
    [SerializeField] private Image _cardImage;
    private HumorType _gagType;
    private System.Action<HumorType> _onSelected;

    public void Setup(HumorType type, System.Action<HumorType> callback)
    {
        var uiTexts = LocalizationManager.CurrentLanguage?.UITexts;

        _gagType = type;
        _onSelected = callback;

        if (_buttonText != null)
            _buttonText.text = new GagCard(type).displayName;

        // Устанавливаем иконку в зависимости от типа гэга
        if (_cardImage != null)
        {
            Sprite gagIcon = GagIconManager.GetGagIcon(type);
            _cardImage.sprite = gagIcon;
        }

        GagCard existing = GagDeck.Instance.GetCardByType(type);

        if (existing != null && _labelText != null)
        {
            _labelText.text = $"{uiTexts.CardLevel}: {existing.level}";
        }
    }

    public void OnClick()
    {
        AudioUtils.PlayClick();
        _onSelected?.Invoke(_gagType);
    }
    
    public void RefreshText()
    {
        if (_buttonText != null)
            _buttonText.text = new GagCard(_gagType).displayName;
    }
}
