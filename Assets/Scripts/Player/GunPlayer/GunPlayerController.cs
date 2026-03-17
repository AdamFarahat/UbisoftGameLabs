using System;
using System.Collections;
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

    [Header ("Super")]
    [SerializeField] private float activateSuperWaitTime = 0.1f;
    private bool fireButtonPressedSuper = false;
    private bool grenadeButtonPressedSuper = false;

    Coroutine resetFireButtonPressedSuperCoroutine = null;
    Coroutine resetThrowButtonPressedSuperCoroutine = null;

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
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        playerInput.actions["Fire"].performed += PressFire;
        playerInput.actions["Fire"].canceled += ReleaseFire;
        playerInput.actions["UpEffect"].performed += ToggleGunUp;
        playerInput.actions["DownEffect"].performed += ToggleGunDown;
        playerInput.actions["Throw"].performed += PressThrow;
        playerInput.actions["Throw"].canceled += ReleaseThrow;
        grenadeBelt.OnCooldownReady += HandleGrenadeReady;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        playerInput.actions["Fire"].performed -= PressFire;
        playerInput.actions["Fire"].canceled -= ReleaseFire;
        playerInput.actions["UpEffect"].performed -= ToggleGunUp;
        playerInput.actions["DownEffect"].performed -= ToggleGunDown;
        playerInput.actions["Throw"].performed -= PressThrow;
        playerInput.actions["Throw"].canceled -= ReleaseThrow;
    }

    private void Update()
    {
        if (Stunned)
            return;

        if (holdingGunInput == HoldingState.FirstFrame)
            holdingGunInput = HoldingState.Held;
        else if (holdingGunInput == HoldingState.Held)
            holster.KeepFiring();
    }

    private void PressFire(InputAction.CallbackContext ctx)
    {
        if (Stunned)
            return;

        if (PlayerStats.Instance.GetGunSuperPercent() >= 1f && !PlayerStats.Instance.IsSuperActive())
        {
            Debug.Log("Fire button pressed with super ready");
            //Set fire button pressed super to true
            fireButtonPressedSuper = true;
            if (grenadeButtonPressedSuper && !PlayerStats.Instance.IsSuperActive())
            {
                Debug.Log("Gun Player Activating Super Fire!");
                PlayerStats.Instance.PrepareGunSuperReady(true);
                return;
            }
            resetFireButtonPressedSuperCoroutine = StartCoroutine(ResetFireButtonPressedSuper());
        }

        holster.StartFiring();
        holdingGunInput = HoldingState.FirstFrame;
        grenadeBelt.CancelThrow();
    }

    private void ReleaseFire(InputAction.CallbackContext ctx)
    {
        holdingGunInput = HoldingState.Released;
        if (Stunned)
            holster.CancelFiring();
        else
            holster.StopFiring();
    }

    private void ToggleGunUp(InputAction.CallbackContext ctx)
    {
        if (Stunned)
            return;

        holster.ToggleUp();
    }

    private void ToggleGunDown(InputAction.CallbackContext ctx)
    {
        if (Stunned)
            return;

        holster.ToggleDown();
    }

    private void PressThrow(InputAction.CallbackContext ctx)
    {
        if (Stunned)
            return;

        if (PlayerStats.Instance.GetGunSuperPercent() >= 1f && !PlayerStats.Instance.IsSuperActive())
        {
            Debug.Log("Grenade button pressed with super ready");
            //Set grenade button pressed super to true
            grenadeButtonPressedSuper = true;
            if (fireButtonPressedSuper && !PlayerStats.Instance.IsSuperActive())
            {
                Debug.Log("Gun Player Activating Super Grenade Throw!");
                //PlayerStats.Instance.ActivateSuper();
                return;
            }
            resetThrowButtonPressedSuperCoroutine = StartCoroutine(ResetThrowButtonPressedSuper());
        }

        grenadeBelt.ChargeThrow();
        if (holdingGunInput != HoldingState.Released)
        {
            holdingGunInput = HoldingState.Released;
            holster.StopFiring();
        }

    }

    private void ReleaseThrow(InputAction.CallbackContext ctx)
    {
        if (Stunned)
            grenadeBelt.CancelThrow();
        else
            grenadeBelt.Throw();
    }

    protected override void OnStunStart()
    {
        base.OnStunStart();
        holster.CancelFiring();
        grenadeBelt.CancelThrow();
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

    private IEnumerator ResetFireButtonPressedSuper()
    {
        yield return new WaitForSeconds(activateSuperWaitTime);
        fireButtonPressedSuper = false;
        resetFireButtonPressedSuperCoroutine = null;
    }

    private IEnumerator ResetThrowButtonPressedSuper()
    {
        yield return new WaitForSeconds(activateSuperWaitTime);
        grenadeButtonPressedSuper = false;
        resetThrowButtonPressedSuperCoroutine = null;
    }
}