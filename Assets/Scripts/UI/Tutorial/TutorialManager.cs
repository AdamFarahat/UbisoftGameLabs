using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Fade Out")]
    [SerializeField] private float fadeOutDuration = 2f;
    [SerializeField] private RawImage fadeOutImage;
    private bool fadingOut = false;

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
    [SerializeField] private GameObject[] disabledLanes;
    public GameObject[] DisabledLanes => disabledLanes;

    private TutorialBase[] tutorials;
    private int tutorialIndex = -1;

    [SerializeField] private DifficultyMenu difficultyMenu;

    private void Awake()
    {
        Assert.IsNotNull(fadeOutImage);

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

        Assert.IsNotNull(difficultyMenu);

        tutorials = GetComponentsInChildren<TutorialBase>();
    }

    private void Start()
    {
        fadeOutImage.color = Color.clear;

        if (GunPlayerController.Instance != null)
        {
            GunPlayerController.Instance.StartButtonPressed += OnStartButtonPressed;
            GunPlayerController.Instance.SelectButtonPressed += ExitTutorial;
            GunPlayerController.Instance.moveEnabled = !tutorialSwitchLanes.isActiveAndEnabled;
            GunPlayerController.Instance.shootEnabled = !tutorialPrimaryAction.isActiveAndEnabled;
            GunPlayerController.Instance.throwEnabled = !tutorialSecondaryAction.isActiveAndEnabled;
            GunPlayerController.Instance.toggleGunEnabled = !tutorialSwitchGuns.isActiveAndEnabled;
        }

        if (SwordPlayerController.Instance != null)
        {
            SwordPlayerController.Instance.StartButtonPressed += OnStartButtonPressed;
            SwordPlayerController.Instance.SelectButtonPressed += ExitTutorial;
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

        foreach (GameObject lane in disabledLanes)
            lane.SetActive(!tutorialSwitchLanes.isActiveAndEnabled);

        foreach (TutorialBase tutorial in tutorials)
            tutorial.gameObject.SetActive(false);

        NextTutorial();
    }

    public void NextTutorial()
    {
        tutorialIndex++;
        if (tutorialIndex < tutorials.Length)
            tutorials[tutorialIndex].DoTutorial();
        else
            SetDifficulty();
    }

    public void ExitTutorial()
    {
        ExitScene("Menu");
    }

    private void OnStartButtonPressed()
    {
        tutorials[tutorialIndex].OnStartPressed();
    }

    private void SetDifficulty()
    {
        difficultyMenu.ShowDifficultyMenu();
    }

    public void StartGame()
    {
        ExitScene("Game");
    }

    private void ExitScene(string sceneName)
    {
        if (fadingOut)
            return;

        fadingOut = true;

        IEnumerator Routine()
        {
            for (float t = 0f; t < fadeOutDuration; t += Time.deltaTime)
            {
                fadeOutImage.color = new Color(0f, 0f, 0f, Mathf.Clamp01(t / fadeOutDuration));
                yield return null;
            }

            fadeOutImage.color = Color.black;

            SceneManager.LoadScene(sceneName);
        }

        StartCoroutine(Routine());
    }
}
