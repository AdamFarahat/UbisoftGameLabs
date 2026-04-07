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

    [SerializeField] private float delayBeforeSelectingRestartButton = 1f;

    private Coroutine selectRestartButtonCoroutine;

    [SerializeField] private GameObject gunFireVFX;
    [SerializeField] private GameObject swordFireVFX;

    private void Awake()
    {
        Assert.IsNotNull(scoreManagerSO);
        Assert.IsNotNull(gunScore);
        Assert.IsNotNull(swordScore);
        Assert.IsNotNull(overallScoreText);
        Assert.IsNotNull(restartButton);
        Assert.IsNotNull(mainMenuButton);
        Assert.IsNotNull(eventSystem);

        Assert.IsNotNull(gunFireVFX);
        Assert.IsNotNull(swordFireVFX);

        restartButton.onClick.AddListener(Restart);
        mainMenuButton.onClick.AddListener(MainMenu);
    }

    private void Start()
    {
        gunFireVFX.SetActive(false);
        swordFireVFX.SetActive(false);
    }

    public void ShowGameOverScreen()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerDeath, Vector3.zero);
        AudioManager.Instance.PlayMusic(FMODEvents.Instance.OSTGameOver);
        gameOverFader.FadeToOpaque();
        gunScore.text = GunPlayerController.Instance.Score.ToString();
        swordScore.text = SwordPlayerController.Instance.Score.ToString();

        gunFireVFX.SetActive(true);
        swordFireVFX.SetActive(true);
    
        int score = ScoreManagerSO.CalculateOverallFinalTeamScore();
        overallScoreText.text = score.ToString();
        IEnumerator GameOverCoroutine()
        {
            yield return new WaitForSecondsRealtime(delayBeforeSelectingRestartButton);
            Debug.Log("Selecting Restart Button");
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
