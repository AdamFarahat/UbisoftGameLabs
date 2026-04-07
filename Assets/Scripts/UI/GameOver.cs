using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using System.Collections;

public class GameOver : MonoBehaviour
{
    [SerializeField] private ScoreManagerSO  scoreManagerSO;
    [SerializeField] private TextMeshProUGUI gunScore;
    [SerializeField] private TextMeshProUGUI swordScore;
    [SerializeField] private TextMeshProUGUI overallScoreText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private MenuFader gameOverFader;
    [SerializeField] private MenuFader blackScreenFader;


    [SerializeField] private GameObject HighScoreTeam;
    [SerializeField] private GameObject HighScoreGun;
    [SerializeField] private GameObject HighScoreSword;

    [SerializeField] private EventSystem eventSystem;

    [SerializeField] private float delayBeforeSelectingRestartButton = 1f;

    private Coroutine selectRestartButtonCoroutine;

    private void Awake()
    {
        Assert.IsNotNull(scoreManagerSO);
        Assert.IsNotNull(gunScore);
        Assert.IsNotNull(swordScore);
        Assert.IsNotNull(overallScoreText);
        Assert.IsNotNull(restartButton);
        Assert.IsNotNull(mainMenuButton);
        Assert.IsNotNull(eventSystem);

        Assert.IsNotNull(HighScoreTeam);
        Assert.IsNotNull(HighScoreGun); 
        Assert.IsNotNull(HighScoreSword);

        restartButton.onClick.AddListener(Restart);
        mainMenuButton.onClick.AddListener(MainMenu);
    }

    public void ShowGameOverScreen()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerDeath, Vector3.zero);
        AudioManager.Instance.PlayMusic(FMODEvents.Instance.OSTGameOver);
        gameOverFader.FadeToOpaque();
        gunScore.text = GunPlayerController.Instance.Score.ToString();
        swordScore.text = SwordPlayerController.Instance.Score.ToString();

        int score = ScoreManagerSO.CalculateOverallFinalTeamScore(out bool isHighScoreTeam, out bool isHighScoreGun, out bool isHighScoreSword);
        if(isHighScoreGun)
        {
            HighScoreGun.SetActive(true);
        }
        if(isHighScoreSword)
        {
            HighScoreSword.SetActive(true);
        }
        if(isHighScoreTeam)
        {
            HighScoreTeam.SetActive(true);
        }


        overallScoreText.text = score.ToString();
        

        IEnumerator GameOverCoroutine()
        {
            yield return new WaitForSecondsRealtime(delayBeforeSelectingRestartButton);
            eventSystem.SetSelectedGameObject(restartButton.gameObject);
            selectRestartButtonCoroutine = null; 
        }
        selectRestartButtonCoroutine = StartCoroutine(GameOverCoroutine());
    }

    public void Restart(){
        Time.timeScale = 1f;
        blackScreenFader.FadeToOpaque(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        });
    }

    public void MainMenu(){
        Time.timeScale = 1f;
        blackScreenFader.FadeToOpaque(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        });
    }
}
