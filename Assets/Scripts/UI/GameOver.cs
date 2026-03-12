using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    private ScoreManagerSO  scoreManagerSO;
    private TextMeshProUGUI gunScore;
    private TextMeshProUGUI swordScore;
    private TextMeshProUGUI overallScoreText;
    private Button restartButton;

    private void Awake()
    {
        scoreManagerSO = ScoreManagerSO.Instance;
        if (scoreManagerSO == null)
            Debug.LogWarning("ScoreManagerSO instance not found. Score UI will not be updated.");
        overallScoreText = GameObject.Find("Overall Score").GetComponent<TextMeshProUGUI>();
        gunScore = GameObject.Find("GunScore").GetComponent<TextMeshProUGUI>();
        swordScore = GameObject.Find("SwordScore").GetComponent<TextMeshProUGUI>();
        restartButton = GameObject.Find("Restart").GetComponent<Button>();
        restartButton.onClick.AddListener(Restart);
    }

    public void ShowGameOverScreen()
    {
        if (gunScore != null)
            gunScore.text = GunPlayerController.Instance.Score.ToString();
        if (swordScore != null)
            swordScore.text = SwordPlayerController.Instance.Score.ToString();
        if (scoreManagerSO != null)
        {
            int score = ScoreManagerSO.CalculateOverallFinalTeamScore();
            overallScoreText.text = score.ToString();
        }
        gameObject.SetActive(true); 
    }

    public void Restart(){
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    
    }
}
