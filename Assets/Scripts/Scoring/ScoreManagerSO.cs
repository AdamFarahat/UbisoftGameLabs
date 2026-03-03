using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "ScoreManagerSO", menuName = "Scriptable Objects/ScoreManagerSO")]
public class ScoreManagerSO : ScriptableObject
{
    public float TEAM_MULTIPLIER_BASE = 20f;

    private const int numberOfPlayers = 2;

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
    public static float CalculateOverallTeamScore()
    {
        float totalScore = GunPlayerController.Instance.Score + SwordPlayerController.Instance.Score;
        float average = totalScore / (float)(numberOfPlayers);
        float teamMultiplier = _instance.TEAM_MULTIPLIER_BASE / (Mathf.Abs(GunPlayerController.Instance.Score - average)
            + Mathf.Abs(SwordPlayerController.Instance.Score - average));
        return teamMultiplier * totalScore;
    }
}
