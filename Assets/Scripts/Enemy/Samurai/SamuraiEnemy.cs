using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class SamuraiEnemy : MonoBehaviour
{
    [SerializeField] private Collider healthCollider;
    [SerializeField] private float walkingSpeed = 10f;
    [SerializeField] private float stunTime = 1f;
    [SerializeField] private EnemySwordHitbox swordHitBox;
    [SerializeField] private float parrySpeedMultipliyer = 1.1f;

    [Header("Stunned")]
    [SerializeField] private float stunnedDuration = 0.5f;

    [Header("Slashing")]
    [SerializeField] private int numberOfSlashes = 2;
    [SerializeField] private float slashCooldown = 1f;
    [SerializeField] private float slashDistance = 5f;
    [SerializeField] private float windupDuration = 0.5f;
    [SerializeField] private float slashDuration = 0.3f;

    private enum SamuraiState { Walking, Slashing, Stunned, Leaving };
    private SamuraiState state = SamuraiState.Walking;
    private BulletDetector bulletDetector;
    private int numberOfSlashesDone;
    private PlayerStats playerStats;
    protected LaneBound lane;

    private bool gunPlayerSlashed = false;
    private bool swordPlayerSlashed = false;

    private Billboard[] billboards;
    private SpriteAnimator[] animators;

    private Coroutine slashRoutine;

    private ShotgunImmune shotgunImmunity;
    private LaserImmune laserImmunity;

    protected void Awake()
    {
        lane = GetComponent<LaneBound>();
        playerStats = FindFirstObjectByType<PlayerStats>();
        bulletDetector = GetComponentInChildren<BulletDetector>();
        Assert.IsNotNull(bulletDetector);
        Assert.IsNotNull(playerStats);
        Assert.IsNotNull(lane);
        Assert.IsNotNull(swordHitBox);
        Assert.IsNotNull(healthCollider);

        Enemy enemy = GetComponent<Enemy>();
        Assert.IsNotNull(enemy);
        enemy.OnTakeFromPool += ResetState;
        enemy.ImmuneToBullet = IsImmuneToBullet;
        enemy.ImmuneToSword = CheckImmuneToSword;
        enemy.StunFromBullet = StunFromBullet;

        billboards = GetComponentsInChildren<Billboard>();
        Assert.IsTrue(billboards.Length > 0);

        animators = GetComponentsInChildren<SpriteAnimator>();
        Assert.IsTrue(animators.Length > 0);

        foreach (var animator in animators)
        {
            animator.SetAnimationDuration("Stunned", stunnedDuration);
            animator.SetAnimationDuration("Windup", windupDuration);
            animator.SetAnimationDuration("Slash", slashDuration);
        }

        shotgunImmunity = GetComponent<ShotgunImmune>();
        Assert.IsNotNull(shotgunImmunity);
        shotgunImmunity.HitByShotgun += OnHitByShotgun;

        laserImmunity = GetComponent<LaserImmune>();
        Assert.IsNotNull(laserImmunity);

        Assert.IsTrue(slashCooldown >= stunTime + 0.25f);
    }

    private void ResetState()
    {
        state = SamuraiState.Walking;
        numberOfSlashesDone = 0;
        lane.LaneDistance = LaneSet.SpawnLine;
        swordHitBox.gameObject.SetActive(false);
        if (slashRoutine != null)
        {
            StopCoroutine(slashRoutine);
            slashRoutine = null;
        }
        gunPlayerSlashed = false;
        swordPlayerSlashed = false;
    }

    private void Update()
    {
        switch (state)
        {
            case SamuraiState.Walking:
                if (ParryIncomingBullets())
                {
                    // TODO sfx
                    foreach (SpriteAnimator animator in animators)
                        animator.PlayOneShot("Parry");
                }

                if (IsInSlashingRange())
                    state = SamuraiState.Slashing;
                else
                    WalkForward();
                break;
            case SamuraiState.Slashing:
                if (DestroyIncomingBullets())
                {
                    // TODO sfx
                }

                if (slashRoutine == null)
                {
                    IEnumerator Routine()
                    {
                        swordHitBox.gameObject.SetActive(true);
                        
                        foreach (var animator in animators)
                            animator.PlayOneShot("Windup");
                        yield return new WaitForSeconds(windupDuration);

                        foreach (var animator in animators)
                            animator.PlayOneShot("Slash");
                        yield return new WaitForSeconds(slashDuration);

                        swordHitBox.gameObject.SetActive(false);
                        gunPlayerSlashed = false;
                        swordPlayerSlashed = false;

                        if (++numberOfSlashesDone >= numberOfSlashes)
                            state = SamuraiState.Leaving;
                        else
                            yield return new WaitForSeconds(slashCooldown);
                        slashRoutine = null;
                    }

                    slashRoutine = StartCoroutine(Routine());
                }
                break;
            case SamuraiState.Leaving:
                if (DestroyIncomingBullets())
                {
                    // TODO sfx
                }

                WalkForward();
                break;
        }
    }

    private bool ParryIncomingBullets()
    {
        return HandleIncomingBullets(b => b.Parry(null, parrySpeedMultipliyer, Bullet.ProjectileState.ParriedByEnemy));
    }

    private bool DestroyIncomingBullets()
    {
        return HandleIncomingBullets(b => b.Despawn());
    }

    private bool HandleIncomingBullets(System.Action<Bullet> callback)
    {
        bool handled = false;
        foreach (Bullet b in bulletDetector.NearbyBullets)
        {
            if (b.enabled && b.State == Bullet.ProjectileState.ShotByPlayer && IsPredictedToHit(b))
            {
                callback(b);
                handled = true;
            }
        }

        return handled;
    }

    private bool IsPredictedToHit(Bullet b)
    {
        return Physics.Raycast(b.transform.position, b.transform.forward, out RaycastHit hit) && hit.collider == healthCollider;
    }

    private bool IsImmuneToBullet(Bullet b, Collider c)
    {
        return c != healthCollider;
    }

    private bool CheckImmuneToSword()
    {
        if (state != SamuraiState.Stunned)
        {
            // TODO sfx
            // TODO flash vfx
            return true;
        }
        return false;
    }

    private bool StunFromBullet()
    {
        if (state != SamuraiState.Walking)
            return false;

        state = SamuraiState.Stunned;

        // TODO stun sfx
        foreach (var animator in animators)
            animator.PlayOneShot("Stunned");

        IEnumerator Routine()
        {
            yield return new WaitForSeconds(stunnedDuration);
            state = SamuraiState.Walking;
        }

        StartCoroutine(Routine());

        return true;
    }

    private void OnHitByShotgun()
    {
        // TODO flash vfx
    }

    private void WalkForward()
    {
        lane.LaneDistance -= walkingSpeed * Time.deltaTime;
    }

    private bool IsInSlashingRange()
    {
        return PlayerController.AnyPlayerInLane(lane.LaneIndex) && lane.LaneDistance <= slashDistance
            && lane.LaneDistance >= LaneSet.PlayerLine;
    }

    public void OnSwordHitBoxTriggerStay(Collider collider)
    {
        if (animators[0].CurrentAnimationName == "Windup"
                || animators[0].LocalFrame < animators[0].GetAnimationFrameCount("Slash") - 1)
            return;

        if (!swordPlayerSlashed && collider.TryGetComponentInHierarchy(out SwordPlayerController swordPlayer))
        {
            swordPlayerSlashed = true;
            if (swordPlayer.TryBlock())
            {
                swordPlayer.AddContinuousMultiplier(swordPlayer.MeleeParryMultiplierGain);
                StunSamurai();
            }
            else
            {
                swordPlayer.Stun(stunTime);
            }
        }
        else if (!gunPlayerSlashed && collider.TryGetComponentInHierarchy(out GunPlayerController gunPlayer))
        {
            gunPlayerSlashed = true;

            IEnumerator Routine()
            {
                yield return null;
                if (state != SamuraiState.Stunned)
                    gunPlayer.Stun(stunTime);
            }
            
            StartCoroutine(Routine());
        }
    }

    private void StunSamurai()
    {
        state = SamuraiState.Stunned;
        shotgunImmunity.enabled = false;
        laserImmunity.enabled = false;

        swordHitBox.gameObject.SetActive(false);
        gunPlayerSlashed = false;
        swordPlayerSlashed = false;

        if (slashRoutine != null)
        {
            StopCoroutine(slashRoutine);
            slashRoutine = null;
        }

        // TODO stun sfx

        foreach (var animator in animators)
            animator.PlayOneShot("Stunned");

        IEnumerator Routine()
        {
            yield return new WaitForSeconds(stunnedDuration);
            state = SamuraiState.Slashing;

            if (++numberOfSlashesDone >= numberOfSlashes)
                state = SamuraiState.Leaving;
            else
                yield return new WaitForSeconds(slashCooldown);
        }

        StartCoroutine(Routine());
    }
}
