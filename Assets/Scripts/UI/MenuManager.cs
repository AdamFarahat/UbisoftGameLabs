using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject menuCanvas;
    
    [Header("Hover Settings")]
    [SerializeField] private float hoverDuration = 0.2f;
    [SerializeField] private float hoverOffsetX = 20f; 

    [Header("Animate Out Settings")]
    [SerializeField] private float anticipationDuration = 0.2f;
    [SerializeField] private float exitDuration = 0.3f;
    [SerializeField] private float exitPositionX = -1000f; 
    [Tooltip("The extra space the selected button will move before animating off screen")]
    [SerializeField] private float selectedBonusX = 30f; 
    [SerializeField] private float staggerDelay = 0.2f;

    private UIButtonHoverEffect[] buttons;

    void Awake()
    {
        Assert.IsNotNull(menuCanvas);
        buttons = menuCanvas.GetComponentsInChildren<UIButtonHoverEffect>();
    }

    void Start()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].HoverOffsetX = hoverOffsetX;
            buttons[i].HoverDuration = hoverDuration;
            buttons[i].ExitPositionX = exitPositionX;
            buttons[i].AnticipationDuration = anticipationDuration;
            buttons[i].ExitDuration = exitDuration;
        }
    }

    public void AnimateButtonsOut()
    {
        StartCoroutine(AnimateSequence());
    }

    IEnumerator AnimateSequence()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            // If this is the button at index 0 (the selected one), give it the bonus!
            // Otherwise, give it 0 extra push.
            float bonus = (i == 0) ? selectedBonusX : 0f;
            
            buttons[i].AnimateOffScreen(bonus);
            yield return new WaitForSeconds(staggerDelay);
        }
    }

    public void ReorderButtons(UIButtonHoverEffect button)
    {
        int index = 0;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (button == buttons[i])
            {
                index = i;
                break; // We found it, stop looking
            }
        }

        // Shift the array to bring the clicked button to index 0
        if (index != 0)
        {
            UIButtonHoverEffect selected = buttons[index];
            for (int i = index; i > 0; i--)
            {
                buttons[i] = buttons[i - 1];
            }
            buttons[0] = selected;
        }
    }
}