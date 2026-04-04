using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class CowboyEnemy : ShooterEnemy
{
    [SerializeField] private Collider healthCollider;
    [SerializeField] private float laneStayPeriod = 2f;
    [SerializeField] private float ProjectileTresholdSpeed = 400f;
    [SerializeField] private float checkIfCanShootInterval = 0.5f;

    private enum CowBoyState { Walking, Charging, Dodging };
    private CowBoyState state;
    private float time = 0f;
    private float nextLaneSwitchTime = 0f;
    private BulletDetector bulletDetector;
    private List<Bullet> ignoredBullets = new();
    
    protected override void Awake()
    {
        base.Awake();

        Assert.IsNotNull(healthCollider);

        bulletDetector = GetComponentInChildren<BulletDetector>();
        Assert.IsNotNull(bulletDetector);

        Enemy enemy = GetComponent<Enemy>();
        enemy.OnTakeFromPool += ResetState;
        enemy.AvoidsBullet.Add(b => ignoredBullets.Contains(b));
    }

    private void ResetState()
    {
        lane.LaneDistance = LaneSet.SpawnLine;
        time = 0f;
        nextLaneSwitchTime = laneStayPeriod;
        state = CowBoyState.Walking;
    }

    private void Start()
    {
        nextLaneSwitchTime = laneStayPeriod;
        state = CowBoyState.Walking;
    }

    private void Update()
    {
        time += Time.deltaTime;
        switch (state)
        {
            case CowBoyState.Walking:
                if (AnyBulletsComingInRange())
                {
                    state = CowBoyState.Dodging;
                    Debug.Log("Dodge!");
                    // TODO play one-shot dodge animation
                    time = 0f;
                    break;
                }
                else if (time >= nextLaneSwitchTime)
                {
                    ChangeLane();
                    time = 0f;
                }
                else if (time >= checkIfCanShootInterval && IsInShootingRange())
                {
                    state = CowBoyState.Charging;
                    time = 0f;
                }
                WalkForward();
                break;
            case CowBoyState.Charging:
                if (time >= shotCooldown)
                {
                    time = 0f;
                    Shoot();
                    state = CowBoyState.Walking;
                }
                break;
            case CowBoyState.Dodging:
                AnyBulletsComingInRange(); // keep ignoring incoming bullets
                if (time >= 0.5f) // TODO use animation duration
                {
                    state = CowBoyState.Walking;
                    time = 0f;
                }
                break;
        }

        ignoredBullets = ignoredBullets.Where(b => b != null && bulletDetector.NearbyBullets.Contains(b)).ToList();
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
