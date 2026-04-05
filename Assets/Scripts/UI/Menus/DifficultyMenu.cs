using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class DifficultyMenu : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;

    [SerializeField] private Button easyButton;
    [SerializeField] private Button mediumButton;
    [SerializeField] private Button hardButton;

    [SerializeField] private GunPlayerController gunPlayerController;
    [SerializeField] private SwordPlayerController swordPlayerController;

    public UnityEvent onAllPlayersReady;

    void Awake()
    {
        Assert.IsNotNull(eventSystem);
        Assert.IsNotNull(easyButton);
        Assert.IsNotNull(mediumButton);
        Assert.IsNotNull(hardButton);

        easyButton.onClick.AddListener(() => OnDifficultySelected("Easy"));
        mediumButton.onClick.AddListener(() => OnDifficultySelected("Normal"));
        hardButton.onClick.AddListener(() => OnDifficultySelected("Hard"));
    }

    public void OnDifficultySelected(string difficulty)
    {
        Debug.Log($"[DifficultyMenu] Difficulty selected: {difficulty}");
        PlayerSelect.Instance.gameObject.GetComponent<DifficultyTracker>().SetDifficulty(difficulty);
        onAllPlayersReady?.Invoke();
        gameObject.SetActive(false);
    }

    public void ShowDifficultyMenu()
    {
        if(gunPlayerController != null)
            gunPlayerController.PlayerInput.enabled = false;
        if(swordPlayerController != null)
            swordPlayerController.PlayerInput.enabled = false;
        gameObject.SetActive(true);
        eventSystem.SetSelectedGameObject(mediumButton.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
