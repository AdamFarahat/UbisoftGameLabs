using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "ScoreManagerSO", menuName = "Scriptable Objects/ScoreManagerSO")]
public class ScoreManagerSO : ScriptableObject
{
    public float TEAM_MULTIPLIER_BASE = 20f;

    private float MIN_MULTIPLIER = 1f;

    private float MAX_MulTIPLIER = 0.00001f;
    
    private const int numberOfPlayers = 2;
    [SerializeField]
    private float SumOfSTDDevTreshold = 0.001f;
    private static ScoreManagerSO _instance;
    public static ScoreManagerSO Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<ScoreManagerSO>("ScoreManagerSO");
                Assert.IsNotNull(_instance, "ScoreManagerSO not found in Resources!");
            }

            return _instance;
        }
    }

    public static int CalculateOverallTeamScore()
    {
        int totalScore = 0;
        if (GunPlayerController.Instance != null)
            totalScore += GunPlayerController.Instance.Score;
        if (SwordPlayerController.Instance != null)
            totalScore += SwordPlayerController.Instance.Score;
        return totalScore;
    }
    /// <summary>
    /// calculates the total score based on how close both players are to the average in order to reward them for team play.
    /// For now, we do not store the score somewhere the calculation cannot be done outside of the level.
    /// </summary>
    /// <returns>Final score that should be displayed</returns>
    public static int CalculateOverallFinalTeamScore()
    {
        float totalScore = CalculateOverallTeamScore();

        Debug.Log("TotalScore: " + totalScore);
        
        float average = totalScore / (float)(numberOfPlayers);
        
        float sumOfStdDev = Mathf.Abs(GunPlayerController.Instance.Score - average)
            + Mathf.Abs(SwordPlayerController.Instance.Score - average);

        float teamMultiplier = sumOfStdDev <= _instance.SumOfSTDDevTreshold ?
_instance.MIN_MULTIPLIER + _instance.TEAM_MULTIPLIER_BASE / _instance.SumOfSTDDevTreshold
              : _instance.MIN_MULTIPLIER + _instance.TEAM_MULTIPLIER_BASE / (sumOfStdDev);
        Debug.Log("return value: " + (int)(teamMultiplier * totalScore));
        return (int)(teamMultiplier * totalScore);
    }
}
