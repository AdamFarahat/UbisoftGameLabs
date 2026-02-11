using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    protected PlayerInput playerInput;
    protected LaneBound laneBound;
    protected virtual void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        laneBound = GetComponent<LaneBound>();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        playerInput.SwitchCurrentActionMap("Player");
        playerInput.actions["MoveLeft"].performed += OnMoveLeft;
        playerInput.actions["MoveRight"].performed += OnMoveRight;
    }

    private void OnMoveLeft(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            laneBound.LaneIndex--;
        }
    }

    private void OnMoveRight(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            laneBound.LaneIndex++;
        }
    }

}
