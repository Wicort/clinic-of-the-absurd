using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class GagCardHoverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Animation Settings")]
    [SerializeField] private float _flyUpDistance = 50f;
    [SerializeField] private float _flyUpDuration = 0.3f;
    [SerializeField] private float _returnDuration = 0.4f;
    
    private Vector3 _originalPosition;
    private Tween _currentMoveTween;
    private RewardCardHoverAnimation _hoverAnimation;
    private bool _isHovered = false;
    private RectTransform _rectTransform;
    private HorizontalLayoutGroup _layoutGroup;
    
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _layoutGroup = GetComponentInParent<HorizontalLayoutGroup>();
        _hoverAnimation = GetComponent<RewardCardHoverAnimation>();
        
        // Сохраняем позицию в следующем кадре, после того как Layout Group расставит элементы
        StartCoroutine(SaveOriginalPosition());
    }
    
    private IEnumerator SaveOriginalPosition()
    {
        yield return new WaitForEndOfFrame(); // Ждем конца кадра
        _originalPosition = _rectTransform.anchoredPosition; // Сохраняем правильную позицию
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHovered = true;
        
        // Временно отключаем Layout Group чтобы избежать конфликтов
        if (_layoutGroup != null)
        {
            _layoutGroup.enabled = false;
        }
        
        // Анимация вылета вверх при наведении
        FlyUpAnimation();
        
        // Запускаем hover анимацию если компонент есть
        _hoverAnimation?.OnPointerEnter(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHovered = false;
        
        // Возвращаем в исходное положение
        ReturnToOriginalPosition();
        
        // Останавливаем hover анимацию
        _hoverAnimation?.OnPointerExit(eventData);
        
        // Включаем обратно Layout Group после завершения анимации
        if (_layoutGroup != null)
        {
            _currentMoveTween.OnComplete(() => {
                _layoutGroup.enabled = true;
            });
        }
    }
    
    private void FlyUpAnimation()
    {
        // Останавливаем текущие анимации
        _currentMoveTween?.Kill();
        
        // Поднимаем карту вверх и оставляем там
        // Используем anchoredPosition для UI элементов
        Vector3 targetPosition = _rectTransform.anchoredPosition;
        targetPosition.y += _flyUpDistance;
        
        _currentMoveTween = _rectTransform.DOAnchorPos(targetPosition, _flyUpDuration)
            .SetEase(Ease.OutBack);
    }
    
    private void ReturnToOriginalPosition()
    {
        // Останавливаем текущие анимации
        _currentMoveTween?.Kill();
        
        // Возвращаем в полностью оригинальную позицию
        _currentMoveTween = _rectTransform.DOAnchorPos(_originalPosition, _returnDuration)
            .SetEase(Ease.OutBounce);
    }
    
    private void OnDisable()
    {
        _currentMoveTween?.Kill();
        if (_isHovered)
        {
            // Возвращаем в оригинальную позицию
            _rectTransform.anchoredPosition = _originalPosition;
        }
        
        // Включаем обратно Layout Group при отключении
        if (_layoutGroup != null)
        {
            _layoutGroup.enabled = true;
        }
    }
    
    private void OnDestroy()
    {
        _currentMoveTween?.Kill();
    }
}
