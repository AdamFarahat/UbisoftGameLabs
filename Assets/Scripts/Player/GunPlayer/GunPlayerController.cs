using System;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.UI.GridLayoutGroup;

public class GunPlayerController : PlayerController
{
    private static GunPlayerController instance = null;
    public static GunPlayerController Instance => instance;
    public static float LaneIndex => instance ? instance.GetLaneIndex() : -1f;

    private Holster holster;
    private GrenadeBelt grenadeBelt;

    private enum HoldingState
    {
        Released,
        FirstFrame,
        Held
    }

    private HoldingState holdingGunInput = HoldingState.Released;

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
    }

    private void Update()
    {
        if (holdingGunInput == HoldingState.FirstFrame)
            holdingGunInput = HoldingState.Held;
        else if (holdingGunInput == HoldingState.Held)
            holster.KeepFiring(this);
    }

    private void PressFire(InputAction.CallbackContext ctx)
    {
        holster.StartFiring(this);
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
        grenadeBelt.Throw(this);
    }

    public override float GetCooldownPercent()
    {
        return grenadeBelt.GetCooldownPercent();
    }

    internal void UpdateScore(float multiplierGain, int scoreOfEnemy)
    {
        deltaMultiplierGain += multiplierGain;
        score = deltaMultiplierGain * scoreOfEnemy;

    }
}