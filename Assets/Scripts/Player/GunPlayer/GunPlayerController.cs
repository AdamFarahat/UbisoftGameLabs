using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

public class GunPlayerController : PlayerController
{
    private Holster holster;

    protected override void Awake()
    {
        base.Awake();

        holster = GetComponentInChildren<Holster>();
        Assert.IsNotNull(holster);
    }

    protected override void Start()
    {
        base.Start();
        playerInput.actions["Fire"].performed += Fire;
        playerInput.actions["UpEffect"].performed += ToggleUp;
    }

    private void Fire(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            holster.Fire();
    }

    private void ToggleUp(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            holster.ToggleUp();
    }
}