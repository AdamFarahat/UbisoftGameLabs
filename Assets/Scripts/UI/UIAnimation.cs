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
    [SerializeField] private float actionOutDuration = 0.3f;
    [SerializeField] private float actionInDuration = 0.6f;
    [SerializeField] private float overshootDuration = 0.2f;

    public float ActionDuration { get => actionOutDuration; set => actionOutDuration = value; }


    // --- ANIMATION LOGIC --

    protected virtual void OnDisable()
    {
        // Reset position 
        rectTransform.DOKill();
        rectTransform.anchoredPosition = cachedStartingPos;
    }

    // Animate UI with a slight anticipation 
    public virtual Sequence AnimateOut()
    {
        rectTransform.DOKill();
        Sequence offScreenSeq = DOTween.Sequence();

        // ANTICIPATION: Move forward a bit 
        offScreenSeq.Append(rectTransform.DOAnchorPos(anticipationPositionPin.anchoredPosition, anticipationDuration).SetEase(Ease.OutQuad));

        // EXIT: Move completely off screen.
        offScreenSeq.Append(rectTransform.DOAnchorPos(offscreenPositionPin.anchoredPosition, actionOutDuration).SetEase(Ease.InQuad));
        return offScreenSeq;
    }

    // Animate UI with a slight overshoot
    public virtual Sequence AnimateIn()
    {
        rectTransform.DOKill();
        Sequence onScreenSeq = DOTween.Sequence();

        // OVERSHOOT: Move a little too forward
        onScreenSeq.Append(rectTransform.DOAnchorPos(overshootPositionPin.anchoredPosition, actionInDuration).SetEase(Ease.OutQuad));
        // ENTER: Move onto the screen
        onScreenSeq.Append(rectTransform.DOAnchorPos(cachedStartingPos, overshootDuration).SetEase(Ease.InQuad));
        return onScreenSeq;
    }

    // Grabs the initial position and places the buttons off screen
    public void PlaceOffScreen()
    {
        cachedStartingPos = rectTransform.anchoredPosition;
        rectTransform.anchoredPosition = offscreenPositionPin.anchoredPosition;
    }
}