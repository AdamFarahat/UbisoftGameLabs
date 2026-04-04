using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class CowboyEnemy : ShooterEnemy
{
    [SerializeField] private Collider healthCollider;
    [SerializeField] private float laneStayPeriod = 2f;
    [SerializeField] private float ProjectileTresholdSpeed = 400f;
    [SerializeField] private float chargeupTime = 0.5f;

    private enum CowBoyState { Walking, Charging, Dodging };
    private CowBoyState state;
    private BulletDetector bulletDetector;
    private List<Bullet> ignoredBullets = new();
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
        enemy.AvoidsBullet.Add(b => ignoredBullets.Contains(b));

        animators = GetComponentsInChildren<SpriteAnimator>();
        Assert.IsTrue(animators.Length > 0);
        dodgeDuration = animators[0].GetAnimationDuration("Dodge");
        foreach (var animator in animators)
            animator.SetAnimationDuration("Charge", shotCooldown);
    }

    private void ResetState()
    {
        lane.LaneDistance = LaneSet.SpawnLine;
        state = CowBoyState.Walking;
        shotCooldownLeft = shotCooldown;
        laneStayTimeLeft = laneStayPeriod;
    }

    private void Start()
    {
        state = CowBoyState.Walking;
        shotCooldownLeft = shotCooldown;
        laneStayTimeLeft = laneStayPeriod;
    }

    private void Update()
    {
        switch (state)
        {
            case CowBoyState.Walking:
                if (AnyBulletsComingInRange())
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
                AnyBulletsComingInRange(); // keep ignoring incoming bullets
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

        ignoredBullets = ignoredBullets.Where(b => b != null && bulletDetector.NearbyBullets.Contains(b)).ToList();
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

    private bool AnyBulletsComingInRange()
    {
        bool bulletsInRange = false;
        foreach (Bullet b in bulletDetector.NearbyBullets)
        {
            if (!b.isActiveAndEnabled || b.State != Bullet.ProjectileState.ShotByPlayer)
                continue;

            if (IsPredictedToHit(b) && b.Speed <= ProjectileTresholdSpeed)
            {
                if (!ignoredBullets.Contains(b))
                {
                    ignoredBullets.Add(b);
                    bulletsInRange = true;
                }
            }
        }

        return bulletsInRange;
    }

    private bool IsPredictedToHit(Bullet b)
    {
        return Physics.Raycast(b.transform.position, b.transform.forward, out RaycastHit hit)
            && hit.collider == healthCollider;
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
        lane.LaneDistance -= speed * Time.deltaTime;
    }
}
