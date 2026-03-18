using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;

public class ButtonSelect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private ButtonUIAnimation buttonParent;

    void Start()
    {
        buttonParent = GetComponentInParent<ButtonUIAnimation>();
    }

    
    // --- MOUSE HOVER ---
    public void OnPointerEnter(PointerEventData eventData) => buttonParent.HoverForward();
    public void OnPointerExit(PointerEventData eventData) => buttonParent.HoverBack();


        // --- CONTROLLER/KEYBOARD SELECT ---
    public void OnSelect(BaseEventData eventData) => buttonParent.HoverForward();
    public void OnDeselect(BaseEventData eventData) => buttonParent.HoverBack();


}