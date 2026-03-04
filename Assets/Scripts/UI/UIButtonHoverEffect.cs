using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; 

public class UIButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [Header("Hover Settings")]
    [SerializeField] private float moveAmount = 20f; 
    [SerializeField] private float speed = 0.2f;

    private RectTransform rectTransform;
    private float originalX;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalX = rectTransform.anchoredPosition.x; 
    }

    // --- MOUSE HOVER ---
    public void OnPointerEnter(PointerEventData eventData) => MoveOut();
    public void OnPointerExit(PointerEventData eventData) => MoveBack();

    // --- CONTROLLER/KEYBOARD SELECT ---
    public void OnSelect(BaseEventData eventData) => MoveOut();
    public void OnDeselect(BaseEventData eventData) => MoveBack();

    // --- ANIMATION LOGIC ---
    private void MoveOut()
    {
        rectTransform.DOKill(); 
        rectTransform.DOAnchorPosX(originalX + moveAmount, speed).SetEase(Ease.OutQuad);
    }

    private void MoveBack()
    {
        rectTransform.DOKill();
        rectTransform.DOAnchorPosX(originalX, speed).SetEase(Ease.OutQuad);
    }

    void OnDisable()
    {
        // if the menu is turned off while hovered, snap it back to normal
        rectTransform.DOKill();
        rectTransform.anchoredPosition = new Vector2(originalX, rectTransform.anchoredPosition.y);
    }
}