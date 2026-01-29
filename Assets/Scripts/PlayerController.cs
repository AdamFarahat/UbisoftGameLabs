using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] private float speed = 5.0f;

    [SerializeField] GameObject projectilePrefab;
    private Vector3 direction;

    private PlayerInput playerInput;

    private Rigidbody2D rb;

    private Collider2D coll;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Setup Player Inputs
        playerInput = gameObject.GetComponent<PlayerInput>();
        playerInput.actions["Move"].performed += ctx => Move(ctx);
        playerInput.actions["Move"].canceled += ctx => StopMove(ctx);
        playerInput.actions["Shoot"].performed += _ => Shoot();

        //Setup Rigidbody and Collider
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
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

    void Shoot()
    {
        //Spawns the projectile and sets its direction
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        //If no direction, shoot right by default
        if(direction == Vector3.zero)
        {
            projectileScript.SetDirection(Vector2.right);
        }
        else
        {
            projectileScript.SetDirection(direction);
        }
        
    }
}
