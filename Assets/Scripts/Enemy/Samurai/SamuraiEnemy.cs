using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

// TODO don't take damage from sword player melee attack unless stunned
// TODO split attack animation into windup and slash for individual speed adjustment
// TODO parried bullet from player shouldn't kill samurai, just stun it
// TODO stunned samurai can be killed by either player
public class SamuraiEnemy : MonoBehaviour
{
    [SerializeField] private Collider healthCollider;
    [SerializeField] private float walkingSpeed = 10f;
    [SerializeField] private float stunTime = 1f;
    [SerializeField] private EnemySwordHitbox swordHitBox;
    [SerializeField] private float parrySpeedMultipliyer = 1.1f;

    [Header("Stunned")]
    [SerializeField] private float shakeInterval = 0.1f;
    [SerializeField] private float shakeOffset = 0.025f;
    [SerializeField] private float stunnedDuration = 0.5f;

    [Header("Slashing")]
    [SerializeField] private int numberOfSlashes = 2;
    [SerializeField] private float slashCooldown = 1f;
    [SerializeField] private float slashDistance = 5f;
    [SerializeField] private float slashDuration = 0.5f;

    private enum SamuraiState { Walking, Slashing, Stunned, Leaving };
    private SamuraiState state;
    private BulletDetector bulletDetector;
    private int numberOfSlashesDone;
    private PlayerStats playerStats;
    protected LaneBound lane;

    private Billboard[] billboards;
    private SpriteAnimator[] animators;

    private Coroutine slashRoutine;

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
        enemy.immuneToBullet = (b, c) => b.State != Bullet.ProjectileState.ParriedByPlayer || c != healthCollider;

        billboards = GetComponentsInChildren<Billboard>();
        Assert.IsTrue(billboards.Length > 0);

        animators = GetComponentsInChildren<SpriteAnimator>();
        Assert.IsTrue(animators.Length > 0);

        foreach (var animator in animators)
        {
            animator.SetAnimationDuration("Stunned", stunnedDuration);
            animator.SetAnimationDuration("Slash", slashDuration);
        }
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
    }

    private void Start()
    {
        state = SamuraiState.Walking;
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
                            animator.PlayOneShot("Slash");

                        yield return new WaitForSeconds(slashDuration);
                        swordHitBox.gameObject.SetActive(false);

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
        // TODO this is not working
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
            else
            {
                if (Physics.Raycast(b.transform.position, b.transform.forward, out RaycastHit hit))
                    Debug.Log(hit.collider.name);
            }
        }

        return handled;
    }

    private bool IsPredictedToHit(Bullet b)
    {
        return Physics.Raycast(b.transform.position, b.transform.forward, out RaycastHit hit) && hit.collider == healthCollider;
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
        if (animators[0].LocalFrame < animators[0].GetAnimationFrameCount("Slash") - 1)
            return;

        if (collider.TryGetComponentInHierarchy(out GunPlayerController gunPlayer))
        {
            gunPlayer.Stun(stunTime);
        }
        else if (collider.TryGetComponentInHierarchy(out SwordPlayerController swordPlayer))
        {
            if (swordPlayer.TryBlock())
            {
                swordPlayer.AddContinuousMultiplier(swordPlayer.MeleeParryMultiplierGain);

                IEnumerator Routine(Billboard spriteBillboard)
                {
                    AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerStunned, transform.position);

                    Vector3 initialCameraOffset = spriteBillboard.cameraOffset;
                    int shakeCounter = 0;
                    for (float t = 0f; t < stunnedDuration; t += Time.deltaTime)
                    {
                        if (t > shakeCounter * shakeInterval)
                        {
                            shakeCounter = Mathf.CeilToInt(t / shakeInterval);
                            Vector3 cameraOffset = initialCameraOffset;
                            Vector2 shake = Random.insideUnitCircle * shakeOffset;
                            cameraOffset.x += shake.x;
                            cameraOffset.y += shake.y;
                            spriteBillboard.cameraOffset = cameraOffset;
                        }

                        yield return null;
                    }
                    spriteBillboard.cameraOffset = initialCameraOffset;

                    state = SamuraiState.Slashing;
                }

                state = SamuraiState.Stunned;
                // TODO stun sfx

                foreach (var animator in animators)
                    animator.PlayOneShot("Stunned");

                foreach (var billboard in billboards)
                    StartCoroutine(Routine(billboard));
            }
            else
            {
                swordPlayer.Stun(stunTime);
            }
        }
    }
}
