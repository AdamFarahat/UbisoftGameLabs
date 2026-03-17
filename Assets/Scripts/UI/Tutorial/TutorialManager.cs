using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject gunPlayerCooldownUI;
    public GameObject GunPlayerCooldownUI => gunPlayerCooldownUI;

    [SerializeField] private GameObject swordPlayerCooldownUI;
    public GameObject SwordPlayerCooldownUI => swordPlayerCooldownUI;

    [SerializeField] private GameObject scoreUI;
    public GameObject ScoreUI => scoreUI;

    [SerializeField] private GameObject gunPlayerMultiplierUI;
    public GameObject GunPlayerMultiplierUI => gunPlayerMultiplierUI;

    [SerializeField] private GameObject swordPlayerMultiplierUI;
    public GameObject SwordPlayerMultiplierUI => swordPlayerMultiplierUI;

    private TutorialBase[] tutorials;
    private int tutorialIndex = -1;

    private void Awake()
    {
        Assert.IsNotNull(gunPlayerCooldownUI);
        Assert.IsNotNull(swordPlayerCooldownUI);
        Assert.IsNotNull(scoreUI);
        Assert.IsNotNull(gunPlayerMultiplierUI);
        Assert.IsNotNull(swordPlayerMultiplierUI);

        tutorials = GetComponentsInChildren<TutorialBase>();
        foreach (TutorialBase tutorial in tutorials)
            tutorial.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (GunPlayerController.Instance != null)
        {
            GunPlayerController.Instance.StartButtonPressed += OnStartButtonPressed;
            GunPlayerController.Instance.moveEnabled = false;
            GunPlayerController.Instance.shootEnabled = false;
            GunPlayerController.Instance.throwEnabled = false;
            GunPlayerController.Instance.toggleGunEnabled = false;
        }

        if (SwordPlayerController.Instance != null)
        {
            SwordPlayerController.Instance.StartButtonPressed += OnStartButtonPressed;
            SwordPlayerController.Instance.moveEnabled = false;
            SwordPlayerController.Instance.slashEnabled = false;
            SwordPlayerController.Instance.blockEnabled = false;
            SwordPlayerController.Instance.jumpEnabled = false;
        }

        PlayerStats.Instance.superEnabled = false;
        PlayerStats.Instance.ResetGunSuper();
        PlayerStats.Instance.ResetSwordSuper();

        gunPlayerCooldownUI.SetActive(false);
        swordPlayerCooldownUI.SetActive(false);
        scoreUI.SetActive(false);
        gunPlayerMultiplierUI.SetActive(false);
        swordPlayerMultiplierUI.SetActive(false);

        NextTutorial();
    }

    private void OnStartButtonPressed()
    {
        ExitTutorial();
    }

    public void NextTutorial()
    {
        tutorialIndex++;
        if (tutorialIndex < tutorials.Length)
            tutorials[tutorialIndex].DoTutorial();
    }

    public void ExitTutorial()
    {
        SceneManager.LoadScene("Menu");
    }
}
