using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private MenuFader pauseFader;
    [SerializeField] private MenuFader blackScreenFader;
    [SerializeField] private GameObject optionsMenu;

    void Awake()
    {
        Assert.IsNotNull(resumeButton);
        Assert.IsNotNull(optionsButton);
        Assert.IsNotNull(restartButton);
        Assert.IsNotNull(mainMenuButton);
        Assert.IsNotNull(pauseFader);
    }

    private void OnDisable()
    {
        resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
        optionsButton.onClick.RemoveListener(OnOptionsButtonClicked);
        restartButton.onClick.RemoveListener(OnRestartButtonClicked);
        mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);
    }
    private void OnEnable()
    {
        
        Debug.Log("Showing pause menu");
        Time.timeScale = 0f; // Pause the game

        pauseFader.FadeToOpaque();
        
        // Set the first selected button to resumeButton
        gameObject.SetActive(true);

        resumeButton.onClick.AddListener(OnResumeButtonClicked);
        optionsButton.onClick.AddListener(OnOptionsButtonClicked);
        restartButton.onClick.AddListener(OnRestartButtonClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        IEnumerator setSelectedButtonNextFrame()
        {
            // Wait for the end of the frame to ensure the UI is fully active
            yield return new WaitForEndOfFrame();
            eventSystem.SetSelectedGameObject(resumeButton.gameObject);
        }

        StartCoroutine(setSelectedButtonNextFrame());
    }
    public void ShowPauseMenu()
    {
        
        Debug.Log("Showing pause menu");
        pauseFader.FadeToOpaque();
        Time.timeScale = 0f; // Pause the game

        // Set the first selected button to resumeButton
        gameObject.SetActive(true);
    }
    
    public void OnResumeButtonClicked()
    {
        Time.timeScale = 1f;
        pauseFader.FadeToTransparent();

        resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
        optionsButton.onClick.RemoveListener(OnOptionsButtonClicked);
        restartButton.onClick.RemoveListener(OnRestartButtonClicked);
        mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);

        eventSystem.SetSelectedGameObject(null);

        FindFirstObjectByType<UIManager>().OnResume();
    }

    public void OnOptionsButtonClicked()
    {
        // TODO: Implement options menu logic here
        gameObject.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void OnRestartButtonClicked()
    {
        Time.timeScale = 1f;
        blackScreenFader.FadeToOpaque(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);      
        });

    }

    public void OnMainMenuButtonClicked()
    {
        Time.timeScale = 1f;
        blackScreenFader.FadeToOpaque(() =>
        {
            SceneManager.LoadScene("Menu");
        });
    }
}
