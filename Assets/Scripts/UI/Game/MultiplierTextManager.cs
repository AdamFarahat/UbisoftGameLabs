using UnityEngine;
using TMPro; 
using DG.Tweening;
using UnityEngine.Assertions;

public class MultiplierTextManager : MonoBehaviour
{
    public enum PlayerType { Gun, Sword }
    public PlayerType playerToTrack;

    private PlayerController currentPlayerController;
    [SerializeField] private MultiplierText[] texts;
    
    [Header("Super Multiplier Text")]
    [SerializeField] private MultiplierText superText;
    [SerializeField] private TextMeshProUGUI superTextTMP;
    [SerializeField] private float pulseScale = 1.2f;
    [SerializeField] private float pulseDuration = 0.5f;
    private float originalFontSize;

    void Awake()
    {
        Assert.IsNotNull(superText);
        Assert.IsNotNull(superTextTMP);

        foreach (var text in texts)
        {
            text.gameObject.SetActive(false);
        }
        if (superText != null) superText.gameObject.SetActive(false);
    }

    void Start()
    {
        // Assign player instance
        if (playerToTrack == PlayerType.Gun)
            currentPlayerController = GunPlayerController.Instance;
        else if (playerToTrack == PlayerType.Sword)
            currentPlayerController = SwordPlayerController.Instance;

        if (currentPlayerController == null)
        {
            Debug.LogWarning($"No {playerToTrack} PlayerController instance found. MultiplierTextManager will not function.");
            return;
        }

        // Subscribe to regular multiplier changes
        currentPlayerController.OnDiscreteMultiplierChange += AnimateText;
        
        // Subscribe to Super events
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.SuperStarted += ShowSuperText;
            PlayerStats.Instance.SuperEnded += AnimateText; // Force standard text to return when super ends
        }

        // Store original font size for pulsing effect
        if (superTextTMP != null)
        {
            originalFontSize = superTextTMP.fontSize;
        }
    }

    void ShowSuperText()
    {
        // Hide all normal texts
        foreach (var text in texts)
        {
            if (text.gameObject.activeSelf)
                text.AnimateTextOut();
        }

        // Dynamically set the combined string value
        float combinedMult = currentPlayerController.GetDiscreteMultiplier();
        superTextTMP.text = "x" + combinedMult.ToString();
        
        // Double the font size
        superTextTMP.fontSize = originalFontSize * 2;
        
        superText.gameObject.SetActive(true);
                
        // Begin pulsing animation
        superText.transform.DOKill(true);
        superText.transform.localScale = Vector3.zero;

        // Wait until the text has fully animated in before starting the pulse
        superText.transform.DOScale(pulseScale, 0.4f).SetEase(Ease.OutQuad).OnComplete(() => 
            {
                // Start pulsing
                if (superText != null && superText.gameObject.activeSelf)
                {
                    superText.transform.DOScale(1f, pulseDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                }
            });
    }
    void AnimateText()
    {
        // Entering Super: Hide normal multiplier texts and show super text with combined multiplier
        if (PlayerStats.Instance != null && PlayerStats.Instance.IsSuperActive())
        {
            float combinedMult = currentPlayerController.GetDiscreteMultiplier();
            superTextTMP.text = "x" + combinedMult.ToString();
            
            // Exit early so we don't trigger the normal base text animations
            return; 
        }

        // Exiting Super: Hide super text and show normal multiplier texts based on the current multiplier index
        if (superText != null && superText.gameObject.activeSelf)
        {
            // Kill the looping pulse tween so it doesn't run in the background
            superText.transform.DOKill();
            
            // Animate out
            superText.AnimateTextOut();
        }

        // Multiplier logic 
        float multiplierIndex = currentPlayerController.GetNormalizedMultiplier();
        
        // Assign index based on the normalized float
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