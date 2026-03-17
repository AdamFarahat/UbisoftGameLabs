using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class UITester : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Material healthMaterial;
    [SerializeField] private Material powerBarMaterial;
    [SerializeField] private Material gunCooldownMaterial;
    [SerializeField] private Material gunMultiplierMaterial;
    [SerializeField] private Material swordCooldownMaterial;
    [SerializeField] private Material swordMultiplierMaterial;
    private readonly int amountID = Shader.PropertyToID("_Amount");
    private readonly int leftAmountID = Shader.PropertyToID("_LeftAmount");
    private readonly int rightAmountID = Shader.PropertyToID("_RightAmount");

    [Header("Test UI (modifies visuals only)")]
    [Tooltip("Percentage remaining of player health")]
    [Range(0f, 1f)]
    [SerializeField] private float healthPercent;

    [Tooltip("Current score to display on UI")]
    [SerializeField] private int score;

    [Header("Gun Player UI")]

    [Tooltip("Current percentage of player gun power bar")]
    [Range(0f, 1f)]
    [SerializeField] private float gunPowerBarPercent;

    [Tooltip("Percentage left on gun player's cooldown")]
    [Range(0f, 1f)]
    [SerializeField] private float gunCooldownPercent;

    [Tooltip("Converted multiplier UI value for gun player (x1: 0.0 => 0.0, x2: 0.25 => 0.2, x4: 0.5 => 0.42, x8: 0.75 => 0.67, x16: 1.0 => 1.0)")]
    [Range(0f, 1f)]
    [SerializeField] private float gunMultiplierPercent;

    [Header("Sword Player UI")]

    [Tooltip("Current percentage of player sword power bar")]
    [Range(0f, 1f)]
    [SerializeField] private float swordPowerBarPercent;

    [Tooltip("Percentage left on sword player's cooldown")]
    [Range(0f, 1f)]
    [SerializeField] private float swordCooldownPercent;

    [Tooltip("Converted multiplier UI value for sword player (x1: 0.0 => 0.0, x2: 0.25 => 0.2, x4: 0.5 => 0.42, x8: 0.75 => 0.67, x16: 1.0 => 1.0)")]
    [Range(0f, 1f)]
    [SerializeField] private float swordMultiplierPercent;

    void Awake()
    {
        Assert.IsNotNull(healthMaterial);
        Assert.IsNotNull(scoreText);
        Assert.IsNotNull(powerBarMaterial);
        Assert.IsNotNull(gunCooldownMaterial);
        Assert.IsNotNull(gunMultiplierMaterial);
        Assert.IsNotNull(swordCooldownMaterial);
        Assert.IsNotNull(swordMultiplierMaterial);
    }

    private void OnValidate()
    {
        if (healthMaterial != null)
            healthMaterial.SetFloat(amountID, healthPercent);
        if (scoreText != null)
            scoreText.text = score.ToString();
        if (powerBarMaterial != null)
        {
            powerBarMaterial.SetFloat(leftAmountID, gunPowerBarPercent);
            powerBarMaterial.SetFloat(rightAmountID, swordPowerBarPercent);
        }
        if (gunCooldownMaterial != null)
            gunCooldownMaterial.SetFloat(amountID, gunCooldownPercent);
        if (gunMultiplierMaterial != null)
            gunMultiplierMaterial.SetFloat(amountID, gunMultiplierPercent);
        if (swordCooldownMaterial != null)
            swordCooldownMaterial.SetFloat(amountID, swordCooldownPercent);
        if (swordMultiplierMaterial != null)
            swordMultiplierMaterial.SetFloat(amountID, swordMultiplierPercent);
    }
}
