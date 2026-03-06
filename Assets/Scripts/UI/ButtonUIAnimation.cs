using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;

public class ButtonUIAnimation : UIAnimation
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


    // --- ANIMATION LOGIC ---
    public void HoverForward()
    {
        if (isExiting) return; 

        rectTransform.DOKill(); 
        rectTransform.DOAnchorPos(cachedStartingPos + new Vector2(hoverOffset, 0.0f), hoverDuration).SetEase(Ease.OutQuad);
    }

    public void HoverBack()
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

   public override Sequence AnimateOut()
    {
        if (isExiting) return null; 
        isExiting = true; 

        // Add bonus if it is selected
        if (isSelected)
        anticipationPositionPin.anchoredPosition += new Vector2(selectedBonusOffset, 0);

        return base.AnimateOut();
    }

    public override Sequence AnimateIn()
    {
        Sequence sequence = base.AnimateIn();
        isExiting = true; 
        sequence.OnComplete(() => isExiting = false);

        return sequence;
    }

}