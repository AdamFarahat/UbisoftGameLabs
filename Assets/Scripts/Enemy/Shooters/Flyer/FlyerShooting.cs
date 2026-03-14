using UnityEngine;
using UnityEngine.Assertions;

public class FlyerShooting : ShooterEnemy
{
    private float lastShotTime = 0f;

    private void Awake()
    {
        lane = GetComponent<LaneBound>();
        Assert.IsNotNull(lane);
    }

    private void Update()
    {
        if (PlayerController.AnyPlayerInLane(lane.LaneIndex) && lane.LaneDistance >= shotDistanceThreshold && lane.LaneDistance <= LaneSet.VisibleEndLine)
        {
            if (Time.time - lastShotTime > shotCooldown)
            {
                Shoot();
                lastShotTime = Time.time;
            }
        }
    }
}
