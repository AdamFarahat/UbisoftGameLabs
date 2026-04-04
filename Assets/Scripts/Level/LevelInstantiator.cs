using UnityEngine;

public class LevelInstantiator : MonoBehaviour
{
    private string currentSceneName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        switch (currentSceneName)
        {
            case "Game":
                AudioManager.Instance.PlayMusic(FMODEvents.Instance.OSTGameStart);
                break;
            case "Menu":
                AudioManager.Instance.PlayMusic(FMODEvents.Instance.OSTMenu);
                break;
            case "Tutorial":
                AudioManager.Instance.PlayMusic(FMODEvents.Instance.OSTutorial);
                break;
            default:
                Debug.LogWarning($"No specific music assigned for scene '{currentSceneName}'.");
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
