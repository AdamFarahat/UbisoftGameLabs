using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordPlayerController : PlayerController
{
    [Header("Jumping")]
    [SerializeField] private float jumpSpeed = 100f;
    [SerializeField] private float fallAcceleration = 500f;

    private Coroutine jumpRoutine = null;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        playerInput.actions.Enable();
        playerInput.actions["UpEffect"].performed += Jump;
        playerInput.actions["DownEffect"].performed += Duck;
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            if (jumpRoutine != null)
                return;

            void SetY(float y)
            {
                transform.position = new(transform.position.x, y, transform.position.z);
            }

            IEnumerator Routine()
            {
                float y = 0f;
                SetY(y);
                float velocity = jumpSpeed;

                // Animate jump
                while (velocity > 0f)
                {
                    y += velocity * Time.deltaTime;
                    velocity = Mathf.Max(velocity - fallAcceleration * Time.deltaTime, 0f);
                    SetY(y);
                    yield return null;
                }

                // Animate fall
                while (y > 0f)
                {
                    y = Mathf.Max(y + velocity * Time.deltaTime, 0f);
                    velocity -= fallAcceleration * Time.deltaTime;
                    SetY(y);
                    yield return null;
                }

                y = 0f;
                SetY(y);
                jumpRoutine = null;
            }

            jumpRoutine = StartCoroutine(Routine());
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