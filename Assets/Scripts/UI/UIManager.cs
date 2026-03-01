using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
public class UIManager : MonoBehaviour
{
    private GunPlayerController gunPlayerController;
    private SwordPlayerController swordPlayerController;
    [SerializeField] private Image gunPlayerCooldownUI;
    [SerializeField] private Image swordPlayerCooldownUI;
    [SerializeField] private Image healthBarUI;
    [SerializeField] private Image gunMultiplierUI;
    [SerializeField] private Image swordMultiplierUI;

    // temp
    float health = 1.0f;

    private readonly int amountLeftID = Shader.PropertyToID("_AmountLeft");
    

    void Awake()
    {
        gunPlayerController = GameObject.FindFirstObjectByType<GunPlayerController>();
        swordPlayerController = GameObject.FindFirstObjectByType<SwordPlayerController>();
        Assert.IsNotNull(gunPlayerCooldownUI);
        Assert.IsNotNull(swordPlayerCooldownUI);
        Assert.IsNotNull(healthBarUI);
        Assert.IsNotNull(gunMultiplierUI);
        Assert.IsNotNull(swordMultiplierUI);

        Assert.IsNotNull(gunPlayerCooldownUI.material);
        Assert.IsNotNull(swordPlayerCooldownUI.material);
        Assert.IsNotNull(healthBarUI.material);
        Assert.IsNotNull(gunMultiplierUI.material);
        Assert.IsNotNull(swordMultiplierUI.material);
    }

    void Start()
    {
        // Create new instances so as not to change the original mats
        gunPlayerCooldownUI.material = new Material(gunPlayerCooldownUI.material);
        swordPlayerCooldownUI.material = new Material(swordPlayerCooldownUI.material);
        healthBarUI.material = new Material(healthBarUI.material);
        gunMultiplierUI.material = new Material(gunMultiplierUI.material);
        swordMultiplierUI.material = new Material(swordMultiplierUI.material);

        
    }

    // Update is called once per frame
    void Update()
    {
        // Update the materials

        if (gunPlayerController != null)
        {
            float grenadeCooldown = gunPlayerController.GetCooldownPercent();
            gunPlayerCooldownUI.material.SetFloat(amountLeftID, grenadeCooldown);
            // float gunMultiplier = gunPlayerController.GetMultiplier();
            float gunMultiplier = 0.8f; // temp        
            gunMultiplierUI.material.SetFloat(amountLeftID, ConvertMultiplierToUIValue(gunMultiplier));
        }

        if (swordPlayerController != null)
        {   
            float swordCooldown = swordPlayerController.GetCooldownPercent();
            swordPlayerCooldownUI.material.SetFloat(amountLeftID, swordCooldown);    
            // float swordMultiplier = swordPlayerController.GetMultiplier();
            float swordMultiplier = 0.2f; // temp
            swordMultiplierUI.material.SetFloat(amountLeftID, ConvertMultiplierToUIValue(swordMultiplier));
        }

        health -= 0.005f;// temp
        // float health = getHealth from playerController
        healthBarUI.material.SetFloat(amountLeftID, health);       
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

