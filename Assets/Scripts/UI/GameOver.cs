using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class GameOver : MonoBehaviour
{
    [SerializeField] private ScoreManagerSO  scoreManagerSO;
    [SerializeField] private TextMeshProUGUI gunScore;
    [SerializeField] private TextMeshProUGUI swordScore;
    [SerializeField] private TextMeshProUGUI overallScoreText;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        Assert.IsNotNull(scoreManagerSO);
        Assert.IsNotNull(gunScore);
        Assert.IsNotNull(swordScore);
        Assert.IsNotNull(overallScoreText);
        Assert.IsNotNull(restartButton);

        restartButton.onClick.AddListener(Restart);
    }

    public void ShowGameOverScreen()
    {
        gunScore.text = GunPlayerController.Instance.Score.ToString();
        swordScore.text = SwordPlayerController.Instance.Score.ToString();
    
        int score = ScoreManagerSO.CalculateOverallFinalTeamScore();
        overallScoreText.text = score.ToString(); 
    }

    public void Restart(){
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    
    }
}
