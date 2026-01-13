using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class RewardCardHoverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Animation Settings")]
    [SerializeField] private float _pulseDuration = 0.2f;
    [SerializeField] private float _slowPulseDuration = 1.0f;
    [SerializeField] private float _rotationDuration = 0.3f;
    [SerializeField] private Ease _pulseEase = Ease.InOutSine;
    [SerializeField] private Ease _rotationEase = Ease.OutBack;
    
    [Header("Rotation Settings")]
    [SerializeField] private float _rotationAngle = 10f;
    [SerializeField] private bool _isLeftCard = false; // Левая карта - против часовой, правая - по часовой

    private Vector3 _originalScale;
    private Quaternion _originalRotation;
    private Tween _currentPulseTween;
    private Tween _currentRotationTween;

    private void Awake()
    {
        _originalScale = transform.localScale;
        _originalRotation = transform.rotation;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Останавливаем текущие анимации
        _currentPulseTween?.Kill();
        _currentRotationTween?.Kill();

        // Запускаем анимацию пульсации
        StartPulseAnimation();
        
        // Запускаем анимацию поворота
        StartRotationAnimation(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Останавливаем текущие анимации
        _currentPulseTween?.Kill();
        _currentRotationTween?.Kill();

        // Возвращаем к исходному состоянию
        _currentPulseTween = transform.DOScale(_originalScale, 0.3f)
            .SetEase(Ease.OutSine);
            
        _currentRotationTween = transform.DORotateQuaternion(_originalRotation, _rotationDuration)
            .SetEase(Ease.OutSine);
    }

    private void StartPulseAnimation()
    {
        // Создаем последовательность пульсации: 115% -> 100% -> 110% -> 100% -> 105%
        Sequence pulseSequence = DOTween.Sequence();

        // 115%
        pulseSequence.Append(transform.DOScale(_originalScale * 1.15f, _pulseDuration)
            .SetEase(_pulseEase));
            
        // 100%
        pulseSequence.Append(transform.DOScale(_originalScale, _pulseDuration * 0.8f)
            .SetEase(_pulseEase));
            
        // 110%
        /*pulseSequence.Append(transform.DOScale(_originalScale * 1.10f, _pulseDuration * 0.7f)
            .SetEase(_pulseEase));
            
        // 100%
        pulseSequence.Append(transform.DOScale(_originalScale, _pulseDuration * 0.6f)
            .SetEase(_pulseEase));*/
            
        // 105%
        pulseSequence.Append(transform.DOScale(_originalScale * 1.05f, _pulseDuration * 0.5f)
            .SetEase(_pulseEase));

        // Добавляем медленную пульсацию 105-107% после основной анимации
        Sequence slowPulse = DOTween.Sequence();
        slowPulse.Append(transform.DOScale(_originalScale * 1.07f, _slowPulseDuration)
            .SetEase(Ease.InOutSine));
        slowPulse.Append(transform.DOScale(_originalScale * 1.05f, _slowPulseDuration)
            .SetEase(Ease.InOutSine));

        // Зацикливаем только медленную пульсацию
        _currentPulseTween = pulseSequence.Append(slowPulse.SetLoops(-1, LoopType.Restart));
    }

    private void StartRotationAnimation(bool isEntering)
    {
        float targetAngle = isEntering ? 
            (_isLeftCard ? _rotationAngle : -_rotationAngle) : 0f;
            
        Vector3 targetRotation = new Vector3(0, 0, targetAngle);
        
        _currentRotationTween = transform.DORotate(targetRotation, _rotationDuration)
            .SetEase(_rotationEase);
    }

    public void SetCardPosition(bool isLeftCard)
    {
        _isLeftCard = isLeftCard;
    }

    private void OnDisable()
    {
        // Останавливаем все анимации при отключении
        _currentPulseTween?.Kill();
        _currentRotationTween?.Kill();
        
        // Возвращаем к исходному состоянию
        transform.localScale = _originalScale;
        transform.rotation = _originalRotation;
    }

    private void OnDestroy()
    {
        _currentPulseTween?.Kill();
        _currentRotationTween?.Kill();
    }
}
