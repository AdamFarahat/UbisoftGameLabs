using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordPlayerController : PlayerController
{
    [Header("Jumping")]
    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private AnimationCurve fallCurve;

    private bool jumping = false;

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
            if (jumping)
                return;

            void SetY(float y)
            {
                transform.position = new(transform.position.x, y, transform.position.z);
            }

            IEnumerator Routine()
            {
                jumping = true;
                SetY(0f);

                // Animate jump
                for (float t = 0f; t < jumpCurve.keys.Last().time; t += Time.deltaTime)
                {
                    SetY(jumpCurve.Evaluate(t));
                    yield return null;
                }

                // Animate fall
                for (float t = 0f; t < fallCurve.keys.Last().time; t += Time.deltaTime)
                {
                    SetY(fallCurve.Evaluate(t));
                    yield return null;
                }

                SetY(0f);
                jumping = false;
            }

            StartCoroutine(Routine());
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