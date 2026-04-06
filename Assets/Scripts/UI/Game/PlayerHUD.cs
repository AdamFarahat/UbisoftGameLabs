using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerHUD : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image multiplierUI;
    [SerializeField] private Image cooldownUI;
    [SerializeField] private const float SUPER_LERP_DURATION = 0.5f;

    // Shader Property IDs
    private readonly int amountID = Shader.PropertyToID("_Amount");
    private readonly int isSuperID = Shader.PropertyToID("_IsSuper");

    // Animation Tweens
    private Tween fillTween;
    private Tween colorTween;

    public void Initialize()
    {
        if (multiplierUI != null)
            multiplierUI.material = new Material(multiplierUI.material);

        if (cooldownUI != null)
        {
            cooldownUI.material = new Material(cooldownUI.material);
            Settings.LoadUIMaterialIfNotProcessed(cooldownUI.material, "_Emission");
        }
    }

    public void UpdateCooldown(float cooldownPercent)
    {
        if (cooldownUI != null)
            cooldownUI.material.SetFloat(amountID, cooldownPercent);
    }

    public void TriggerCooldownPulse()
    {
        if (cooldownUI != null)
            cooldownUI.material.SetFloat("_TimeHitZero", Time.time);
    }

    public void UpdateMultiplier(float normalizedMultiplier, bool isSuperActive)
    {
        if (isSuperActive || multiplierUI == null) return;

        float targetValue = ConvertMultiplierToUIValue(normalizedMultiplier);

        fillTween?.Kill();
        fillTween = multiplierUI.material.DOFloat(targetValue, amountID, SUPER_LERP_DURATION)
            .SetEase(Ease.OutBack)
            .OnUpdate(() => multiplierUI.SetMaterialDirty());
    }

    public void PlaySuperStartAnimation()
    {
        if (multiplierUI == null) return;

        // Lerp Color to glowing super state
        colorTween?.Kill();
        colorTween = multiplierUI.material.DOFloat(1f, isSuperID, SUPER_LERP_DURATION).OnUpdate(() => multiplierUI.SetMaterialDirty());
        
        fillTween?.Kill();
        fillTween = multiplierUI.material.DOFloat(1f, amountID, SUPER_LERP_DURATION).SetEase(Ease.Linear).OnUpdate(() => multiplierUI.SetMaterialDirty());
    }

    public void PlaySuperEndAnimation(float normalizedMultiplier)
    {
        if (multiplierUI == null) return;

        // Lerp Color back to normal
        colorTween?.Kill();
        colorTween = multiplierUI.material.DOFloat(0f, isSuperID, SUPER_LERP_DURATION).OnUpdate(() => multiplierUI.SetMaterialDirty());

        // Return the bar to its multiplier fill
        UpdateMultiplier(normalizedMultiplier, false);
    }

    private float ConvertMultiplierToUIValue(float value)
    {
        switch (value)
        {
            case 0.0f: return 0.0f;  // x1
            case 0.25f: return 0.2f; // x2
            case 0.5f: return 0.42f; // x4
            case 0.75f: return 0.67f;// x6
            case 1.0f: return 1.00f; // x8
            default:
                Debug.LogWarning($"Unexpected multiplier value: {value}. Using raw value.");
                return value;
        }
    }

    private void OnDestroy()
    {
        fillTween?.Kill();
        colorTween?.Kill();
    }
}