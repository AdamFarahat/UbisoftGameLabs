using UnityEngine;
using UnityEngine.SceneManagement; 

public class SceneManagement : MonoBehaviour
{
    [SerializeField] private MenuAnimations menuAnimations;
    private string sceneToLoad = "";

    void OnEnable()
    {
        // Add listener 
        if (menuAnimations != null)
            menuAnimations.OnMenuAnimateOutComplete += LoadPendingScene;
    }

    void OnDisable()
    {
        // Clean up the listener
        if (menuAnimations != null)
            menuAnimations.OnMenuAnimateOutComplete -= LoadPendingScene;
    }

    // Hook up to Buttons in inspector 
    public void TransitionToScene(string sceneName)
    {
        sceneToLoad = sceneName;
        
        if (menuAnimations != null) {
            menuAnimations.AnimateButtonsOut();
        } else {
            // Fallback just in case there are no animations hooked up
            LoadPendingScene();
        }
    }

    private void LoadPendingScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}