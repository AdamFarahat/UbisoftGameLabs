using UnityEngine;

public class MultiplierTextManager : MonoBehaviour
{
    public enum PlayerType { Gun, Sword }
    
    public PlayerType playerToTrack;

    private PlayerController currentPlayerController;
    [SerializeField] private MultiplierText[] texts;

    void Awake()
    {
        foreach (var text in texts)
        {
            text.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        // Assign player instance
        if (playerToTrack == PlayerType.Gun)
        {
            currentPlayerController = GunPlayerController.Instance;
        }
        else if (playerToTrack == PlayerType.Sword)
        {
            currentPlayerController = SwordPlayerController.Instance;
        }

        if (currentPlayerController == null)
        {
            Debug.LogWarning($"No {playerToTrack} PlayerController instance found. MultiplierTextManager will not function.");
            return;
        }

        // Subscribe to event
        currentPlayerController.OnDiscreteMultiplierChange.AddListener(AnimateText);
    }

    void AnimateText()
    {
        float multiplierIndex = currentPlayerController.GetNormalizedMultiplier();
        
        // Assign index
        int targetIndex = -1;
        if (Mathf.Approximately(multiplierIndex, 0.25f)) targetIndex = 0;
        else if (Mathf.Approximately(multiplierIndex, 0.5f)) targetIndex = 1;
        else if (Mathf.Approximately(multiplierIndex, 0.75f)) targetIndex = 2;
        else if (Mathf.Approximately(multiplierIndex, 1f)) targetIndex = 3;
        
        // Loop through all texts and update their states
        for (int i = 0; i < texts.Length; i++)
        {
            if (i == targetIndex)
            {
                // If it's currently off, turn it on and call the animation
                if (!texts[i].gameObject.activeSelf)
                {
                    texts[i].gameObject.SetActive(true);
                    texts[i].AnimateTextIn(); 
                }
            }
            else
            {
                // Only trigger the hide animation on texts that are currently visible
                if (texts[i].gameObject.activeSelf)
                {
                    texts[i].AnimateTextOut();
                }
            }
        }
    }
}