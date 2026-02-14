using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class GunPlayerController : PlayerController
{
    private Holster holster;

    private enum HoldingState
    {
        Released,
        FirstFrame,
        Held
    }

    private HoldingState holdingPrimary = HoldingState.Released;

    protected override void Awake()
    {
        base.Awake();

        holster = GetComponentInChildren<Holster>();
        Assert.IsNotNull(holster);
    }

    protected override void Start()
    {
        base.Start();
        playerInput.actions["Fire"].performed += PressFire;
        playerInput.actions["Fire"].canceled += ReleaseFire;
        playerInput.actions["UpEffect"].performed += ToggleUp;
    }

    private void Update()
    {
        if (holdingPrimary == HoldingState.FirstFrame)
            holdingPrimary = HoldingState.Held;
        else if (holdingPrimary == HoldingState.Held)
            holster.HoldPrimary();
    }

    private void PressFire(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            holster.FirePrimary();
            holdingPrimary = HoldingState.FirstFrame;
        }
    }

    private void ReleaseFire(InputAction.CallbackContext ctx)
    {
        holdingPrimary = HoldingState.Released;
        if (ctx.performed)
            holster.ReleasePrimary();
    }

    private void ToggleUp(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            holster.ToggleUp();
    }
}