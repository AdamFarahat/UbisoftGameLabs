using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "ScoreManagerSO", menuName = "Scriptable Objects/ScoreManagerSO")]
public class ScoreManagerSO : ScriptableObject
{
    public float TEAM_MULTIPLIER_BASE = 20f;
    
    private const int numberOfPlayers = 2;
    [SerializeField]
    private float MaxTeamBasedMultiplier = 900.0f;
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
    /// <summary>
    /// calculates the total score based on how close both players are to the average in order to reward them for team play.
    /// For now, we do not store the score somewhere the calculation cannot be done outside of the level.
    /// </summary>
    /// <returns>Final score that should be displayed</returns>
    public static int CalculateOverallTeamScore()
    {
        float totalScore = GunPlayerController.Instance.Score + SwordPlayerController.Instance.Score;
        
        float average = totalScore / (float)(numberOfPlayers);
        
        float sumOfStdDev = Mathf.Abs(GunPlayerController.Instance.Score - average)
            + Mathf.Abs(SwordPlayerController.Instance.Score - average);

        float teamMultiplier = sumOfStdDev == 0 ? 0 
            : sumOfStdDev <= _instance.SumOfSTDDevTreshold ? 
                _instance.MaxTeamBasedMultiplier 
              : _instance.TEAM_MULTIPLIER_BASE / (sumOfStdDev);

        return (int)(teamMultiplier * totalScore);
    }
}
