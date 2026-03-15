using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using UnityEditor.Search;
using UnityEngine;

public class CowboyEnemy : ShooterEnemy
{
    [SerializeField] private float initialLaneDistance = 300f;
    [SerializeField] private float laneStayPeriod = 1.5f;
    [SerializeField] private float ProjectileTresholdSpeed = 10f;
    [SerializeField] private float WalkingSpeed = 2f;
    [SerializeField] private float CheckIfCanShootInterval = 2f;
    [SerializeField] private float CheckIfCanPunchInterval = 1f;
    [SerializeField] private float PunchDistance = 10f;



    private enum CowBoyState { Walking, Charging, Dodging, Punching };
    private CowBoyState state;
    private float time = 0f;
    private float nextLaneSwitchTime = 0f;
    private int laneIndex = 0;
    private BulletDetector bulletDetector;

    void Awake()
    {
        base.Awake();

        bulletDetector = GetComponentInChildren<BulletDetector>();

        GetComponent<Enemy>().OnTakeFromPool += ResetState;
    }

    private void ResetState()
    {
        lane.LaneDistance = initialLaneDistance;
        time = 0f;
        nextLaneSwitchTime = laneStayPeriod;
    }

    private void Start()
    {
        nextLaneSwitchTime = laneStayPeriod;
        state = CowBoyState.Walking;
    }

    void Update()
    {
        time += Time.deltaTime;
        switch (state)
        {
            case CowBoyState.Walking:
                if (BulletComingInRange())
                {
                    state = CowBoyState.Dodging;
                }
                else if (time >= CheckIfCanPunchInterval && IsInPunchingRange())
                {
                    state = CowBoyState.Punching;
                    time = 0f;
                }
                else if (time >= nextLaneSwitchTime)
                {
                    ChangeLane();
                    time = 0f;
                }
                else if (time >= CheckIfCanShootInterval && isInShootingRange())
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
                //right now it goes back into walking but we could make it change the lane
                //a second time back to where it was in order to continue shooting
                ChangeLane();
                state = CowBoyState.Walking;
                time = 0f;
                break;
            case CowBoyState.Punching:
                Punch();
                break;
        }
    }

    private void Punch()
    {
        Debug.LogWarning("punch not yet implemented");
    }

    private bool BulletComingInRange()
    {

        foreach (Bullet b in bulletDetector.NearbyBullets)
        {
            var projCollider = b.GetComponent<SphereCollider>();
            if (projCollider && IsPredictedToHit(b, projCollider))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsPredictedToHit(Bullet b, SphereCollider projCollider)
    {
        return b.velocity <= ProjectileTresholdSpeed
            && Vector3.Distance(b.transform.forward * b.velocity, transform.position)
            <= projCollider.radius;
    }

    private void ChangeLane()
    {
        if (lane.LaneIndex == 0)
        {
            laneIndex = lane.LaneIndex + 1;
        }
        else if (lane.LaneIndex == LaneSet.LaneCount - 1)
        {
            laneIndex = lane.LaneIndex - 1;
        }
        else
        {
            laneIndex = lane.LaneIndex + UnityEngine.Random.Range(0, 2) * 2 - 1;
        }

        lane.MoveToLane(laneIndex);
    }

    private void WalkForward()
    {
        lane.LaneDistance -= WalkingSpeed * Time.deltaTime;
    }

    private bool IsInPunchingRange()
    {
        return PlayerController.AnyPlayerInLane(lane.LaneIndex) && lane.LaneDistance <= PunchDistance
            && lane.LaneDistance <= LaneSet.VisibleEndLine;
    }
}
