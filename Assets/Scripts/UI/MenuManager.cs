using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private float staggerDelay = 0.2f;
    [SerializeField] private UIAnimation title;
    private ButtonUIAnimation[] buttons;

    void Awake()
    {
        Assert.IsNotNull(menuCanvas);
        Assert.IsNotNull(title);
        buttons = menuCanvas.GetComponentsInChildren<ButtonUIAnimation>();

        title.PlaceOffScreen();

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

    // Animate the buttons in staggered fashion
    IEnumerator AnimateOutSequence()
    {
        // Clear selection when animating out so the player can't keep navigating
        EventSystem.current.SetSelectedGameObject(null);

        Sequence lastAnimation = null;


        for (int i = 0; i < buttons.Length; i++)
        {
            // If this is the button at index 0 (the selected one), give it the bonus!
            buttons[i].IsSelected = i == 0;

            lastAnimation = buttons[i].AnimateOut();
            yield return new WaitForSeconds(staggerDelay);
        }

        // Wait until the last button's sequence is finished
        if (lastAnimation != null)
        {
            yield return lastAnimation.WaitForCompletion();
        }

        title.AnimateOut();
    }

    // Animate the buttons in staggered fashion
    IEnumerator AnimateInSequence()
    {
        // Clear current selection so the controller does nothing during the intro
        EventSystem.current.SetSelectedGameObject(null);

        // Small delay before animating everything in
        yield return new WaitForSeconds(2f);
        
        Sequence titleAnimation = title.AnimateIn();

        yield return titleAnimation.WaitForCompletion();

        // Small delay before bringing in buttons
        yield return new WaitForSeconds(0.5f);

        Sequence lastAnimation = null;

        for (int i = 0; i < buttons.Length; i++)
        {
            // Disable button while animating in 
            Button unityButton = buttons[i].GetComponentInChildren<Button>();
            if (unityButton != null) unityButton.interactable = false;

            // Grab the last animation
            lastAnimation = buttons[i].AnimateIn();
            
            yield return new WaitForSeconds(staggerDelay);
        }

        // Wait until the last button's sequence is finished
        if (lastAnimation != null)
        {
            yield return lastAnimation.WaitForCompletion();
        }

        // Re-enable buttons
        for (int i = 0; i < buttons.Length; i++)
        {
            Button unityButton = buttons[i].GetComponentInChildren<Button>();
            if (unityButton != null) unityButton.interactable = true;
        }

        // Yield for one frame to let Unity's internal UI state update
        yield return null; 

        // Force Unity to select the Play button
        ButtonSelect firstButtonTarget = buttons[0].GetComponentInChildren<ButtonSelect>();
        if (firstButtonTarget != null)
        {
            EventSystem.current.SetSelectedGameObject(firstButtonTarget.gameObject);
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