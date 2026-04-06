using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenuFader : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [SerializeField] private readonly float fadeDuration = 0.3f;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        // Start invisible        
        canvasGroup.alpha = 0f; 
    }

    public void FadeIn()
    {
        gameObject.SetActive(true);
        canvasGroup.interactable = true;
        
        // Animate even when timescale is set to 0
        canvasGroup.DOFade(1f, fadeDuration).SetUpdate(true);
    }

    public void FadeOut()
    {
        canvasGroup.interactable = false;
        
        canvasGroup.DOFade(0f, fadeDuration).SetUpdate(true).OnComplete(() => 
        {
            gameObject.SetActive(false);
        });
    }
}