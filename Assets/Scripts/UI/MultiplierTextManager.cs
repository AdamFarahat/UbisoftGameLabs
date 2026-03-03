using UnityEngine;

public class MultiplierTextManager : MonoBehaviour
{
    private GunPlayerController gunPlayerController;
    private SwordPlayerController swordPlayerController;

    private PlayerController currentPlayerController;

    [SerializeField] private MultiplierText[] texts;

    void Awake()
    {
        foreach (var text in texts)
        {
            text.gameObject.SetActive(false);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gunPlayerController = GunPlayerController.Instance;
        swordPlayerController = SwordPlayerController.Instance;

        if (gunPlayerController != null)
            currentPlayerController = gunPlayerController;
        else if (swordPlayerController != null)
            currentPlayerController = swordPlayerController;
        else
        {
            Debug.LogWarning("No PlayerController instance found. MultiplierTextManager will not function.");
            return;
        }

        Debug.Log("Current PlayerController: " + currentPlayerController.GetType().Name);

        currentPlayerController.OnDiscreteMultiplierChange.AddListener(ManageText);
    }

    void ManageText()
    {
        float multiplierIndex = currentPlayerController.GetNormalizedMultiplier();
        
        // 1. Use Mathf.Approximately to safely compare floats!
        int targetIndex = -1;
        if (Mathf.Approximately(multiplierIndex, 0.25f)) targetIndex = 0;
        else if (Mathf.Approximately(multiplierIndex, 0.5f)) targetIndex = 1;
        else if (Mathf.Approximately(multiplierIndex, 0.75f)) targetIndex = 2;
        else if (Mathf.Approximately(multiplierIndex, 1f)) targetIndex = 3;

        // If it's 0 (or some unexpected number), targetIndex stays -1 and hides everything.
        
        // 2. Loop through all texts and update their states
        for (int i = 0; i < texts.Length; i++)
        {
            if (i == targetIndex)
            {
                // If it's currently off, turn it on AND explicitly call the animation
                if (!texts[i].gameObject.activeSelf)
                {
                    texts[i].gameObject.SetActive(true);
                    texts[i].ShowText(); // Explicit call is much safer for UI!
                }
            }
            else
            {
                // Only trigger the hide animation on texts that are currently visible
                if (texts[i].gameObject.activeSelf)
                {
                    texts[i].HideText();
                }
            }
        }
    }
}
