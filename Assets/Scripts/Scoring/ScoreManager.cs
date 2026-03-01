using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class ScoreManager : MonoBehaviour
{ 
    public const float MULTIPLER_GAIN = 2f;
    public const float BLOCKING_GAIN = 1f;
    public const float PARRIED_DEFAULT_SCORE = 10f;
    public const float PARRIED_MULTIPLER_FACTOR = 3f;

    public const float GRENADE_MULTIPLIER = 2f;
    public const float DEFAULT_BULLET_MULTIPLIER = 4f;
    public const float SHOTGUN_BLAST_MULTIPLIER = 7f;
    private const float BASE = 20f;
    
    private const int numberOfPlayers = 2;
    
    public GunPlayerController gunPlayerController;
    public SwordPlayerController swordPlayerController;


    /// <summary>
    /// calculates the total score based on how close both players are to the average in order to reward them for team play.
    /// </summary>
    /// <returns>Final score that should be displayed</returns>
    public float calculateOverallTeamScore() {
        float totalScore = gunPlayerController.score + swordPlayerController.score;
        float average = totalScore / (float) (numberOfPlayers);
        float teamMultiplier = BASE / (Mathf.Abs(gunPlayerController.score - average) + Mathf.Abs(swordPlayerController.score - average));
        return teamMultiplier * totalScore;
    }
}
