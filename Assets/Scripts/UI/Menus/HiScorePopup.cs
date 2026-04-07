using UnityEngine;
using DG.Tweening;

public class HiScorePopup : MonoBehaviour
{
    [SerializeField] private MultiplierText gunHiScoreText;
    [SerializeField] private MultiplierText swordHiScoreText;

    [Header("Idle Bounce Animation")]
    [SerializeField] private float bounceScale = 1.15f;
    [SerializeField] private float bounceDuration = 0.6f;

    void OnEnable()
    {
        // Create sequences and ignore timeScale entirely
        Sequence seqGun = DOTween.Sequence().SetUpdate(true);
        Sequence seqSword = DOTween.Sequence().SetUpdate(true);

        // Append the intro animations
        seqGun.Append(gunHiScoreText.AnimateTextIn());
        seqSword.Append(swordHiScoreText.AnimateTextIn());

        // Append the bounce 
        seqGun.Append(gunHiScoreText.transform.DOScale(bounceScale, bounceDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo));
        
        seqSword.Append(swordHiScoreText.transform.DOScale(bounceScale, bounceDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo));
    }
}