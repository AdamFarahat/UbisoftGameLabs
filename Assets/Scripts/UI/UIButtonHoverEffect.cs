using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; 

public class UIButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private RectTransform rectTransform;
    // Hover settings
    private float originalX;
    private float hoverOffsetX; 
    private float hoverDuration;

    // Exit settings
    private bool isExiting = false; 
    private float exitPositionX; 
    private float anticipationDuration;
    private float exitDuration;

    public float HoverOffsetX { get => hoverOffsetX; set => hoverOffsetX = value; }
    public float HoverDuration { get => hoverDuration; set => hoverDuration = value; }
    public float ExitPositionX { get => exitPositionX; set => exitPositionX = value; }
    public float AnticipationDuration { get => anticipationDuration; set => anticipationDuration = value; }
    public float ExitDuration { get => exitDuration; set => exitDuration = value; }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalX = rectTransform.anchoredPosition.x; 
    }

    // --- MOUSE HOVER ---
    public void OnPointerEnter(PointerEventData eventData) => HoverForward();
    public void OnPointerExit(PointerEventData eventData) => HoverBack();

    // --- CONTROLLER/KEYBOARD SELECT ---
    public void OnSelect(BaseEventData eventData) => HoverForward();
    public void OnDeselect(BaseEventData eventData) => HoverBack();

    // --- ANIMATION LOGIC ---
    private void HoverForward()
    {
        if (isExiting) return; 

        rectTransform.DOKill(); 
        rectTransform.DOAnchorPosX(originalX + hoverOffsetX, hoverDuration).SetEase(Ease.OutQuad);
    }

    private void HoverBack()
    {
        if (isExiting) return; 

        rectTransform.DOKill();
        rectTransform.DOAnchorPosX(originalX, hoverDuration).SetEase(Ease.OutQuad);
    }

    void OnDisable()
    {
        // Reset position 
        rectTransform.DOKill();
        rectTransform.anchoredPosition = new Vector2(originalX, rectTransform.anchoredPosition.y);
        isExiting = false; 
    }

    public void AnimateOffScreen(float extraPush = 0f)
    {
        if (isExiting) return; 
        isExiting = true; 

        rectTransform.DOKill();
        Sequence offScreenSeq = DOTween.Sequence();

        // ANTICIPATION: Move forward a bit + any extra push for being selected
        float totalPush = originalX + hoverOffsetX + extraPush;
        offScreenSeq.Append(rectTransform.DOAnchorPosX(totalPush, anticipationDuration).SetEase(Ease.OutQuad));

        // EXIT: Move completely off screen.
        offScreenSeq.Append(rectTransform.DOAnchorPosX(exitPositionX, exitDuration).SetEase(Ease.InQuad));
    }
}