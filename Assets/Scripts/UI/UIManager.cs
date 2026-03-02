using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Reflection;
public class UIManager : MonoBehaviour
{
    private GunPlayerController gunPlayerController;
    private SwordPlayerController swordPlayerController;
    [SerializeField] private Image gunPlayerCooldownUI;
    [SerializeField] private Image swordPlayerCooldownUI;
    [SerializeField] private Image healthBarUI;
    [SerializeField] private Image gunMultiplierUI;
    [SerializeField] private Image swordMultiplierUI;

    [SerializeField] private Image superUI;

    private PlayerStats playerStats;

    // temp
    float health = 1.0f;

    private readonly int amountID = Shader.PropertyToID("_Amount");

    private readonly int leftAmountID = Shader.PropertyToID("_LeftAmount");
    private readonly int rightAmountID = Shader.PropertyToID("_RightAmount");
    

    void Awake()
    {
        gunPlayerController = GameObject.FindFirstObjectByType<GunPlayerController>();
        swordPlayerController = GameObject.FindFirstObjectByType<SwordPlayerController>();
        playerStats = GameObject.FindFirstObjectByType<PlayerStats>();
        Assert.IsNotNull(gunPlayerCooldownUI);
        Assert.IsNotNull(swordPlayerCooldownUI);
        Assert.IsNotNull(playerStats);
        Assert.IsNotNull(healthBarUI);
        Assert.IsNotNull(gunMultiplierUI);
        Assert.IsNotNull(swordMultiplierUI);

        Assert.IsNotNull(gunPlayerCooldownUI.material);
        Assert.IsNotNull(swordPlayerCooldownUI.material);
        Assert.IsNotNull(healthBarUI.material);
        Assert.IsNotNull(gunMultiplierUI.material);
        Assert.IsNotNull(swordMultiplierUI.material);
        Assert.IsNotNull(superUI.material);
    }

    void Start()
    {
        // Create new instances so as not to change the original mats
        gunPlayerCooldownUI.material = new Material(gunPlayerCooldownUI.material);
        swordPlayerCooldownUI.material = new Material(swordPlayerCooldownUI.material);
        healthBarUI.material = new Material(healthBarUI.material);
        gunMultiplierUI.material = new Material(gunMultiplierUI.material);
        swordMultiplierUI.material = new Material(swordMultiplierUI.material);

        superUI.material.SetFloat(leftAmountID, 0f);
        superUI.material.SetFloat(rightAmountID, 0f);

        
    }

    // Update is called once per frame
    void Update()
    {
        // Update the materials

        if (gunPlayerController != null)
        {
            float grenadeCooldown = gunPlayerController.GetCooldownPercent();
            gunPlayerCooldownUI.material.SetFloat(amountID, grenadeCooldown);
            // float gunMultiplier = gunPlayerController.GetMultiplier();
            float gunMultiplier = 0.8f; // temp        
            gunMultiplierUI.material.SetFloat(amountID, ConvertMultiplierToUIValue(gunMultiplier));
            float gunSuper = playerStats.GetGunSuperPercent();
            float gunSuperSmoothed = Mathf.Lerp(gunSuper, playerStats.GetGunSuperPercent(), 0.1f);
            superUI.material.SetFloat(leftAmountID, gunSuperSmoothed);

        }
        else
        {
            Debug.LogWarning("GunPlayerController not found. Gun cooldown and multiplier UI will not be updated.");
        }

        if (swordPlayerController != null)
        {   
            float swordCooldown = swordPlayerController.GetCooldownPercent();
            swordPlayerCooldownUI.material.SetFloat(amountID, swordCooldown);    
            // float swordMultiplier = swordPlayerController.GetMultiplier();
            float swordMultiplier = 0.2f; // temp
            swordMultiplierUI.material.SetFloat(amountID, ConvertMultiplierToUIValue(swordMultiplier));
            float swordSuper = playerStats.GetSwordSuperPercent();
            float swordSuperSmoothed = Mathf.Lerp(swordSuper, playerStats.GetSwordSuperPercent(), 0.1f);
            superUI.material.SetFloat(rightAmountID, swordSuperSmoothed);
        }
        else
        {
            Debug.LogWarning("SwordPlayerController not found. Sword cooldown and multiplier UI will not be updated.");
        }

        health = Mathf.Lerp(health, playerStats.GetHealthPercentage(), 0.1f);
        // float health = getHealth from playerController
        healthBarUI.material.SetFloat(amountID, health);       
    }
    float ConvertMultiplierToUIValue(float value)
    {
        float fillAmount = 0.0f;

        switch (value)
        {
            case 0.0f: fillAmount = 0.0f; break; // x1
            case 0.25f: fillAmount = 0.2f; break;  // x2
            case 0.5f: fillAmount = 0.42f; break; // x4
            case 0.75f: fillAmount = 0.67f; break; // x8
            case 1: fillAmount = 1.00f; break; // x16
            default: 
                fillAmount = value; // Fallback: use the raw value if it's not one of the expected multipliers
                Debug.LogWarning("Unexpected multiplier value: " + value + ". Using raw value for UI fill amount.");
                break;
        }
        return fillAmount;
    }
}

