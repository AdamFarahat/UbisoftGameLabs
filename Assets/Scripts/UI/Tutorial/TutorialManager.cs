using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [Header("Special Tutorials")]
    [SerializeField] private TutorialSwitchLanes tutorialSwitchLanes;
    [SerializeField] private TutorialPrimaryAction tutorialPrimaryAction;
    [SerializeField] private TutorialSecondaryAction tutorialSecondaryAction;
    [SerializeField] private TutorialSwitchGuns tutorialSwitchGuns;
    [SerializeField] private TutorialJump tutorialJump;

    [Header("UI Elements")]
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
        Assert.IsNotNull(tutorialSwitchLanes);
        Assert.IsNotNull(tutorialPrimaryAction);
        Assert.IsNotNull(tutorialSecondaryAction);
        Assert.IsNotNull(tutorialSwitchGuns);
        Assert.IsNotNull(tutorialJump);

        Assert.IsNotNull(gunPlayerCooldownUI);
        Assert.IsNotNull(swordPlayerCooldownUI);
        Assert.IsNotNull(scoreUI);
        Assert.IsNotNull(gunPlayerMultiplierUI);
        Assert.IsNotNull(swordPlayerMultiplierUI);

        tutorials = GetComponentsInChildren<TutorialBase>();
    }

    private void Start()
    {
        if (GunPlayerController.Instance != null)
        {
            GunPlayerController.Instance.StartButtonPressed += OnStartButtonPressed;
            GunPlayerController.Instance.moveEnabled = !tutorialSwitchLanes.isActiveAndEnabled;
            GunPlayerController.Instance.shootEnabled = !tutorialPrimaryAction.isActiveAndEnabled;
            GunPlayerController.Instance.throwEnabled = !tutorialSecondaryAction.isActiveAndEnabled;
            GunPlayerController.Instance.toggleGunEnabled = !tutorialSwitchGuns.isActiveAndEnabled;
        }

        if (SwordPlayerController.Instance != null)
        {
            SwordPlayerController.Instance.StartButtonPressed += OnStartButtonPressed;
            SwordPlayerController.Instance.moveEnabled = !tutorialSwitchLanes.isActiveAndEnabled;
            SwordPlayerController.Instance.slashEnabled = !tutorialPrimaryAction.isActiveAndEnabled;
            SwordPlayerController.Instance.blockEnabled = !tutorialSecondaryAction.isActiveAndEnabled;
            SwordPlayerController.Instance.jumpEnabled = !tutorialJump.isActiveAndEnabled;
        }

        PlayerStats.Instance.superEnabled = false;
        PlayerStats.Instance.ResetGunSuper();
        PlayerStats.Instance.ResetSwordSuper();
        PlayerStats.Instance.damageEnabled = false;

        gunPlayerCooldownUI.SetActive(false);
        swordPlayerCooldownUI.SetActive(false);
        scoreUI.SetActive(false);
        gunPlayerMultiplierUI.SetActive(false);
        swordPlayerMultiplierUI.SetActive(false);

        foreach (TutorialBase tutorial in tutorials)
            tutorial.gameObject.SetActive(false);

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
