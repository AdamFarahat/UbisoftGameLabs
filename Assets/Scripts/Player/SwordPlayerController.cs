using UnityEngine;
using UnityEngine.InputSystem;

public class SwordPlayerController : PlayerController
{
    Rigidbody rb;
    
    Collider playerCollider;
    [SerializeField] private float jumpForce = 5f;
    private bool isGrounded = true;

    public bool IsGrounded
    {
        get => isGrounded;
        set => isGrounded = value;
    }
    
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponentInChildren<Rigidbody>();
        playerCollider = GetComponentInChildren<Collider>();
    }

    protected override void Start()
    {
        base.Start();
        playerInput.actions["UpEffect"].performed += Jump;
        playerInput.actions["DownEffect"].performed += Duck;
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            //If the player is on the ground, allow them to jump.
            if (!isGrounded) return;
            //Have the player jump upwards and then fall back down to the ground.
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            IsGrounded = false;
        }
    }

    private void Duck(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("Duck");
        }
    }
}