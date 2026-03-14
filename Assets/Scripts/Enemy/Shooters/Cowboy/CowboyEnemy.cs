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
    
    

    private enum CowBoyState {Walking, Charging, Dodging, Punching};
    private CowBoyState state;
    private float time = 0f;
    private float nextLaneSwitchTime = 0f;
    private LaneBound laneBound;
    private int laneIndex = 0;
    private BulletDetector bulletDetector;
    void Awake()
    {
        laneBound = GetComponent<LaneBound>();
        Assert.IsNotNull(laneBound);

        bulletDetector = GetComponentInChildren<BulletDetector>();

        GetComponent<Enemy>().OnTakeFromPool += ResetState;
    }
    private void ResetState()
    {
        laneBound.LaneDistance = initialLaneDistance;
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
        if (state == CowBoyState.Walking)
        {
            if (bulletComingInRange())
            {
                state = CowBoyState.Dodging;
            }
            else if (time >= CheckIfCanPunchInterval && (GunPlayerController.LaneIndex == laneIndex || SwordPlayerController.LaneIndex == laneIndex)
                && (Vector3.Distance(GunPlayerController.Instance.transform.position, transform.position) <= PunchDistance ||
                    Vector3.Distance(SwordPlayerController.Instance.transform.position, transform.position) <= PunchDistance))
            {
                state = CowBoyState.Punching;
                time = 0f;
            }
            else if (time >= nextLaneSwitchTime)
            {
                changeLane();
                time = 0f;
            }
            else if (time >= CheckIfCanShootInterval && (GunPlayerController.LaneIndex == laneIndex || SwordPlayerController.LaneIndex == laneIndex)
                //should discuss if that should be done before charging or when we can shoot
                && (Vector3.Distance(GunPlayerController.Instance.transform.position, transform.position) <= shotDistanceThreshold ||
                    Vector3.Distance(SwordPlayerController.Instance.transform.position, transform.position) <= shotDistanceThreshold))

            {
                state = CowBoyState.Charging;
                time = 0f;
            }
            WalkForward();
        }
        else if (state == CowBoyState.Charging)
        {
            if (time >= shotCooldown)
            {
                time = 0f;
                Shoot();
                state = CowBoyState.Walking;
            }
        }
        else if (state == CowBoyState.Dodging)
        {
            changeLane();
            state = CowBoyState.Walking;
            time = 0f;
        }
        else if (state == CowBoyState.Punching) {
            punch();
        }
    }

    private void punch()
    {
        //TODO: when it gets in melee range it charges up a punch that can be parried (or you can just slash him)
        // need more clarification.
        throw new NotImplementedException();
    }

    private bool bulletComingInRange()
    {
        if (bulletDetector.bulletsNearby.Count == 0)
            return false;

        foreach (Bullet b in bulletDetector.bulletsNearby)
        {
            var projCollider = b.GetComponent<SphereCollider>();
            if (projCollider && b.velocity <= ProjectileTresholdSpeed 
                && Vector3.Distance(b.transform.forward * b.velocity, transform.position) <= projCollider.radius) {
                return true;
            }
        }
        return false;
    }

    private void changeLane()
    {
        if (laneBound.LaneIndex == 0)
        {
            laneIndex = laneBound.LaneIndex + 1;
        }
        else if (laneBound.LaneIndex == LaneSet.LaneCount - 1)
        {
            laneIndex = laneBound.LaneIndex - 1;
        }
        else
        {
            laneIndex = laneBound.LaneIndex + UnityEngine.Random.Range(0, 2) * 2 - 1;
        }

        laneBound.MoveToLane(laneIndex);
    }

    private void WalkForward()
    {
        laneBound.LaneDistance -= WalkingSpeed * Time.deltaTime;
    }
}
