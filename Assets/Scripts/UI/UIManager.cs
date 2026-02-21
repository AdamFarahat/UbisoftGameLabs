using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
public class UIManager : MonoBehaviour
{
    // [SerializeField] private GrenadeBelt grenadeBelt;
    // [SerializeField] private SwordPlayerController swordPlayerController;
    [SerializeField] private Image gunPlayerCooldownUI;
    [SerializeField] private Image swordPlayerCooldownUI;

    // temp
    float grenadeCooldown = 1.0f;
    float swordCooldown = 1.0f;

    private readonly int amountLeftID = Shader.PropertyToID("_AmountLeft");
    

    void Awake()
    {
        // Assert.IsNotNull(grenadeBelt);
        // Assert.IsNotNull(swordPlayerController);
        Assert.IsNotNull(gunPlayerCooldownUI);
        Assert.IsNotNull(swordPlayerCooldownUI);
        Assert.IsNotNull(gunPlayerCooldownUI.material);
        Assert.IsNotNull(swordPlayerCooldownUI.material);
    }

    // Update is called once per frame
    void Update()
    {
        grenadeCooldown -= 0.01f; //temp
        // float grenadeCooldown = grenadeBelt.GetCooldownPercentage();
        gunPlayerCooldownUI.material.SetFloat(amountLeftID, grenadeCooldown);

       // Assuming swordPlayerController has a getter like: public float GetCooldownPercentage()
        swordCooldown -= 0.01f; //temp
        // float swordCooldown = swordPlayerController.GetCooldownPercentage();
        swordPlayerCooldownUI.material.SetFloat(amountLeftID, swordCooldown);
        
    }
}
