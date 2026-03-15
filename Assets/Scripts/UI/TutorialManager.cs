using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    private void Start()
    {
        if (GunPlayerController.Instance != null)
            GunPlayerController.Instance.StartButtonPressed += OnStartButtonPressed;

        if (SwordPlayerController.Instance != null)
            SwordPlayerController.Instance.StartButtonPressed += OnStartButtonPressed;
    }

    private void OnStartButtonPressed()
    {
        SceneManager.LoadScene("Menu");
    }
}
