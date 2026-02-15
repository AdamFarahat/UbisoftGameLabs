using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordPlayerController : PlayerController
{
    [Header("Jumping")]
    [SerializeField] private float jumpSpeed = 100f;
    [SerializeField] private float fallAcceleration = 500f;
    [SerializeField] private float attackDuration = 0.5f;

    [SerializeField] private float parryWindow = 0.5f;

    private float parryTimer = 0f;

    bool canBlock = true;

    [Header("Parrying")]
    [SerializeField] private float parryBulletMultiplier = 2.0f;
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

    private Coroutine parryRoutine = null;

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
        playerInput.actions["Attack"].performed += Attack;
        playerInput.actions["Block/Parry"].started += Block;
        playerInput.actions["Block/Parry"].canceled += CancelBlock;
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

    public void Block(InputAction.CallbackContext ctx)
    {
        Debug.Log("Block/Parry");
        if(canBlock && state == SwordPlayerStates.Normal)
        {
            parryRoutine = StartCoroutine(ParryWindow());
        }
        else
        {
            Debug.Log("Block on cooldown");
        }
        
    }

    public void CancelBlock(InputAction.CallbackContext ctx)
    {
        Debug.Log("Cancel Block");
        if (ctx.canceled)
        {
            if(parryRoutine != null)
            {
                StopCoroutine(parryRoutine);
                parryRoutine = null;
            }
            state = SwordPlayerStates.Normal;
            //Trigger animation state change to Normal
            GetComponentInChildren<MeshRenderer>().material.color = Color.white;
        }
    }

    private IEnumerator ParryWindow()
    {
        parryTimer = 0f;
        state = SwordPlayerStates.Parrying;
        GetComponentInChildren<MeshRenderer>().material.color = Color.green;
        while(parryTimer < parryWindow)
        {
            parryTimer += Time.deltaTime;
            yield return null;
        }
        if(state == SwordPlayerStates.Parrying)
        {
            state = SwordPlayerStates.Blocking;
            GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
        }
    }

    private IEnumerator BlockCooldown()
    {
        canBlock = false;
        yield return new WaitForSeconds(3f);
        canBlock = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Projectile projectile;
        if (other.CompareTag("Enemy"))
        {
            if (state == SwordPlayerStates.Attacking || state == SwordPlayerStates.Parrying)
            {
                other.GetComponentInParent<DemoEnemy>().Death();
                if (state == SwordPlayerStates.Parrying)
                {
                    parryTimer = 0f;
                }

            }
            else if (state == SwordPlayerStates.Blocking)
            {
                other.GetComponentInParent<DemoEnemy>().Death();
                StartCoroutine(BlockCooldown());
            }
        }
        else if (other.TryGetComponent<Projectile>(out projectile)) {
            if (state == SwordPlayerStates.Parrying) {
                Vector3 dir = (projectile.owner.transform.position - transform.position).normalized;
                projectile.Initialize(dir);
                projectile.speed = projectile.speed * parryBulletMultiplier;
                parryTimer = 0f;
            }
        }
    }
}