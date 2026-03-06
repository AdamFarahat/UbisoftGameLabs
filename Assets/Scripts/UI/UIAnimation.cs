using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;

public class UIAnimation : MonoBehaviour
{
    protected Vector2 cachedStartingPos;
    [SerializeField] protected RectTransform rectTransform;
    [SerializeField] private RectTransform offscreenPositionPin;
    [SerializeField] protected RectTransform anticipationPositionPin;
    [SerializeField] private RectTransform overshootPositionPin;
    [SerializeField] private float anticipationDuration = 0.2f;
    [SerializeField] private float actionDuration = 0.3f;
    [SerializeField] private float overshootDuration = 0.2f;

    void Start()
    {
        cachedStartingPos = rectTransform.anchoredPosition;
        // StartCoroutine(waitSec());
    }
    // --- ANIMATION LOGIC --

    protected virtual void OnDisable()
    {
        // Reset position 
        rectTransform.DOKill();
        rectTransform.anchoredPosition = cachedStartingPos;
    }

    public virtual void AnimateOffScreen()
    {
        rectTransform.DOKill();
        Sequence offScreenSeq = DOTween.Sequence();

        // ANTICIPATION: Move forward a bit 
        offScreenSeq.Append(rectTransform.DOAnchorPos(anticipationPositionPin.anchoredPosition, anticipationDuration).SetEase(Ease.OutQuad));

        // EXIT: Move completely off screen.
        offScreenSeq.Append(rectTransform.DOAnchorPos(offscreenPositionPin.anchoredPosition, actionDuration).SetEase(Ease.InQuad));
    }

    public virtual void AnimateOnScreen()
    {
        rectTransform.DOKill();
        Sequence onScreenSeq = DOTween.Sequence();

        // OVERSHOOT: Move a little too forward
        onScreenSeq.Append(rectTransform.DOAnchorPos(overshootPositionPin.anchoredPosition, actionDuration).SetEase(Ease.OutQuad));
        // ENTER: Move onto the screen
        onScreenSeq.Append(rectTransform.DOAnchorPos(cachedStartingPos, overshootDuration).SetEase(Ease.InQuad));
    }
}