using UnityEngine;
using TMPro; 
using DG.Tweening;

public class ScoreRoller : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gunScoreText;
    [SerializeField] private TextMeshProUGUI swordScoreText;
    [SerializeField] private float rollDuration = 2f;

    private GunPlayerController gunPlayer;
    private SwordPlayerController swordPlayer;

    private int displayedScore = 0;

    void OnEnable()
    {
        gunPlayer = GunPlayerController.Instance;
        swordPlayer = SwordPlayerController.Instance;

        int gunScore = gunPlayer.Score;
        AnimateScore(gunScore, gunScoreText);

        int swordScore = swordPlayer.Score;
        AnimateScore(swordScore, swordScoreText);
    }
    void AnimateScore(int finalScore, TextMeshProUGUI scoreText)
    {
        // Reset state
        scoreText.transform.DOKill();
        displayedScore = 0;
        scoreText.text = "0";

        // Animate the integer from 0 to the finalScore
        DOTween.To(() => displayedScore, x => 
        {
            displayedScore = x;
            scoreText.text = displayedScore.ToString("N0"); // N0 adds commas
            
        }, finalScore, rollDuration)
        
        .SetEase(Ease.OutExpo)
        .SetUpdate(true) 
        
        // Add extra pop at end
        .OnComplete(() => 
        {
            scoreText.transform.DOPunchScale(Vector3.one * 0.3f, 0.4f, 10, 1).SetUpdate(true);
        });
    }
}