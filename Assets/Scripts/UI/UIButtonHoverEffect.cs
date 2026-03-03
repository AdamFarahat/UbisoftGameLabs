// using UnityEngine;
// using UnityEngine.EventSystems;
// using DG.Tweening; 

// public class UIButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
// {
//     [Header("Hover Settings")]
//     [SerializeField] private float moveAmount = 20f; // How far it moves on the X axis
//     [SerializeField] private float duration = 0.2f;  // How fast it moves

//     private RectTransform rectTransform;
//     private float originalX;

//     void Awake()
//     {
//         rectTransform = GetComponent<RectTransform>();
//         // Save the starting position so we always know where "home" is
//         originalX = rectTransform.anchoredPosition.x; 
//     }

//     // --- MOUSE HOVER ---
//     public void OnPointerEnter(PointerEventData eventData) => MoveOut();
//     public void OnPointerExit(PointerEventData eventData) => MoveBack();

//     // --- CONTROLLER/KEYBOARD SELECT ---
//     public void OnSelect(BaseEventData eventData) => MoveOut();
//     public void OnDeselect(BaseEventData eventData) => MoveBack();

//     // --- ANIMATION LOGIC ---
//     private void MoveOut()
//     {
//         // DOKill stops any currently running tweens on this object so they don't fight
//         rectTransform.DOKill(); 
//         rectTransform.DOAnchorPosX(originalX + moveAmount, duration).SetEase(Ease.OutQuad);
//     }

//     private void MoveBack()
//     {
//         rectTransform.DOKill();
//         rectTransform.DOAnchorPosX(originalX, duration).SetEase(Ease.OutQuad);
//     }

//     void OnDisable()
//     {
//         // if the menu is turned off while hovered, snap it back to normal
//         rectTransform.DOKill();
//         rectTransform.anchoredPosition = new Vector2(originalX, rectTransform.anchoredPosition.y);
//     }
// }