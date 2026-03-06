using NUnit.Framework;
using UnityEngine;

public class PlayerSelectAnimations : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    private UIAnimation[] uiElements;

    void Awake()
    {
        Assert.IsNotNull(canvas);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiElements = canvas.GetComponentsInChildren<UIAnimation>();
        foreach (UIAnimation elem in uiElements)
        {
            elem.PlaceOffScreen();
            elem.AnimateIn();
        }


        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
