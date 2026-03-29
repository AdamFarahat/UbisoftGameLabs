using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;
using DG.Tweening;

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

    [SerializeField] private GameOver gameOverScreen;

    float health = 1.0f;
    float lerpSpeed = 0.1f;

    private readonly int amountID = Shader.PropertyToID("_Amount");
    private readonly int leftAmountID = Shader.PropertyToID("_LeftAmount");
    private readonly int rightAmountID = Shader.PropertyToID("_RightAmount");
    private readonly int isSuperID = Shader.PropertyToID("_IsSuper");
    
    // Tween References to prevent animations from overlapping
    private Tween gunFillTween;
    private Tween swordFillTween;
    private Tween gunColorTween;
    private Tween swordColorTween;

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
        {
            gunPlayerController.OnGrenadeCooldownReady += TriggerGunCooldownPulse;
            gunPlayerController.OnDiscreteMultiplierChange.AddListener(UpdateGunMultiplierUI);
        }

        if (swordPlayerController != null)
        {
            swordPlayerController.OnBlockCooldownReady += TriggerSwordCooldownPulse;
            swordPlayerController.OnDiscreteMultiplierChange.AddListener(UpdateSwordMultiplierUI);
        }

        if (playerStats != null)
        {
            playerStats.SuperStarted += OnSuperStarted;
            playerStats.SuperEnded += OnSuperEnded;
        }

        // Set initial fill amounts
        UpdateGunMultiplierUI();
        UpdateSwordMultiplierUI();
    }

    void Update()
    {
        if (gunPlayerController != null)
        {
            float grenadeCooldown = gunPlayerController.GetCooldownPercent();
            gunPlayerCooldownUI.material.SetFloat(amountID, grenadeCooldown);

            float gunSuper = PlayerStats.Instance.GetGunSuperPercent();
            float gunSuperSmoothed = Mathf.Lerp(superUI.material.GetFloat(leftAmountID), gunSuper, lerpSpeed);
            superUI.material.SetFloat(leftAmountID, gunSuperSmoothed);
        }

        if (swordPlayerController != null)
        {   
            float swordCooldown = swordPlayerController.GetCooldownPercent();
            swordPlayerCooldownUI.material.SetFloat(amountID, swordCooldown);  

            float swordSuper = PlayerStats.Instance.GetSwordSuperPercent();
            float swordSuperSmoothed = Mathf.Lerp(superUI.material.GetFloat(rightAmountID), swordSuper, lerpSpeed);
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

    private void UpdateGunMultiplierUI()
    {
        if (playerStats != null && playerStats.IsSuperActive()) return;

        float gunMultiplier = gunPlayerController.GetNormalizedMultiplier();
        float targetValue = ConvertMultiplierToUIValue(gunMultiplier);
        
        gunFillTween?.Kill(); 
        gunFillTween = gunMultiplierUI.material.DOFloat(targetValue, amountID, 0.5f).SetEase(Ease.OutBack);
    }

    private void UpdateSwordMultiplierUI()
    {
        if (playerStats != null && playerStats.IsSuperActive()) return;

        float swordMultiplier = swordPlayerController.GetNormalizedMultiplier();
        float targetValue = ConvertMultiplierToUIValue(swordMultiplier);
        
        swordFillTween?.Kill();
        swordFillTween = swordMultiplierUI.material.DOFloat(targetValue, amountID, 0.5f).SetEase(Ease.OutBack);
    }

    private void OnSuperStarted()
    {
        // Lerp Color to 1
        gunColorTween?.Kill();
        swordColorTween?.Kill();
        gunColorTween = gunMultiplierUI.material.DOFloat(1f, isSuperID, 0.5f);
        swordColorTween = swordMultiplierUI.material.DOFloat(1f, isSuperID, 0.5f);

        // Lerp Fill Amount to 1
        gunFillTween?.Kill();
        swordFillTween?.Kill();
        gunFillTween = gunMultiplierUI.material.DOFloat(1f, amountID, 0.5f).SetEase(Ease.Linear);
        swordFillTween = swordMultiplierUI.material.DOFloat(1f, amountID, 0.5f).SetEase(Ease.Linear);
    }

    private void OnSuperEnded()
    {
        // Lerp Color back to 0
        gunColorTween?.Kill();
        swordColorTween?.Kill();
        gunColorTween = gunMultiplierUI.material.DOFloat(0f, isSuperID, 0.3f);
        swordColorTween = swordMultiplierUI.material.DOFloat(0f, isSuperID, 0.3f);

        // Send the bars back to their normal values
        UpdateGunMultiplierUI();
        UpdateSwordMultiplierUI();
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
                fillAmount = value; // Fallback to using the raw value for fill amount if it's an unexpected value
                Debug.LogWarning("Unexpected multiplier value: " + value + ". Using raw value for UI fill amount.");
                break;
        }
        return fillAmount;
    }

    private void OnDestroy()
    {
        // Clean up subscriptions
        if (gunPlayerController != null)
        {
            gunPlayerController.OnGrenadeCooldownReady -= TriggerGunCooldownPulse;
            gunPlayerController.OnDiscreteMultiplierChange.RemoveListener(UpdateGunMultiplierUI);
        }

        if (swordPlayerController != null)
        {
            swordPlayerController.OnBlockCooldownReady -= TriggerSwordCooldownPulse;
            swordPlayerController.OnDiscreteMultiplierChange.RemoveListener(UpdateSwordMultiplierUI);
        }

        if (playerStats != null)
        {
            playerStats.SuperStarted -= OnSuperStarted;
            playerStats.SuperEnded -= OnSuperEnded;
        }

        // Kill tweens so they don't cause errors after the scene unloads
        gunFillTween?.Kill();
        swordFillTween?.Kill();
        gunColorTween?.Kill();
        swordColorTween?.Kill();
    }

    private void TriggerGunCooldownPulse()
    {
        gunPlayerCooldownUI.material.SetFloat("_TimeHitZero", Time.time);
    }

    private void TriggerSwordCooldownPulse()
    {
        swordPlayerCooldownUI.material.SetFloat("_TimeHitZero", Time.time);
    }

    public void ShowGameOverScreen()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.gameObject.SetActive(true);
            gameOverScreen.ShowGameOverScreen();
        }
        else
        {
            Debug.LogWarning("GameOver screen reference is missing. Cannot show Game Over screen.");
        }
    }
}