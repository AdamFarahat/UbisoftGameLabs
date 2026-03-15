using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class CowboyEnemy : ShooterEnemy
{
    [SerializeField] private Collider healthCollider;
    [SerializeField] private float initialLaneDistance = 200f;
    [SerializeField] private float laneStayPeriod = 2f;
    [SerializeField] private float projectileTresholdSpeed = 400f;
    [SerializeField] private float walkingSpeed = 10f;
    [SerializeField] private float checkIfCanShootInterval = 0.5f;
    [SerializeField] private float checkIfCanPunchInterval = 1f;
    [SerializeField] private float punchDistance = 10f;

    private enum CowBoyState { Walking, Charging, Punching };
    private CowBoyState state;
    private float time = 0f;
    private float nextLaneSwitchTime = 0f;
    private BulletDetector bulletDetector;

    protected override void Awake()
    {
        base.Awake();

        Assert.IsNotNull(healthCollider);

        bulletDetector = GetComponentInChildren<BulletDetector>();
        Assert.IsNotNull(bulletDetector);

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

    private void Update()
    {
        // TODO cowboy can only dodge when walking. If shooting, change line to dodge then change back.

        time += Time.deltaTime;
        switch (state)
        {
            case CowBoyState.Walking:
                if (BulletComingInRange())
                {
                    Dodge();
                }
                else if (time >= checkIfCanPunchInterval && IsInPunchingRange())
                {
                    state = CowBoyState.Punching;
                    time = 0f;
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
            case CowBoyState.Punching:
                Punch();
                break;
        }
    }

    private void Punch()
    {
        Debug.LogWarning("punch not yet implemented");
        // TODO
    }

    private bool BulletComingInRange()
    {
        foreach (Bullet b in bulletDetector.NearbyBullets)
            if (b != null && b.velocity <= projectileTresholdSpeed)
                return true;

        return false;
    }

    private void Dodge()
    {
        DodgeInvulnerability();
        ChangeLane();
        nextLaneSwitchTime = laneStayPeriod;
        time = 0f;
    }
    
    private void DodgeInvulnerability()
    {
        IEnumerator Routine()
        {
            healthCollider.enabled = false;
            yield return new WaitForSeconds(lane.SwitchLaneDuration);
            healthCollider.enabled = true;
        }

        StartCoroutine(Routine());
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
        lane.LaneDistance -= walkingSpeed * Time.deltaTime;
    }

    private bool IsInPunchingRange()
    {
        return PlayerController.AnyPlayerInLane(lane.LaneIndex) && lane.LaneDistance <= punchDistance
            && lane.LaneDistance <= LaneSet.VisibleEndLine;
    }
}
