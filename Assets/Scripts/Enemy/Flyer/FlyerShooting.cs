using UnityEngine;
using UnityEngine.Assertions;

public class FlyerShooting : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float stunTime = 0.3f;
    [SerializeField] private float shotCooldown = 0.65f;
    [SerializeField] private float shotDistanceThreshold = 20f;
    [SerializeField] private float bulletSpeed = 80f;

    private LaneBound lane;
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

    private void Shoot()
    {
        GameObject go = ProjectilePool.SharedInstance.Spawn(spawnPoint.position, Quaternion.identity);
        Assert.IsNotNull(go);
        
        EnemyProjectile projectile = go.GetComponent<EnemyProjectile>();
        Assert.IsNotNull(projectile);
        Vector3 direction = LaneSet.Instance.GetLanePosition(lane.LaneIndex, LaneSet.PlayerLine) - spawnPoint.position;
        projectile.Initialize(direction, bulletSpeed);
        Stunner stunner = go.GetComponent<Stunner>();
        Assert.IsNotNull(stunner);
        stunner.stunTime = stunTime;
    }
}
