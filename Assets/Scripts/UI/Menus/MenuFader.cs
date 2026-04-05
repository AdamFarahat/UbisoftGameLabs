using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class MenuFader : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1.0f;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        // Start invisible        
        canvasGroup.alpha = 0f; 
    }

    public void FadeIn(Action onComplete = null)
    {
        canvasGroup.DOKill();
        gameObject.SetActive(true);
        canvasGroup.interactable = true;
        // Animate even when timescale is set to 0
        canvasGroup.DOFade(1f, fadeDuration).SetUpdate(true).OnComplete(() => 
        {
            onComplete?.Invoke();
        });
    }

    public void FadeOut(Action onComplete = null) 
    {
        canvasGroup.DOKill();
        canvasGroup.interactable = false;
        
        canvasGroup.DOFade(0f, fadeDuration).SetUpdate(true).OnComplete(() => 
        {
            gameObject.SetActive(false);
            onComplete?.Invoke(); 
        });
    }
}