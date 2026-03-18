using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.Assertions;
using System.Linq;

public class PlayerSelectAnimations : MonoBehaviour
{
    [SerializeField] private GameObject canvas;    
    
    [Header("Events")]
    public UnityEvent onIntroFinished; 
    public UnityEvent onOutroFinished;

    private UIAnimation[] uiElements;

    

    void Awake()
    {
        Assert.IsNotNull(canvas);
    }

    void Start()
    {
        uiElements = canvas.GetComponentsInChildren<UIAnimation>();
        foreach (UIAnimation elem in uiElements)
        {
            elem.PlaceOffScreen();
        }

        StartCoroutine(AnimateInSequence());        
    }

    // Wrapper for Inspector 
    public void TriggerAnimateOut()
    {
        StartCoroutine(AnimateOutSequence());
    }

    IEnumerator AnimateInSequence()
    {
        Sequence lastAnimation = null;

        foreach(UIAnimation elem in uiElements)
        {            
            lastAnimation = elem.AnimateIn(); 
            yield return new WaitForSeconds(0.2f);
        }

        if (lastAnimation != null)
        {
            yield return lastAnimation.WaitForCompletion();
        }

        // Fire event 
        onIntroFinished?.Invoke(); 
    }

    IEnumerator AnimateOutSequence()
    {
        Sequence lastAnimation = null;

        // Loop in reverse
        for (int i = uiElements.Length - 1; i >= 0; i--)
        {            
            lastAnimation = uiElements[i].AnimateOut(); 
            yield return new WaitForSeconds(0.1f);
        }

        if (lastAnimation != null)
        {
            yield return lastAnimation.WaitForCompletion();
        }

        // Fire event 
        onOutroFinished?.Invoke(); 
    }
}