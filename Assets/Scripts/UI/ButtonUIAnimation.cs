using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;

public class ButtonUIAnimation : UIAnimation, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private float hoverDuration = 0.2f;
    [SerializeField] private float hoverOffset;
    [SerializeField] private float selectedBonusOffset; 
    private bool isExiting = false; 
    private bool isSelected = false; 

    public float HoverOffset { get => hoverOffset; set => hoverOffset = value; }
    public float HoverDuration { get => hoverDuration; set => hoverDuration = value; }
    public float SelectedBonusOffset { get => selectedBonusOffset; set => selectedBonusOffset = value; }
    public bool IsSelected { get => isSelected; set => isSelected = value; }


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
        rectTransform.DOAnchorPos(cachedStartingPos + new Vector2(hoverOffset, 0.0f), hoverDuration).SetEase(Ease.OutQuad);
    }

    private void HoverBack()
    {
        if (isExiting) return; 

        rectTransform.DOKill();
        rectTransform.DOAnchorPos(cachedStartingPos, hoverDuration).SetEase(Ease.OutQuad);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        isExiting = false;        
    }

   public override void AnimateOffScreen()
    {
        if (isExiting) return; 
        isExiting = true; 

        // Add bonus if it is selected
        if (isSelected)
        anticipationPositionPin.anchoredPosition += new Vector2(selectedBonusOffset, 0);

        base.AnimateOffScreen();
    }

    public override void AnimateOnScreen()
    {
        if (isExiting) return; 
        isExiting = true; 

        base.AnimateOnScreen();
    }

}