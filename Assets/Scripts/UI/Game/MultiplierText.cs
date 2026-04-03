using UnityEngine;
using DG.Tweening;

public class MultiplierText : MonoBehaviour
{
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private float overshootAmount = 3f;

    public float AnimationDuration => animationDuration;

    public Tween AnimateTextIn()
    {
        transform.DOKill();
        transform.localScale = Vector3.zero;
        return transform.DOScale(1f, animationDuration).SetEase(Ease.OutBack, overshootAmount);
    }

    public void AnimateTextOut()
    {
        transform.DOKill();
        transform.DOScale(0f, animationDuration)
            .SetEase(Ease.InBack, overshootAmount)
            .OnComplete(DisableObject);
    }

    private void DisableObject()
    {
        gameObject.SetActive(false);
    }
}