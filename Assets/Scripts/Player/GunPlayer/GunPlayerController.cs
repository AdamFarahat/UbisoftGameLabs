using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class GunPlayerController : PlayerController
{
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
            holster.HoldInput();
    }

    private void PressFire(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            holster.Fire();
            holdingGunInput = HoldingState.FirstFrame;
        }
    }

    private void ReleaseFire(InputAction.CallbackContext ctx)
    {
        holdingGunInput = HoldingState.Released;
        if (ctx.performed)
            holster.ReleaseInput();
    }

    private void ToggleGunUp(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            holster.ToggleUp();
    }

    private void ToggleGunDown(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            holster.ToggleDown();
    }

    private void PressThrow(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            grenadeBelt.ChargeThrow();
    }

    private void ReleaseThrow(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            grenadeBelt.Throw();
    }
}