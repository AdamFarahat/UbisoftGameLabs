using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreManagerSO", menuName = "Scriptable Objects/ScoreManagerSO")]
public class ScoreManagerSO : ScriptableObject
{
    public float MULTIPLER_GAIN = 2f;
    public float BLOCKING_GAIN = 1f;
    public int PARRIED_DEFAULT_SCORE = 10;
    public float PARRIED_MULTIPLER_FACTOR = 3f;

    public float GRENADE_MULTIPLIER = 2f;
    public float DEFAULT_BULLET_MULTIPLIER = 4f;
    public float SHOTGUN_BLAST_MULTIPLIER = 7f;
    private float BASE = 20f;

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
        float totalScore = GunPlayerController.Instance.score + SwordPlayerController.Instance.score;
        float average = totalScore / (float)(numberOfPlayers);
        float teamMultiplier = _instance.BASE / (Mathf.Abs(GunPlayerController.Instance.score - average)
            + Mathf.Abs(SwordPlayerController.Instance.score - average));
        return teamMultiplier * totalScore;
    }
}
