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
    private HoldingState holdingGrenadeInput = HoldingState.Released;

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
        playerInput.actions["UpEffect"].performed += ToggleGun;
        playerInput.actions["Throw"].performed += PressThrow;
        playerInput.actions["Throw"].canceled += ReleaseThrow;
        playerInput.actions["DownEffect"].performed += ToggleGrenade;
    }

    private void Update()
    {
        if (holdingGunInput == HoldingState.FirstFrame)
            holdingGunInput = HoldingState.Held;
        else if (holdingGunInput == HoldingState.Held)
            holster.HoldInput();

        if (holdingGrenadeInput == HoldingState.FirstFrame)
            holdingGrenadeInput = HoldingState.Held;
        else if (holdingGrenadeInput == HoldingState.Held)
            grenadeBelt.HoldInput();
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

    private void ToggleGun(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            holster.Toggle();
    }

    private void PressThrow(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            grenadeBelt.Throw();
            holdingGrenadeInput = HoldingState.FirstFrame;
        }
    }

    private void ReleaseThrow(InputAction.CallbackContext ctx)
    {
        holdingGrenadeInput = HoldingState.Released;
        if (ctx.performed)
            grenadeBelt.ReleaseInput();
    }

    private void ToggleGrenade(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            grenadeBelt.Toggle();
    }
}