using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectAnimations : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private UIAnimation AButtonAnim;
    [SerializeField] private UIAnimation BButtonAnim;
    private UIAnimation[] uiElements;


    void Awake()
    {
        Assert.IsNotNull(canvas);
        Assert.IsNotNull(AButtonAnim);
        Assert.IsNotNull(BButtonAnim);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        foreach(UIAnimation elem in uiElements)
        {
            if (elem == AButtonAnim || elem == BButtonAnim) continue;
            elem.AnimateIn();
            yield return new WaitForSeconds(0.3f);
        }
    }
}
