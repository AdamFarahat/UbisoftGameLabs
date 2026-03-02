using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordPlayerController : PlayerController
{
    private static SwordPlayerController instance = null;
    public static SwordPlayerController Instance => instance;
    public static float LaneIndex => instance ? instance.GetLaneIndex() : -1f;

    private SwordHitBox swordHitBox;
    [Header("Stunning")]
    [SerializeField] private float stunCooldown = 1f;
    
    [Header("Jumping")]
    [SerializeField] private float jumpSpeed = 100f;
    [SerializeField] private float fallAcceleration = 500f;
    [SerializeField] private float attackDuration = 0.5f;

    private float parryTimer = 0f;

    bool canBlock = true;
    [SerializeField] private float blockCooldown = 3f;
    private float blockCooldownPercent = 0f;

    [Header("Parrying")]
    [SerializeField] private float parryBulletMultiplier = 2.0f;
    [SerializeField] private float parryWindow = 0.5f;
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

    private Coroutine stunRoutine = null;

    protected override void Awake()
    {
        instance = this;
        base.Awake();
        swordHitBox = FindFirstObjectByType<SwordHitBox>();
    }

    protected override void Start()
    {
        base.Start();
        playerInput.actions["UpEffect"].performed += Jump;
        playerInput.actions["DownEffect"].performed += Duck;
        playerInput.actions["Attack"].performed += Attack;
        playerInput.actions["Block/Parry"].started += Block;
        playerInput.actions["Block/Parry"].canceled += CancelBlock;
    }

    public override float GetCooldownPercent()
    {
        return blockCooldownPercent;
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
            swordHitBox.gameObject.SetActive(true);
            IEnumerator Routine()
            {
                yield return new WaitForSeconds(attackDuration);
                //TODO trigger animation state change to Normal
                gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
                state = SwordPlayerStates.Normal;
                attackRoutine = null;
                swordHitBox.gameObject.SetActive(false);
            }
            attackRoutine = StartCoroutine(Routine());
        }
    }

    public void Block(InputAction.CallbackContext ctx)
    {
        Debug.Log("Block/Parry");
        if(canBlock && state == SwordPlayerStates.Normal)
        {
            swordHitBox.gameObject.SetActive(true);
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
        if (state != SwordPlayerStates.Stunned)
        {
            if(parryRoutine != null)
            {
                StopCoroutine(parryRoutine);
                parryRoutine = null;
            }
            state = SwordPlayerStates.Normal;
            //Trigger animation state change to Normal
            swordHitBox.gameObject.SetActive(false);
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
            if(state != SwordPlayerStates.Parrying)
            {
                yield break;
            }
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
        blockCooldownPercent = 1f;
        CancelBlock(new InputAction.CallbackContext());
        for (float t = blockCooldown; t >= 0f; t -= Time.deltaTime)
        {
            blockCooldownPercent = Mathf.Clamp01(t / blockCooldown);
            yield return null;
        }
        blockCooldownPercent = 0f;
        canBlock = true;
    }

    private IEnumerator StunCooldown()
    {
        yield return new WaitForSeconds(stunCooldown);
        state = SwordPlayerStates.Normal;
        GetComponentInChildren<MeshRenderer>().material.color = Color.white;
    }

    private void Stun()
    {
        if(state != SwordPlayerStates.Stunned)
        {
            state = SwordPlayerStates.Stunned;
            GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
            IEnumerator Routine()
            {
                yield return new WaitForSeconds(2f);
                state = SwordPlayerStates.Normal;
                GetComponentInChildren<MeshRenderer>().material.color = Color.white;
            }
            swordHitBox.gameObject.SetActive(false);
            stunRoutine = StartCoroutine(Routine());
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.GetComponentInParent<Enemy>() || collider.GetComponent<Projectile>() != null)
        {
            Stun();
        }
    }

    public void OnSwordHitBoxTriggerEnter(Collider collider)
    {
        Enemy enemy = collider.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            if (state == SwordPlayerStates.Attacking || state == SwordPlayerStates.Parrying)
            {
                if (enemy.OnParried())
                {
                    // TODO score + multiplier gain
                    playerStats.AddSwordSuper(5f);
                }
                if (state == SwordPlayerStates.Parrying)
                {
                    parryTimer = 0f;
                }

            }
            else if (state == SwordPlayerStates.Blocking && canBlock)
            {
                if (enemy.OnParried())
                {
                    // TODO score + multiplier gain
                    playerStats.AddSwordSuper(2f);
                }
                StartCoroutine(BlockCooldown());
            }
        }
        else if (collider.TryGetComponent(out Projectile projectile))
        {
            if (state == SwordPlayerStates.Parrying)
            {
                ReflectBackBullet(projectile);
                parryTimer = 0f;
                playerStats.AddSwordSuper(5f);
            }
            else if (state == SwordPlayerStates.Blocking && canBlock)
            {
                ReflectBackBullet(projectile);
                playerStats.AddSwordSuper(2f);
                StartCoroutine(BlockCooldown());
            }
        }
    }

    private void ReflectBackBullet(Projectile projectile)
    {
        projectile.FlipDirection();
        projectile.speed *= parryBulletMultiplier;
    }
}
