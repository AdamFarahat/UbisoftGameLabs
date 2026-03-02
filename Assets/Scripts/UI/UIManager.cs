using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;
public class UIManager : MonoBehaviour
{
    private GunPlayerController gunPlayerController;
    private SwordPlayerController swordPlayerController;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Image healthBarUI;
    [SerializeField] private Image gunPlayerCooldownUI;
    [SerializeField] private Image gunPlayerPowerBarUI;
    [SerializeField] private Image gunMultiplierUI;
    [SerializeField] private Image swordPlayerCooldownUI;
    [SerializeField] private Image swordPlayerPowerBarUI;
    [SerializeField] private Image swordMultiplierUI;

    // temp
    float health = 1.0f;

    private readonly int amountID = Shader.PropertyToID("_Amount");
    private readonly int leftPowerID = Shader.PropertyToID("_LeftAmount");
    private readonly int rightPowerID = Shader.PropertyToID("_RightAmount");
    

    void Awake()
    {
        gunPlayerController = GameObject.FindFirstObjectByType<GunPlayerController>();
        swordPlayerController = GameObject.FindFirstObjectByType<SwordPlayerController>();
        Assert.IsNotNull(healthBarUI);
        Assert.IsNotNull(scoreText);

        Assert.IsNotNull(gunMultiplierUI);
        Assert.IsNotNull(gunPlayerCooldownUI);
        Assert.IsNotNull(gunPlayerPowerBarUI);

        Assert.IsNotNull(swordMultiplierUI);
        Assert.IsNotNull(swordPlayerCooldownUI);
        Assert.IsNotNull(swordPlayerPowerBarUI);
        

        Assert.IsNotNull(healthBarUI.material);

        Assert.IsNotNull(gunMultiplierUI.material);
        Assert.IsNotNull(gunPlayerCooldownUI.material);
        Assert.IsNotNull(gunPlayerPowerBarUI.material);

        Assert.IsNotNull(swordMultiplierUI.material);
        Assert.IsNotNull(swordPlayerCooldownUI.material);
        Assert.IsNotNull(swordPlayerPowerBarUI.material);
    }

    void Start()
    {
        // Create new instances so as not to change the original mats
        healthBarUI.material = new Material(healthBarUI.material);

        gunMultiplierUI.material = new Material(gunMultiplierUI.material);
        gunPlayerCooldownUI.material = new Material(gunPlayerCooldownUI.material);
        gunPlayerPowerBarUI.material = new Material(gunPlayerPowerBarUI.material);

        swordMultiplierUI.material = new Material(swordMultiplierUI.material);
        swordPlayerCooldownUI.material = new Material(swordPlayerCooldownUI.material);
        swordPlayerPowerBarUI.material = new Material(swordPlayerPowerBarUI.material);   
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
            float gunMultiplier = 0.75f; // temp        
            gunMultiplierUI.material.SetFloat(amountID, ConvertMultiplierToUIValue(gunMultiplier));

            // float getPowerBarPercent = gunPlayerController.GetPowerBarPercent();
            float getPowerBarPercent = 0.0f; // temp
            gunPlayerPowerBarUI.material.SetFloat(leftPowerID, getPowerBarPercent);
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
            float swordMultiplier = 0.25f; // temp
            swordMultiplierUI.material.SetFloat(amountID, ConvertMultiplierToUIValue(swordMultiplier));

            // float getSwordPowerBarPercent = swordPlayerController.GetPowerBarPercent();
            float getSwordPowerBarPercent = 0.75f; // temp
            swordPlayerPowerBarUI.material.SetFloat(rightPowerID, getSwordPowerBarPercent);
        }
        else
        {
            Debug.LogWarning("SwordPlayerController not found. Sword cooldown and multiplier UI will not be updated.");
        }

        health -= 0.005f;// temp
        // float health = getHealth from playerController
        healthBarUI.material.SetFloat(amountID, health);   

        int score = 12345; // temp
        // int score = 0; get score from score script
        scoreText.text = score.ToString();
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

