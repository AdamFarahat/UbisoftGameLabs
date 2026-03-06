using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private float staggerDelay = 0.2f;
    private ButtonUIAnimation[] buttons;

    void Awake()
    {
        Assert.IsNotNull(menuCanvas);
        buttons = menuCanvas.GetComponentsInChildren<ButtonUIAnimation>();

        foreach (ButtonUIAnimation button in buttons)
        {
            button.PlaceOffScreen();
        }
    }

    void Start()
    {
        StartCoroutine(AnimateInSequence());
    }

    public void AnimateButtonsOut()
    {
        StartCoroutine(AnimateOutSequence());
    }

    IEnumerator AnimateOutSequence()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            // If this is the button at index 0 (the selected one), give it the bonus!
            buttons[i].IsSelected = i == 0;

            buttons[i].AnimateOut();
            yield return new WaitForSeconds(staggerDelay);
        }
    }

    IEnumerator AnimateInSequence()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].AnimateIn();
            yield return new WaitForSeconds(staggerDelay);
        }
    }

    public void ReorderButtons(ButtonUIAnimation button)
    {
        int index = 0;

        for (int i = 0; i < buttons.Length; i++)
        {
            // Find selected button
            if (button == buttons[i])
            {
                index = i;
                break;
            }
        }

        // Shift the array to bring the clicked button to index 0
        if (index != 0)
        {
            ButtonUIAnimation selected = buttons[index];
            for (int i = index; i > 0; i--)
            {
                buttons[i] = buttons[i - 1];
            }
            buttons[0] = selected;
        }
    }
}