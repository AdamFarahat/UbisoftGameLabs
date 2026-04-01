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

    void Awake()
    {
        Assert.IsNotNull(resumeButton);
        Assert.IsNotNull(optionsButton);
        Assert.IsNotNull(restartButton);
        Assert.IsNotNull(mainMenuButton);
    }

    void OnDisable()
    {
        resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
        optionsButton.onClick.RemoveListener(OnOptionsButtonClicked);
        restartButton.onClick.RemoveListener(OnRestartButtonClicked);
        mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);
    }

    public void ShowPauseMenu()
    {
        
        Debug.Log("Showing pause menu");
        Time.timeScale = 0f; // Pause the game

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
    
    public void OnResumeButtonClicked()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);

        resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
        optionsButton.onClick.RemoveListener(OnOptionsButtonClicked);
        restartButton.onClick.RemoveListener(OnRestartButtonClicked);
        mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);

        eventSystem.SetSelectedGameObject(null);
    }

    public void OnOptionsButtonClicked()
    {
        // TODO: Implement options menu logic here
        Debug.Log("Options button clicked");
    }

    public void OnRestartButtonClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnMainMenuButtonClicked()
    {
        SceneManager.LoadScene("Menu");
    }
}
