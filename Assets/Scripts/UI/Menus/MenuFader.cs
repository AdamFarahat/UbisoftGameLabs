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
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f; // Start invisible
        }
    }

    public void FadeToOpaque(Action onComplete = null)
    {
        gameObject.SetActive(true);

        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f; 
        }

        canvasGroup.DOKill();
        canvasGroup.interactable = true;
        // Animate even when timescale is set to 0
        canvasGroup.DOFade(1f, fadeDuration).SetUpdate(true).OnComplete(() => 
        {
            onComplete?.Invoke();
        });
    }

    public void FadeToTransparent(Action onComplete = null) 
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        canvasGroup.DOKill();
        canvasGroup.interactable = false;
    
        canvasGroup.alpha = 1f;
        canvasGroup.DOFade(0f, fadeDuration).SetUpdate(true).OnComplete(() => 
        {
            // gameObject.SetActive(false);
            onComplete?.Invoke(); 
        });
    }
}