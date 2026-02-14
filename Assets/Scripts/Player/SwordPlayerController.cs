using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordPlayerController : PlayerController
{
    [Header("Jumping")]
    [SerializeField] private float jumpSpeed = 100f;
    [SerializeField] private float fallAcceleration = 500f;
    [SerializeField] private float attackDuration = 0.5f;

    private enum SwordPlayerStates
    {
        Normal,
        Stunned,
        Attacking,
        Parrying,
        Blocking
    }

    private SwordPlayerStates state = SwordPlayerStates.Normal;

    private Coroutine jumpRoutine = null;

    private Coroutine attackRoutine = null;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        playerInput.actions["UpEffect"].performed += Jump;
        playerInput.actions["DownEffect"].performed += Duck;
        playerInput.actions["Attack"].performed += Attack;
    }

    public bool IsAttacking()
    {
        return state == SwordPlayerStates.Attacking;
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

    private void Attack(InputAction.CallbackContext ctx)
    {
        if(state == SwordPlayerStates.Normal && ctx.performed)
        {
            //TODO trigger animation state change to Attacking
            gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
            state = SwordPlayerStates.Attacking;
            IEnumerator Routine()
            {
                yield return new WaitForSeconds(attackDuration);
                //TODO trigger animation state change to Normal
                gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
                state = SwordPlayerStates.Normal;
                attackRoutine = null;
            }
            attackRoutine = StartCoroutine(Routine());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            if(IsAttacking())
            {
                other.GetComponentInParent<DemoEnemy>().Death();
            }
        }
    }
}