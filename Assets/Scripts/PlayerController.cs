using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;

    private LaneBound laneBound;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        laneBound = GetComponent<LaneBound>();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput.SwitchCurrentActionMap("Player");
        playerInput.actions["MoveLeft"].performed += OnMoveLeft;
        playerInput.actions["MoveRight"].performed += OnMoveRight;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMoveLeft(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("Move Left");
            laneBound.LaneIndex--;
        }
    }

    private void OnMoveRight(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("Move Right");
            laneBound.LaneIndex++;
        }
    }

}
