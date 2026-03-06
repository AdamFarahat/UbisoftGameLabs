using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;

public class UIManager : MonoBehaviour
{
    private GunPlayerController gunPlayerController;
    private SwordPlayerController swordPlayerController;
    private ScoreManagerSO scoreManagerSO;
    private PlayerStats playerStats;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Image healthBarUI;
    [SerializeField] private Image superUI;
    [SerializeField] private Image gunPlayerCooldownUI;
    [SerializeField] private Image gunMultiplierUI;
    [SerializeField] private Image swordPlayerCooldownUI;
    [SerializeField] private Image swordMultiplierUI;


    float health = 1.0f;
    float lerpSpeed = 0.1f;

    private readonly int amountID = Shader.PropertyToID("_Amount");
    private readonly int leftAmountID = Shader.PropertyToID("_LeftAmount");
    private readonly int rightAmountID = Shader.PropertyToID("_RightAmount");
    

    void Awake()
    {   
        Assert.IsNotNull(healthBarUI);
        Assert.IsNotNull(superUI);
        Assert.IsNotNull(scoreText);

        Assert.IsNotNull(gunMultiplierUI);
        Assert.IsNotNull(gunPlayerCooldownUI);

        Assert.IsNotNull(swordMultiplierUI);
        Assert.IsNotNull(swordPlayerCooldownUI);
        

        Assert.IsNotNull(healthBarUI.material);
        Assert.IsNotNull(superUI.material);

        Assert.IsNotNull(gunMultiplierUI.material);
        Assert.IsNotNull(gunPlayerCooldownUI.material);

        Assert.IsNotNull(swordMultiplierUI.material);
        Assert.IsNotNull(swordPlayerCooldownUI.material);
    }

    void Start()
    {
        gunPlayerController = GunPlayerController.Instance;
        swordPlayerController = SwordPlayerController.Instance;
        scoreManagerSO = ScoreManagerSO.Instance;
        playerStats = PlayerStats.Instance;

        if (gunPlayerController == null)
            Debug.LogWarning("GunPlayerController instance not found. UI will not be updated.");
        if (swordPlayerController == null)
            Debug.LogWarning("SwordPlayerController instance not found. UI will not be updated.");
        if (scoreManagerSO == null)
            Debug.LogWarning("ScoreManagerSO instance not found. Score UI will not be updated.");
        if (playerStats == null)
            Debug.LogWarning("PlayerStats instance not found. Health UI will not be updated.");

        // Create new instances so as not to change the original mats
        healthBarUI.material = new Material(healthBarUI.material);

        superUI.material.SetFloat(leftAmountID, 0f);
        superUI.material.SetFloat(rightAmountID, 0f);

        gunMultiplierUI.material = new Material(gunMultiplierUI.material);
        gunPlayerCooldownUI.material = new Material(gunPlayerCooldownUI.material);

        swordMultiplierUI.material = new Material(swordMultiplierUI.material);
        swordPlayerCooldownUI.material = new Material(swordPlayerCooldownUI.material);

        if (gunPlayerController != null)
            gunPlayerController.OnGrenadeCooldownReady += TriggerGunCooldownPulse;

        if (swordPlayerController != null)
            swordPlayerController.OnBlockCooldownReady += TriggerSwordCooldownPulse;
    }

    // Update is called once per frame
    void Update()
    {
        // Update the materials. If the required instance doesnt exist, simply ignore

        if (gunPlayerController != null)
        {
            float grenadeCooldown = gunPlayerController.GetCooldownPercent();
            gunPlayerCooldownUI.material.SetFloat(amountID, grenadeCooldown);

            float gunMultiplier = gunPlayerController.GetNormalizedMultiplier();
            gunMultiplierUI.material.SetFloat(amountID, ConvertMultiplierToUIValue(gunMultiplier));

            float gunSuper = PlayerStats.Instance.GetGunSuperPercent();
            float gunSuperSmoothed = Mathf.Lerp(gunSuper, PlayerStats.Instance.GetGunSuperPercent(), lerpSpeed);
            superUI.material.SetFloat(leftAmountID, gunSuperSmoothed);
        }

        if (swordPlayerController != null)
        {   
            
            float swordCooldown = swordPlayerController.GetCooldownPercent();
            swordPlayerCooldownUI.material.SetFloat(amountID, swordCooldown);  

            float swordMultiplier = swordPlayerController.GetNormalizedMultiplier();
            swordMultiplierUI.material.SetFloat(amountID, ConvertMultiplierToUIValue(swordMultiplier));

            float swordSuper = PlayerStats.Instance.GetSwordSuperPercent();
            float swordSuperSmoothed = Mathf.Lerp(swordSuper, PlayerStats.Instance.GetSwordSuperPercent(), lerpSpeed);
            superUI.material.SetFloat(rightAmountID, swordSuperSmoothed);
        }

        if (scoreManagerSO != null)
        {
            int score = ScoreManagerSO.CalculateOverallTeamScore();
            scoreText.text = score.ToString();
        }

        if (playerStats != null)
        {
            health = Mathf.Lerp(health, PlayerStats.Instance.GetHealthPercentage(), lerpSpeed);
            healthBarUI.material.SetFloat(amountID, health);   
        }   
    }
    float ConvertMultiplierToUIValue(float value)
    {
        float fillAmount = 0.0f;

        switch (value)
        {
            case 0.0f: fillAmount = 0.0f; break; // x1
            case 0.25f: fillAmount = 0.2f; break;  // x2
            case 0.5f: fillAmount = 0.42f; break; // x4
            case 0.75f: fillAmount = 0.67f; break; // x6
            case 1: fillAmount = 1.00f; break; // x8
            default: 
                fillAmount = value; // Fallback: use the raw value if it's not one of the expected multipliers
                Debug.LogWarning("Unexpected multiplier value: " + value + ". Using raw value for UI fill amount.");
                break;
        }
        return fillAmount;
    }

    private void OnDestroy()
    {
        // Clean up the subscription if the UI is destroyed
        if (gunPlayerController != null)
            gunPlayerController.OnGrenadeCooldownReady -= TriggerGunCooldownPulse;

        if (swordPlayerController != null)
            swordPlayerController.OnBlockCooldownReady -= TriggerSwordCooldownPulse;
    }

    private void TriggerGunCooldownPulse()
    {
        // Start the stopwatch for the Shader Graph pulse animation
        gunPlayerCooldownUI.material.SetFloat("_TimeHitZero", Time.time);
    }

    private void TriggerSwordCooldownPulse()
    {
        // Start the stopwatch for the Shader Graph pulse animation
        swordPlayerCooldownUI.material.SetFloat("_TimeHitZero", Time.time);
    }
}

