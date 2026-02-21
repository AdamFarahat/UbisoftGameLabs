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

        Assert.IsNotNull(gunPlayerCooldownUI.material);
        Assert.IsNotNull(swordPlayerCooldownUI.material);
        Assert.IsNotNull(healthBarUI.material);
    }

    void Start()
    {
        // Create new instances so as not to change the original mats
        gunPlayerCooldownUI.material = new Material(gunPlayerCooldownUI.material);
        swordPlayerCooldownUI.material = new Material(swordPlayerCooldownUI.material);
        healthBarUI.material = new Material(healthBarUI.material);
    }

    // Update is called once per frame
    void Update()
    {
        // Update the materials

        if (gunPlayerController != null)
        {
            float grenadeCooldown = gunPlayerController.GetCooldownPercent();
            gunPlayerCooldownUI.material.SetFloat(amountLeftID, grenadeCooldown);
            
        }

        if (swordPlayerController != null)
        {   
            float swordCooldown = swordPlayerController.GetCooldownPercent();
            swordPlayerCooldownUI.material.SetFloat(amountLeftID, swordCooldown);    
        }

        health -= 0.005f;// temp
        // float health = getHealth from playerController
        healthBarUI.material.SetFloat(amountLeftID, health);

        // also get multipliers from gunPlayerController and swordPlayerController when ready
        
    }
}
