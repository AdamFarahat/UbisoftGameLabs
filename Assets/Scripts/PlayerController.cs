using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField]float speed = 5.0f;
    Vector3 direction;

    PlayerInput playerInput;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput = gameObject.GetComponent<PlayerInput>();
        playerInput.actions["Move"].performed += ctx => Move(ctx);
        playerInput.actions["Move"].canceled += ctx => StopMove(ctx);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Move(InputAction.CallbackContext ctx)
    {
        Vector2 inputVector = ctx.ReadValue<Vector2>();
        direction = new Vector3(inputVector.x, inputVector.y, 0);
        Debug.Log("Move: " + direction);
    }

    void StopMove(InputAction.CallbackContext ctx)
    {
        direction = Vector3.zero;
    }

    void FixedUpdate()
    {
        transform.Translate(direction * speed * Time.fixedDeltaTime);
    }
}
