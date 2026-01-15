using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;

public class GagRewardScreen : MonoBehaviour
{
    public static GagRewardScreen Instance { get; private set; }

    [SerializeField] private GameObject _rewardButtonPrefab;
    [SerializeField] private Transform _buttonsParent;
    
    [Header("Animation Settings")]
    [SerializeField] private float _animationDuration = 0.6f;
    [SerializeField] private float _staggerDelay = 0.15f;
    [SerializeField] private Ease _scaleEase = Ease.OutElastic;
    [SerializeField] private Vector3 _startScale = Vector3.zero;

    private System.Action<HumorType> _onRewardSelected;
    private HorizontalLayoutGroup _layoutGroup;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
        gameObject.SetActive(false);
        
        // Получаем или добавляем HorizontalLayoutGroup
        _layoutGroup = _buttonsParent.GetComponent<HorizontalLayoutGroup>();
        if (_layoutGroup == null)
        {
            _layoutGroup = _buttonsParent.gameObject.AddComponent<HorizontalLayoutGroup>();
        }
        
        // Настраиваем Layout Group
        _layoutGroup.spacing = 50f; // Расстояние между кнопками
        _layoutGroup.childForceExpandWidth = false;
        _layoutGroup.childForceExpandHeight = false;
        _layoutGroup.childControlWidth = false;
        _layoutGroup.childControlHeight = false;
        _layoutGroup.childAlignment = TextAnchor.MiddleCenter;
    }

    public void ShowRewardScreen(HumorType[] options, System.Action<HumorType> onSelected)
    {
        _onRewardSelected = onSelected;
        gameObject.SetActive(true);

        // Очищаем старые кнопки
        foreach (Transform child in _buttonsParent)
            Destroy(child.gameObject);

        // Создаем и анимируем новые кнопки
        StartCoroutine(AnimateRewardButtons(options));
    }

    private IEnumerator AnimateRewardButtons(HumorType[] options)
    {
        // Создаем все кнопки сразу, но делаем их невидимыми
        GameObject[] buttonObjects = new GameObject[options.Length];
        
        for (int i = 0; i < options.Length; i++)
        {
            GameObject btnObj = Instantiate(_rewardButtonPrefab, _buttonsParent);
            buttonObjects[i] = btnObj;
            
            RewardButton btn = btnObj.GetComponent<RewardButton>();
            if (btn != null)
            {
                btn.Setup(options[i], OnRewardChosen);
            }

            // Добавляем компонент анимации при наведении
            RewardCardHoverAnimation hoverAnimation = btnObj.GetComponent<RewardCardHoverAnimation>();
            if (hoverAnimation == null)
            {
                hoverAnimation = btnObj.AddComponent<RewardCardHoverAnimation>();
            }
            
            // Определяем положение карты (левая или правая)
            bool isLeftCard = (i == 0 && options.Length == 2) || (i < options.Length / 2f);
            hoverAnimation.SetCardPosition(isLeftCard);

            // Начальная настройка для анимации
            btnObj.transform.localScale = _startScale;
            
            // Делаем невидимым в начале
            CanvasGroup canvasGroup = btnObj.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = btnObj.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
        }

        // Принудительно обновляем Layout Group после создания всех кнопок
        LayoutRebuilder.ForceRebuildLayoutImmediate(_buttonsParent.GetComponent<RectTransform>());
        yield return new WaitForSeconds(0.1f); // Даем время на пересчет

        // Анимируем появление с задержкой
        for (int i = 0; i < buttonObjects.Length; i++)
        {
            GameObject btnObj = buttonObjects[i];
            CanvasGroup canvasGroup = btnObj.GetComponent<CanvasGroup>();
            
            // Анимация масштабирования
            btnObj.transform.DOScale(Vector3.one, _animationDuration)
                .SetEase(_scaleEase);
                
            // Анимация появления
            canvasGroup.DOFade(1f, _animationDuration * 0.7f)
                .SetEase(Ease.OutQuad);

            // Задержка перед следующей кнопкой
            if (i < buttonObjects.Length - 1)
                yield return new WaitForSeconds(_staggerDelay);
        }
    }

    private void OnRewardChosen(HumorType type)
    {
        _onRewardSelected?.Invoke(type);
        gameObject.SetActive(false);
    }
}
