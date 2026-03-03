using UnityEngine;
using DG.Tweening;
using System.Collections; // You need this to use IEnumerator/Coroutines

public class MultiplierText : MonoBehaviour
{
    [Header("Animation Settings")]
    public float animationDuration = 0.5f;
    public float displayTime = 3f;

    void OnEnable()
    {
        transform.DOKill();
        transform.localScale = Vector3.zero;
        transform.DOScale(1f, animationDuration).SetEase(Ease.OutBack);

        // Start the temporary countdown the moment the text pops in
        StartCoroutine(HideAfterDelay());
    }

    public void HideText()
    {
        transform.DOKill();
        transform.DOScale(0f, animationDuration)
            .SetEase(Ease.InBack)
            .OnComplete(DisableObject);
    }

    private void DisableObject()
    {
        gameObject.SetActive(false);
    }

    // Temporary Coroutine
    private IEnumerator HideAfterDelay()
    {
        // Wait for exactly 3 seconds (or whatever you set displayTime to)
        yield return new WaitForSeconds(displayTime);
        
        // Trigger the shrinking animation
        HideText();
    }
}