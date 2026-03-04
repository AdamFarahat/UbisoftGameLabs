using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;

public class PlayerSelectAnimations : MonoBehaviour
{
    private RectTransform rectTransform;
    private float originalY;
    [SerializeField] private float anticipationOffsetY = 60.0f;

    // Exit settings
    [SerializeField] private float exitPositionY = -500.0f; 
    [SerializeField] private float anticipationDuration = 0.2f;
    [SerializeField] private float exitDuration = 0.3f;

    public float ExitPositionY { get => exitPositionY; set => exitPositionY = value; }
    public float AnticipationDuration { get => anticipationDuration; set => anticipationDuration = value; }
    public float ExitDuration { get => exitDuration; set => exitDuration = value; }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalY = rectTransform.anchoredPosition.y; 
    }

    void Start()
    {
        StartCoroutine(waitSec());
    }
    // --- ANIMATION LOGIC --

    void OnDisable()
    {
        // Reset position 
        rectTransform.DOKill();
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.y, originalY);
    }

    public void AnimateOffScreen()
    {
        rectTransform.DOKill();
        Sequence offScreenSeq = DOTween.Sequence();

        // ANTICIPATION: Move forward a bit + any extra push for being selected
        float totalPush = originalY + anticipationOffsetY;
        offScreenSeq.Append(rectTransform.DOAnchorPosY(totalPush, anticipationDuration).SetEase(Ease.OutQuad));

        // EXIT: Move completely off screen.
        offScreenSeq.Append(rectTransform.DOAnchorPosY(exitPositionY, exitDuration).SetEase(Ease.InQuad));
    }


    IEnumerator waitSec()
    {
        yield return new WaitForSeconds(5f);
        AnimateOffScreen();

    }
}