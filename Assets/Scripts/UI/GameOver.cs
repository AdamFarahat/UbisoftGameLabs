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

    [SerializeField] private EventSystem eventSystem;

    [SerializeField] private float timebeforeMenuAccessible = 1f;

    private void Awake()
    {
        Assert.IsNotNull(scoreManagerSO);
        Assert.IsNotNull(gunScore);
        Assert.IsNotNull(swordScore);
        Assert.IsNotNull(overallScoreText);
        Assert.IsNotNull(restartButton);
        Assert.IsNotNull(mainMenuButton);
        Assert.IsNotNull(eventSystem);

        restartButton.onClick.AddListener(Restart);
        mainMenuButton.onClick.AddListener(MainMenu);
    }

    public void ShowGameOverScreen()
    {
        gameOverFader.FadeToOpaque();
        gunScore.text = GunPlayerController.Instance.Score.ToString();
        swordScore.text = SwordPlayerController.Instance.Score.ToString();
    
        int score = ScoreManagerSO.CalculateOverallFinalTeamScore();
        overallScoreText.text = score.ToString();

        IEnumerator setSelectedButton()
        {
            // Wait for the end of the frame to ensure the UI is fully active
            yield return new WaitForSecondsRealtime(timebeforeMenuAccessible);
            eventSystem.SetSelectedGameObject(restartButton.gameObject);
        }

        StartCoroutine(setSelectedButton());
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
