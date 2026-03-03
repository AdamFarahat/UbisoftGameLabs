using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class GunPlayerController : PlayerController
{
    private static GunPlayerController instance = null;
    public static GunPlayerController Instance => instance;
    public static float LaneIndex => instance ? instance.GetLaneIndex() : -1f;

    [Header("Scoring")]
    [SerializeField] private float gunKillMultiplierGain = 0.05f;
    [SerializeField] private float grenadeKillMultiplierGain = 0.5f;

    public float GunKillMultiplierGain => gunKillMultiplierGain;
    public float GrenadeKillMultiplierGain => grenadeKillMultiplierGain;

    private Holster holster;
    private GrenadeBelt grenadeBelt;

    private enum HoldingState
    {
        Released,
        FirstFrame,
        Held
    }

    private HoldingState holdingGunInput = HoldingState.Released;

    public Action OnGrenadeCooldownReady;

    protected override void Awake()
    {
        instance = this;
        base.Awake();

        holster = GetComponentInChildren<Holster>();
        Assert.IsNotNull(holster);
        grenadeBelt = GetComponentInChildren<GrenadeBelt>();
        Assert.IsNotNull(grenadeBelt);
    }

    protected override void Start()
    {
        base.Start();
        playerInput.actions["Fire"].performed += PressFire;
        playerInput.actions["Fire"].canceled += ReleaseFire;
        playerInput.actions["UpEffect"].performed += ToggleGunUp;
        playerInput.actions["DownEffect"].performed += ToggleGunDown;
        playerInput.actions["Throw"].performed += PressThrow;
        playerInput.actions["Throw"].canceled += ReleaseThrow;

        grenadeBelt.OnCooldownReady += HandleGrenadeReady;
    }

    private void Update()
    {
        if (holdingGunInput == HoldingState.FirstFrame)
            holdingGunInput = HoldingState.Held;
        else if (holdingGunInput == HoldingState.Held)
            holster.KeepFiring();
    }

    private void PressFire(InputAction.CallbackContext ctx)
    {
        holster.StartFiring();
        holdingGunInput = HoldingState.FirstFrame;
        grenadeBelt.CancelThrow();
    }

    private void ReleaseFire(InputAction.CallbackContext ctx)
    {
        holdingGunInput = HoldingState.Released;
        holster.StopFiring();
    }

    private void ToggleGunUp(InputAction.CallbackContext ctx)
    {
        holster.ToggleUp();
    }

    private void ToggleGunDown(InputAction.CallbackContext ctx)
    {
        holster.ToggleDown();
    }

    private void PressThrow(InputAction.CallbackContext ctx)
    {
        grenadeBelt.ChargeThrow();
        if (holdingGunInput != HoldingState.Released)
        {
            holdingGunInput = HoldingState.Released;
            holster.StopFiring();
        }
    }

    private void ReleaseThrow(InputAction.CallbackContext ctx)
    {
        grenadeBelt.Throw();
    }

    public override float GetCooldownPercent()
    {
        return grenadeBelt.GetCooldownPercent();
    }

    private void OnDestroy()
    {
        if (grenadeBelt != null)
            grenadeBelt.OnCooldownReady -= HandleGrenadeReady;
    }

    private void HandleGrenadeReady()
    {
        OnGrenadeCooldownReady?.Invoke();
    }
}