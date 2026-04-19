using System;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "ScoreManagerSO", menuName = "Scriptable Objects/ScoreManagerSO")]
public class ScoreManagerSO : ScriptableObject
{
    private const string HIGH_SCORE_TEAM = "HIGH_SCORE_TEAM";
    private const string HIGH_SCORE_GUN_PLAYER = "HIGH_SCORE_GUN_PLAYER";
    private const string HIGH_SCORE_SWORD_PLAYER = "HIGH_SCORE_GUN_SWORD_PLAYER";

    public float TEAM_MULTIPLIER_BASE = 20f;

    private readonly float MIN_MULTIPLIER = 1f;

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
    public static int CalculateOverallFinalTeamScore(out bool isHighScoreTeam, out bool isHighScoreGun, out bool isHighScoreSword)
    {
        isHighScoreTeam = false;
        isHighScoreGun = false;
        isHighScoreSword = false;
        float totalScore = CalculateOverallTeamScore();

        
        
        float average = totalScore / (float)(numberOfPlayers);
        
        float sumOfStdDev = Mathf.Abs(GunPlayerController.Instance.Score - average)
            + Mathf.Abs(SwordPlayerController.Instance.Score - average);

        float teamMultiplier = sumOfStdDev <= _instance.SumOfSTDDevTreshold ?
            _instance.MIN_MULTIPLIER + _instance.TEAM_MULTIPLIER_BASE / _instance.SumOfSTDDevTreshold
              : _instance.MIN_MULTIPLIER + _instance.TEAM_MULTIPLIER_BASE / (sumOfStdDev);
        int finalScore = (int)(teamMultiplier * totalScore);


        if (finalScore >= GetHighScoreTeam()) {
            SaveHighScoreTeam(finalScore);
            isHighScoreTeam = true;
        }
        if (GunPlayerController.Instance.Score >= GetHighScoreGunPlayer()) { 
            SaveHighScoreGun(GunPlayerController.Instance.Score);
            isHighScoreGun = true;

        } 
        if (SwordPlayerController.Instance.Score >= GetHighScoreSwordPlayer()) {
            SaveHighScoreSword(SwordPlayerController.Instance.Score);
            isHighScoreSword = true;

        }
        return finalScore;
    }

    

    public static int GetHighScoreTeam()
    {
        return PlayerPrefs.GetInt(HIGH_SCORE_TEAM, 0);
    }
    public static int GetHighScoreGunPlayer()
    {
        return PlayerPrefs.GetInt(HIGH_SCORE_GUN_PLAYER, 0);
    }
    public static int GetHighScoreSwordPlayer()
    {
        return PlayerPrefs.GetInt(HIGH_SCORE_SWORD_PLAYER, 0);
    }

    private static void SaveHighScoreTeam(int score)
    {
        PlayerPrefs.SetInt(HIGH_SCORE_TEAM, score);
        PlayerPrefs.Save();
    }

    private static void SaveHighScoreGun(int score)
    {
        PlayerPrefs.SetInt(HIGH_SCORE_GUN_PLAYER, score);
        PlayerPrefs.Save();
    }

    private static void SaveHighScoreSword(int score)
    {
        PlayerPrefs.SetInt(HIGH_SCORE_SWORD_PLAYER, score);
        PlayerPrefs.Save();
    }


}
