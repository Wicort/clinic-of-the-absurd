using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonHoverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float animationDuration = 0.2f;
    [SerializeField] private Ease hoverEase = Ease.OutBack;
    [SerializeField] private Ease exitEase = Ease.OutBack;
    
    private Vector3 originalScale;
    private Tween currentTween;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale * hoverScale, animationDuration)
            .SetEase(hoverEase);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        currentTween?.Kill();
        currentTween = transform.DOScale(originalScale, animationDuration)
            .SetEase(exitEase);
    }

    private void OnDisable()
    {
        currentTween?.Kill();
        transform.localScale = originalScale;
    }
}
