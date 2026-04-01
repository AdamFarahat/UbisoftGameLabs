using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [Header("Global UI Elements")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Image healthBarUI;
    [SerializeField] private Image vignetteUI;
    [SerializeField] private Image superUI;
    [SerializeField] private GameOver gameOverScreen;

    [Header("Player Specific HUDs")]
    [SerializeField] private PlayerHUD gunPlayerHUD;
    [SerializeField] private PlayerHUD swordPlayerHUD;

    // Cached References
    private GunPlayerController gunPlayerController;
    private SwordPlayerController swordPlayerController;
    private ScoreManagerSO scoreManagerSO;
    private PlayerStats playerStats;

    // State Variables
    private float currentHealthVisual = 1.0f;
    private const float LERP_SPEED = 0.1f;

    // Shader Property IDs
    private readonly int amountID = Shader.PropertyToID("_Amount");
    private readonly int leftAmountID = Shader.PropertyToID("_LeftAmount");
    private readonly int rightAmountID = Shader.PropertyToID("_RightAmount");
    private readonly int isSuperID = Shader.PropertyToID("_IsSuper");

    // Tweens
    private Tween healthColorTween;
    private Tween vignetteTween;

    void Awake()
    {
        Assert.IsNotNull(healthBarUI, "Health Bar UI is missing!");
        Assert.IsNotNull(superUI, "Super UI is missing!");
        Assert.IsNotNull(scoreText, "Score Text is missing!");
        Assert.IsNotNull(gunPlayerHUD, "Gun Player HUD is missing!");
        Assert.IsNotNull(swordPlayerHUD, "Sword Player HUD is missing!");
    }

    void Start()
    {
        CacheSystemReferences();
        InitializeMaterials();
        SubscribeToEvents();

        // Set initial fill amounts
        UpdateGunMultiplierUI();
        UpdateSwordMultiplierUI();
    }

    private void CacheSystemReferences()
    {
        gunPlayerController = GunPlayerController.Instance;
        swordPlayerController = SwordPlayerController.Instance;
        scoreManagerSO = ScoreManagerSO.Instance;
        playerStats = PlayerStats.Instance;
    }

    private void InitializeMaterials()
    {
        healthBarUI.material = new Material(healthBarUI.material);
        
        superUI.material = new Material(superUI.material);
        superUI.material.SetFloat(leftAmountID, 0f);
        superUI.material.SetFloat(rightAmountID, 0f);

        gunPlayerHUD.Initialize();
        swordPlayerHUD.Initialize();
    }

    private void SubscribeToEvents()
    {
        if (gunPlayerController != null)
        {
            gunPlayerController.OnGrenadeCooldownReady += gunPlayerHUD.TriggerCooldownPulse;
            gunPlayerController.OnDiscreteMultiplierChange.AddListener(UpdateGunMultiplierUI);
        }

        if (swordPlayerController != null)
        {
            swordPlayerController.OnBlockCooldownReady += swordPlayerHUD.TriggerCooldownPulse;
            swordPlayerController.OnDiscreteMultiplierChange.AddListener(UpdateSwordMultiplierUI);
        }

        if (playerStats != null)
        {
            playerStats.SuperStarted += OnSuperStarted;
            playerStats.SuperEnded += OnSuperEnded;
        }
    }

    void Update()
    {
        UpdateCooldowns();
        UpdateSuperMeter();
        UpdateScore();
        UpdateHealth();
    }

    // UPDATE UI'S
    private void UpdateCooldowns()
    {
        if (gunPlayerController != null)
            gunPlayerHUD.UpdateCooldown(gunPlayerController.GetCooldownPercent());

        if (swordPlayerController != null)
            swordPlayerHUD.UpdateCooldown(swordPlayerController.GetCooldownPercent());
    }

    private void UpdateSuperMeter()
    {
        if (playerStats == null) return;

        float gunSuperSmoothed = Mathf.Lerp(superUI.material.GetFloat(leftAmountID), playerStats.GetGunSuperPercent(), LERP_SPEED);
        superUI.material.SetFloat(leftAmountID, gunSuperSmoothed);

        float swordSuperSmoothed = Mathf.Lerp(superUI.material.GetFloat(rightAmountID), playerStats.GetSwordSuperPercent(), LERP_SPEED);
        superUI.material.SetFloat(rightAmountID, swordSuperSmoothed);
    }

    private void UpdateScore()
    {
        if (scoreManagerSO != null)
        {
            scoreText.text = ScoreManagerSO.CalculateOverallTeamScore().ToString();
        }
    }

    private void UpdateHealth()
    {
        if (playerStats != null)
        {
            currentHealthVisual = Mathf.Lerp(currentHealthVisual, playerStats.GetHealthPercentage(), LERP_SPEED);
            healthBarUI.material.SetFloat(amountID, currentHealthVisual);
        }
    }

    // EVENT HANDLERS 

    private void UpdateGunMultiplierUI()
    {
        if (playerStats == null || gunPlayerController == null) return;
        gunPlayerHUD.UpdateMultiplier(gunPlayerController.GetNormalizedMultiplier(), playerStats.IsSuperActive());
    }

    private void UpdateSwordMultiplierUI()
    {
        if (playerStats == null || swordPlayerController == null) return;
        swordPlayerHUD.UpdateMultiplier(swordPlayerController.GetNormalizedMultiplier(), playerStats.IsSuperActive());
    }

    private void OnSuperStarted()
    {
        // Health Heart Glow
        healthColorTween?.Kill();
        healthColorTween = healthBarUI.material.DOFloat(1f, isSuperID, 0.75f).OnUpdate(() => healthBarUI.SetMaterialDirty());

        // Activate Vignette
        vignetteTween?.Kill();
        vignetteTween = vignetteUI.material.DOFloat(1f, isSuperID, 0.75f).OnUpdate(() => vignetteUI.SetMaterialDirty());

        // Player HUD Animations
        gunPlayerHUD.PlaySuperStartAnimation();
        swordPlayerHUD.PlaySuperStartAnimation();
    }

    private void OnSuperEnded()
    {
        // Health Heart Glow Removal
        healthColorTween?.Kill();
        healthColorTween = healthBarUI.material.DOFloat(0f, isSuperID, 0.3f).OnUpdate(() => healthBarUI.SetMaterialDirty());

        // Deactivate Vignette
        vignetteTween?.Kill();
        vignetteTween = vignetteUI.material.DOFloat(0f, isSuperID, 0.3f).OnUpdate(() => vignetteUI.SetMaterialDirty());

        // Player HUD Animations
        float gunNorm = gunPlayerController != null ? gunPlayerController.GetNormalizedMultiplier() : 0f;
        gunPlayerHUD.PlaySuperEndAnimation(gunNorm);

        float swordNorm = swordPlayerController != null ? swordPlayerController.GetNormalizedMultiplier() : 0f;
        swordPlayerHUD.PlaySuperEndAnimation(swordNorm);
    }

    public void ShowGameOverScreen()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.gameObject.SetActive(true);
            gameOverScreen.ShowGameOverScreen();
        }
    }

    private void OnDestroy()
    {
        if (gunPlayerController != null)
        {
            gunPlayerController.OnGrenadeCooldownReady -= gunPlayerHUD.TriggerCooldownPulse;
            gunPlayerController.OnDiscreteMultiplierChange.RemoveListener(UpdateGunMultiplierUI);
        }

        if (swordPlayerController != null)
        {
            swordPlayerController.OnBlockCooldownReady -= swordPlayerHUD.TriggerCooldownPulse;
            swordPlayerController.OnDiscreteMultiplierChange.RemoveListener(UpdateSwordMultiplierUI);
        }

        if (playerStats != null)
        {
            playerStats.SuperStarted -= OnSuperStarted;
            playerStats.SuperEnded -= OnSuperEnded;
        }

        healthColorTween?.Kill();
        vignetteTween?.Kill();
    }
}