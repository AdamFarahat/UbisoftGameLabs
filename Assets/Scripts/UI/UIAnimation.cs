using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;

public class UIAnimation : MonoBehaviour
{
    private Vector2 cachedStartingPos;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RectTransform offscreenPositionPin;
    [SerializeField] private RectTransform anticipationPositionPin;
    [SerializeField] private RectTransform overshootPositionPin;
    [SerializeField] private float anticipationDuration = 0.2f;
    [SerializeField] private float actionDuration = 0.3f;
    [SerializeField] private float overshootDuration = 0.2f;

    void Start()
    {
        cachedStartingPos = rectTransform.anchoredPosition;
        StartCoroutine(waitSec());
    }
    // --- ANIMATION LOGIC --

    void OnDisable()
    {
        // Reset position 
        rectTransform.DOKill();
        rectTransform.anchoredPosition = cachedStartingPos;
    }

    public void AnimateOffScreen()
    {
        rectTransform.DOKill();
        Sequence offScreenSeq = DOTween.Sequence();

        // ANTICIPATION: Move forward a bit 
        // Vector2 totalPush = originalPosition.anchoredPosition + anticipationPositionPin.anchoredPosition;
        offScreenSeq.Append(rectTransform.DOAnchorPos(anticipationPositionPin.anchoredPosition, anticipationDuration).SetEase(Ease.OutQuad));

        // EXIT: Move completely off screen.
        offScreenSeq.Append(rectTransform.DOAnchorPos(offscreenPositionPin.anchoredPosition, actionDuration).SetEase(Ease.InQuad));
    }

    public void AnimateOnScreen()
{
    rectTransform.DOKill();
    Sequence onScreenSeq = DOTween.Sequence();

    onScreenSeq.Append(rectTransform.DOAnchorPos(overshootPositionPin.anchoredPosition, actionDuration).SetEase(Ease.OutQuad));
    onScreenSeq.Append(rectTransform.DOAnchorPos(cachedStartingPos, anticipationDuration).SetEase(Ease.InQuad));
}

    IEnumerator waitSec()
    {
        yield return new WaitForSeconds(3f);
        AnimateOffScreen();
        yield return new WaitForSeconds(3f);
        AnimateOnScreen();

    }
}