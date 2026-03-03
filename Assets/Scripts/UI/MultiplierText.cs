using UnityEngine;
using DG.Tweening;

public class MultiplierText : MonoBehaviour
{
    [SerializeField] private float animationDuration = 0.5f;

    public void AnimateTextIn()
    {
        transform.DOKill();
        transform.localScale = Vector3.zero;
        transform.DOScale(1f, animationDuration).SetEase(Ease.OutBack);
    }

    public void AnimateTextOut()
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