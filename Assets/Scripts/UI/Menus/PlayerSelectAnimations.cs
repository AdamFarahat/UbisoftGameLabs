using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerSelectAnimations : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private UIAnimation AButtonAnim;
    [SerializeField] private UIAnimation BButtonAnim;
    
    [SerializeField] private PlayerSelectManager playerManager; 
    
    private UIAnimation[] uiElements;

    void Awake()
    {
        Assert.IsNotNull(canvas);
        Assert.IsNotNull(AButtonAnim);
        Assert.IsNotNull(BButtonAnim);
        Assert.IsNotNull(playerManager); 
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

    IEnumerator AnimateInSequence()
    {
        Sequence lastAnimation = null;

        foreach(UIAnimation elem in uiElements)
        {
            if (elem == AButtonAnim || elem == BButtonAnim) continue;
            
            // Store the sequence as it plays
            lastAnimation = elem.AnimateIn(); 
            yield return new WaitForSeconds(0.3f);
        }

        // Wait for the very last animation in the loop to completely finish its overshoot
        if (lastAnimation != null)
        {
            yield return lastAnimation.WaitForCompletion();
        }

        playerManager.EnableInput();
    }
}