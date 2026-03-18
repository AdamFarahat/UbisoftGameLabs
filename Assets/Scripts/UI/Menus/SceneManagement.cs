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

    // Hooked up to Buttons in inspector 
    // todo refactor
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

    public void LoadPendingScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    public void PrepareSceneLoad(string sceneName)
    {
        sceneToLoad = sceneName;
    }
}