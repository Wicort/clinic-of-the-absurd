using UnityEngine;

public class GagRewardScreen : MonoBehaviour
{
    public static GagRewardScreen Instance { get; private set; }

    [SerializeField] private GameObject _rewardButtonPrefab;
    [SerializeField] private Transform _buttonsParent;

    private System.Action<HumorType> _onRewardSelected;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
        gameObject.SetActive(false);
    }

    public void ShowRewardScreen(HumorType[] options, System.Action<HumorType> onSelected)
    {
        _onRewardSelected = onSelected;
        gameObject.SetActive(true);

        // Очистить старые кнопки
        foreach (Transform child in _buttonsParent)
            Destroy(child.gameObject);

        // Создать новые
        foreach (HumorType option in options)
        {
            GameObject btnObj = Instantiate(_rewardButtonPrefab, _buttonsParent);
            RewardButton btn = btnObj.GetComponent<RewardButton>();
            if (btn != null)
            {
                btn.Setup(option, OnRewardChosen);
            }
        }
    }

    private void OnRewardChosen(HumorType type)
    {
        _onRewardSelected?.Invoke(type);
        gameObject.SetActive(false);
    }
}
