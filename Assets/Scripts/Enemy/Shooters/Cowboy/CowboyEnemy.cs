using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class CowboyEnemy : ShooterEnemy
{
    [SerializeField] private Collider healthCollider;
    [SerializeField] private float laneStayPeriod = 2f;
    [SerializeField] private float chargeupTime = 0.5f;

    [SerializeField] private float surpassingAcceleration = 50f;
    private bool playersSurpassed = false;

    [SerializeField] private bool stayOutOfShotgunRange = false;
    [SerializeField] private float shotgunRange = 75f;

    private enum CowBoyState { Walking, Charging, Dodging };
    private CowBoyState state = CowBoyState.Walking;
    private BulletDetector bulletDetector;
    private SpriteAnimator[] animators;

    private float laneStayTimeLeft = 0f;
    private float shotCooldownLeft = 0f;
    private Coroutine chargeRoutine;
    private float dodgeDuration;
    private Coroutine dodgeRoutine;

    protected override void Awake()
    {
        base.Awake();

        Assert.IsNotNull(healthCollider);

        bulletDetector = GetComponentInChildren<BulletDetector>();
        Assert.IsNotNull(bulletDetector);

        Enemy enemy = GetComponent<Enemy>();
        enemy.OnTakeFromPool += ResetState;
        enemy.ImmuneToBullet = (_, _) => true;
        enemy.SurpassedPlayers += () => { playersSurpassed = true; };

        animators = GetComponentsInChildren<SpriteAnimator>();
        Assert.IsTrue(animators.Length > 0);
    }

    private void Start()
    {
        dodgeDuration = animators[0].GetAnimationDuration("Dodge");
        foreach (var animator in animators)
            animator.SetAnimationDuration("Charge", shotCooldown);

        shotCooldownLeft = shotCooldown;
        laneStayTimeLeft = laneStayPeriod;
    }

    private void ResetState()
    {
        lane.LaneDistance = LaneSet.SpawnLine;
        state = CowBoyState.Walking;
        shotCooldownLeft = shotCooldown;
        laneStayTimeLeft = laneStayPeriod;
        playersSurpassed = false;
    }

    private void Update()
    {
        switch (state)
        {
            case CowBoyState.Walking:
                if (playersSurpassed)
                    speed += surpassingAcceleration * Time.deltaTime;

                if (AnyProjectilesComingInRange())
                {
                    state = CowBoyState.Dodging;
                    AnimateDodge();
                    break;
                }
                else
                {
                    laneStayTimeLeft -= Time.deltaTime;
                    if (laneStayTimeLeft <= 0f && lane.LaneDistance > LaneSet.EnemyMoveBufferLine)
                    {
                        ChangeLane();
                        laneStayTimeLeft = laneStayPeriod;
                    }
                    else if (IsInShootingRange())
                    {
                        shotCooldownLeft -= Time.deltaTime;
                        if (shotCooldownLeft <= 0f)
                        {
                            state = CowBoyState.Charging;
                            AnimateCharge();
                            shotCooldownLeft = shotCooldown;
                        }
                    }
                }
                WalkForward();
                break;
            case CowBoyState.Charging:
                if (chargeRoutine == null)
                {
                    IEnumerator Routine()
                    {
                        yield return new WaitForSeconds(chargeupTime);
                        Shoot();
                        state = CowBoyState.Walking;
                        chargeRoutine = null;
                    }

                    chargeRoutine = StartCoroutine(Routine());
                }
                break;
            case CowBoyState.Dodging:
                if (dodgeRoutine == null)
                {
                    IEnumerator Routine()
                    {
                        yield return new WaitForSeconds(dodgeDuration);
                        state = CowBoyState.Walking;
                        dodgeRoutine = null;
                    }

                    dodgeRoutine = StartCoroutine(Routine());
                }
                break;
        }
    }

    private void AnimateDodge()
    {
        foreach (var animator in animators)
            animator.PlayOneShot("Dodge");
    }

    private void AnimateCharge()
    {
        foreach (var animator in animators)
            animator.PlayOneShot("Charge");
    }

    private bool AnyProjectilesComingInRange()
    {
        return bulletDetector.NearbyShotgunBlasts.Any(b => IsPredictedToHit(b))
            || bulletDetector.NearbyBullets.Any(b => IsPredictedToHit(b));
    }

    private bool IsPredictedToHit(Bullet b)
    {
        return b.isActiveAndEnabled && b.IsComingFromPlayer()
            && Physics.Raycast(b.transform.position, b.transform.forward, out RaycastHit hit)
            && hit.collider == healthCollider;
    }

    private bool IsPredictedToHit(ShotgunBlast b)
    {
        return b.isActiveAndEnabled;
    }

    private void ChangeLane()
    {
        int laneIndex;
        if (lane.LaneIndex == 0)
            laneIndex = lane.LaneIndex + 1;
        else if (lane.LaneIndex == LaneSet.LaneCount - 1)
            laneIndex = lane.LaneIndex - 1;
        else
            laneIndex = lane.LaneIndex + Random.Range(0, 2) * 2 - 1;

        lane.MoveToLane(laneIndex);
    }

    private void WalkForward()
    {
        if (!stayOutOfShotgunRange || lane.LaneDistance > shotgunRange)
           lane.LaneDistance -= speed * Time.deltaTime;
    }
}
