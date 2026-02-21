using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
public class UIManager : MonoBehaviour
{
    // [SerializeField] private GunPlayerController gunPlayerController;
    // [SerializeField] private SwordPlayerController swordPlayerController;
    [SerializeField] private Image gunPlayerCooldownUI;
    [SerializeField] private Image swordPlayerCooldownUI;
    [SerializeField] private Image healthBarUI;

    // temp
    float grenadeCooldown = 1.0f;
    float swordCooldown = 1.0f;
    float health = 1.0f;

    private readonly int amountLeftID = Shader.PropertyToID("_AmountLeft");
    

    void Awake()
    {
        // Assert.IsNotNull(gunPlayerController);
        // Assert.IsNotNull(swordPlayerController);
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
        grenadeCooldown -= 0.01f; //temp
        // float grenadeCooldown = gunPlayerController.GetCooldownPercentage();
        gunPlayerCooldownUI.material.SetFloat(amountLeftID, grenadeCooldown);

       // Assuming swordPlayerController has a getter like: public float GetCooldownPercentage()
        swordCooldown -= 0.01f; //temp
        // float swordCooldown = swordPlayerController.GetCooldownPercentage();
        swordPlayerCooldownUI.material.SetFloat(amountLeftID, swordCooldown);

        health -= 0.005f;// temp
        // float health = getHealth from player 
        healthBarUI.material.SetFloat(amountLeftID, health);

        // also get multipliers when ready
        
    }
}
