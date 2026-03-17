using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    private TutorialBase[] tutorials;

    private int tutorialIndex = -1;

    private void Awake()
    {
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
            GunPlayerController.Instance.toggleGunEnabled = false;
        }

        if (SwordPlayerController.Instance != null)
        {
            SwordPlayerController.Instance.StartButtonPressed += OnStartButtonPressed;
            SwordPlayerController.Instance.moveEnabled = false;
            SwordPlayerController.Instance.slashEnabled = false;
            SwordPlayerController.Instance.jumpEnabled = false;
        }

        NextTutorial();
    }

    private void OnStartButtonPressed()
    {
        SceneManager.LoadScene("Menu");
    }

    public void NextTutorial()
    {
        tutorialIndex++;
        if (tutorialIndex < tutorials.Length)
            tutorials[tutorialIndex].DoTutorial();
        else
        {
            // TODO end tutorial
        }
    }
}
