using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class SwordPlayerController : PlayerController
{
    private static SwordPlayerController instance = null;
    public static SwordPlayerController Instance => instance;
    public static float LaneIndex => instance ? instance.GetLaneIndex() : -1f;

    [SerializeField] private SwordHitBox swordHitBox;

    [Header("Jumping")]
    [SerializeField] private float jumpSpeed = 100f;
    [SerializeField] private float fallAcceleration = 500f;
    [SerializeField] private float attackDuration = 0.5f;

    private float parryTimer = 0f;

    bool canBlock = true;
    [SerializeField] private float blockCooldown = 3f;
    private float blockCooldownPercent = 0f;
    public Action OnBlockCooldownReady;

    [Header("Parrying")]
    [SerializeField] private float parryBulletSpeedMult = 2.0f;
    [SerializeField] private float parryWindow = 0.5f;

    [Header("Scoring")]
    [SerializeField] private float blockingMultiplierGain = 0.2f;
    [SerializeField] private float attackingMultiplierGain = 0.6f;
    [SerializeField] private float meleeParryMultiplierGain = 0.8f;
    [SerializeField] private float bulletParryMultiplierGain = 0.6f;

    private SpriteAnimator animator;
    private Coroutine delayedAnimation = null;

    private enum SwordPlayerStates
    {
        Normal,
        Attacking,
        Parrying,
        Blocking
    }

    private SwordPlayerStates state = SwordPlayerStates.Normal;

    private Coroutine jumpRoutine = null;

    private Coroutine attackRoutine = null;

    private Coroutine parryRoutine = null;

    [Header("Super")]
    [SerializeField] private float activateSuperWaitTime = 0.1f;
    private bool attackButtonPressedSuper = false;
    private bool blockButtonPressedSuper = false;
    private Coroutine resetAttackButtonPressedSuperCoroutine = null;
    private Coroutine resetBlockButtonPressedSuperCoroutine = null;

    protected override void Awake()
    {
        instance = this;
        base.Awake();

        Assert.IsNotNull(swordHitBox);
        animator = GetComponent<SpriteAnimator>();
        Assert.IsNotNull(animator);
        animator.SetAnimationDuration("Attack", attackDuration);

        LaneBound laneBound = GetComponent<LaneBound>();
        Assert.IsNotNull(laneBound);
        laneBound.DashStart += OnDashStart;
        laneBound.DashEnd += OnDashEnd;

        if (PlayerSelect.swordPlayerDevice != null)
        {
            playerInput.user.UnpairDevices();
            InputUser.PerformPairingWithDevice(PlayerSelect.swordPlayerDevice, playerInput.user);
        }
        else
        {
            Debug.LogWarning("Sword player device was not assigned in PlayerSelect.");
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        playerInput.actions["UpEffect"].performed += Jump;
        playerInput.actions["DownEffect"].performed += Duck;
        playerInput.actions["Attack"].performed += Attack;
        playerInput.actions["Block/Parry"].started += Block;
        playerInput.actions["Block/Parry"].canceled += CancelBlock;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        playerInput.actions["UpEffect"].performed -= Jump;
        playerInput.actions["DownEffect"].performed -= Duck;
        playerInput.actions["Attack"].performed -= Attack;
        playerInput.actions["Block/Parry"].started -= Block;
        playerInput.actions["Block/Parry"].canceled -= CancelBlock;
    }

    public override float GetCooldownPercent()
    {
        return blockCooldownPercent;
    }

    private void OnDashStart(float deltaLane)
    {
        if (deltaLane > 0f)
            PlayCycleAnimation("Dash Right");
        else if (deltaLane < 0f)
            PlayCycleAnimation("Dash Left");
    }

    private void OnDashEnd()
    {
        PlayDefaultCycleAnimation();
    }

    private void PlayAnimation(string name, Action<string, float> animate)
    {
        if (delayedAnimation != null)
        {
            StopCoroutine(delayedAnimation);
            delayedAnimation = null;
        }

        if (name == "Attack" || state != SwordPlayerStates.Attacking)
            animate(name, 0f);
        else
        {
            float attackTimeLeft = Time.time - animator.LastAnimationStartTime;
            IEnumerator DelayPlay()
            {
                yield return new WaitForSeconds(attackTimeLeft);
                animate(name, attackTimeLeft);
                delayedAnimation = null;
            }

            if (attackTimeLeft < animator.GetAnimationDuration(name))
                delayedAnimation = StartCoroutine(DelayPlay());
        }
    }

    private void PlayOneShotAnimation(string name)
    {
        PlayAnimation(name, (n, d) => animator.PlayOneShot(n, d));
    }

    private void PlayCycleAnimation(string name)
    {
        PlayAnimation(name, (n, d) => animator.PlayCycle(n, d));
    }

    private void PlayDefaultCycleAnimation()
    {
        PlayAnimation(animator.defaultName, (n, d) => animator.PlayDefaultCycle(d));
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (Stunned)
            return;

        if (jumpRoutine != null)
            return;

        void SetY(float y)
        {
            transform.position = new(transform.position.x, y, transform.position.z);
        }

        IEnumerator Routine()
        {
            animator.defaultName = "Jump";
            PlayDefaultCycleAnimation();

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

            animator.defaultName = "Idle";
            PlayDefaultCycleAnimation();
        }

        jumpRoutine = StartCoroutine(Routine());
    }

    private void Duck(InputAction.CallbackContext ctx)
    {
        if (Stunned)
            return;

        Debug.Log("Duck");
    }

    private void Attack(InputAction.CallbackContext ctx)
    {
        if (Stunned)
            return;
        if (PlayerStats.Instance.GetSwordSuperPercent() >= 1f && !PlayerStats.Instance.IsSuperActive())
        {
            Debug.Log("Attack button pressed with super ready");
            //Set attack button pressed super to true
            attackButtonPressedSuper = true;
            if (blockButtonPressedSuper && !PlayerStats.Instance.IsSuperActive())
            {
                Debug.Log("Sword Player Activating Super Attack!");
                //PlayerStats.Instance.ActivateSuper();
                return;
            }
            resetAttackButtonPressedSuperCoroutine = StartCoroutine(ResetAttackButtonPressedSuper());
        }

        if (state == SwordPlayerStates.Normal)
        {
            state = SwordPlayerStates.Attacking;
            PlayOneShotAnimation("Attack");
            swordHitBox.gameObject.SetActive(true);
            
            IEnumerator Routine()
            {
                yield return new WaitForSeconds(attackDuration);
                state = SwordPlayerStates.Normal;
                attackRoutine = null;
                swordHitBox.gameObject.SetActive(false);
            }
            
            attackRoutine = StartCoroutine(Routine());
        }
    }

    public void Block(InputAction.CallbackContext ctx)
    {
        if (Stunned)
            return;
        if (PlayerStats.Instance.GetSwordSuperPercent() >= 1f)
        {
            //Set block button pressed super to true
            Debug.Log("Block button pressed with super ready");
            blockButtonPressedSuper = true;
            if (attackButtonPressedSuper && !PlayerStats.Instance.IsSuperActive())
            {
                Debug.Log("Sword Player Activating Super Attack!");
                PlayerStats.Instance.PrepareSwordSuperReady(true);
                return;
            }
            resetBlockButtonPressedSuperCoroutine = StartCoroutine(ResetBlockButtonPressedSuper());

        }

        Debug.Log("Block/Parry");
        if (canBlock && state == SwordPlayerStates.Normal)
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
        if (Stunned)
            return;

        Debug.Log("Cancel Block");
        if (parryRoutine != null)
        {
            StopCoroutine(parryRoutine);
            parryRoutine = null;
        }
        state = SwordPlayerStates.Normal;
        PlayDefaultCycleAnimation();
        swordHitBox.gameObject.SetActive(false);
    }

    private IEnumerator ParryWindow()
    {
        parryTimer = 0f;
        state = SwordPlayerStates.Parrying;
        PlayCycleAnimation("Block");
        while (parryTimer < parryWindow)
        {
            if (state != SwordPlayerStates.Parrying)
            {
                yield break;
            }
            if (!PlayerStats.Instance.IsSuperActive())
            {
                parryTimer += Time.deltaTime;
            }
            yield return null;
        }
        if (state == SwordPlayerStates.Parrying)
        {
            state = SwordPlayerStates.Blocking;
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
        OnBlockCooldownReady?.Invoke();
    }

    protected override void OnStunStart()
    {
        base.OnStunStart();
        swordHitBox.gameObject.SetActive(false);
    }

    protected override void OnStunEnd()
    {
        base.OnStunEnd();
        state = SwordPlayerStates.Normal;
        PlayDefaultCycleAnimation();
    }

    public void OnSwordHitBoxTriggerEnter(Collider collider)
    {
        Enemy enemy = collider.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            switch (state)
            {
                case SwordPlayerStates.Attacking:
                    if (!enemy.HasShield() && enemy.OnParried())
                    {
                        playerStats.AddSwordSuper(4f);
                        AddContinuousMultiplier(attackingMultiplierGain);
                        AddScore(enemy.Score);
                    }
                    break;
                case SwordPlayerStates.Parrying:
                    if (enemy.OnParried())
                    {
                        playerStats.AddSwordSuper(5f);
                        AddContinuousMultiplier(meleeParryMultiplierGain);
                        AddScore(enemy.Score);
                    }
                    parryTimer = 0f;
                    break;
                case SwordPlayerStates.Blocking:
                    if (canBlock)
                    {
                        if (!enemy.HasShield() && enemy.OnParried() && !PlayerStats.Instance.IsSuperActive())
                        {
                            playerStats.AddSwordSuper(2f);
                            AddContinuousMultiplier(blockingMultiplierGain);
                            AddScore(enemy.Score);
                        }
                        StartCoroutine(BlockCooldown());
                    }
                    break;
            }
        }
        else if (collider.TryGetComponent(out EnemyProjectile projectile))
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

    private void ReflectBackBullet(EnemyProjectile projectile)
    {
        projectile.Parry(swordHitBox.transform, parryBulletSpeedMult);
    }

    public void OnBulletParryKill(int score)
    {
        playerStats.AddSwordSuper(5f);
        AddContinuousMultiplier(bulletParryMultiplierGain);
        AddScore(score);
    }

    private IEnumerator ResetAttackButtonPressedSuper()
    {
        yield return new WaitForSeconds(activateSuperWaitTime);
        attackButtonPressedSuper = false;
        resetAttackButtonPressedSuperCoroutine = null;
    }

    private IEnumerator ResetBlockButtonPressedSuper()
    {
        yield return new WaitForSeconds(activateSuperWaitTime);
        blockButtonPressedSuper = false;
        resetBlockButtonPressedSuperCoroutine = null;
    }
}
