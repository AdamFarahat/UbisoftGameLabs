using UnityEngine;
using DG.Tweening;

public class MultiplierText : MonoBehaviour
{
    [Header("Animation Settings")]
    public float animationDuration = 0.5f;

    // We removed OnEnable() and created a dedicated public method
    public void ShowText()
    {
        Debug.Log("ShowText is called on " + gameObject.name);
        transform.DOKill();
        transform.localScale = Vector3.zero;
        transform.DOScale(1f, animationDuration).SetEase(Ease.OutBack);
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
}