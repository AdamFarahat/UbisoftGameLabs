using UnityEngine;
using UnityEngine.Assertions;

public class FlyerShooting : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float stunTime = 0.3f;
    [SerializeField] private float shotCooldown = 0.65f;
    [SerializeField] private float shotDistanceThreshold = 20f;

    private LaneBound lane;
    private float lastShotTime = 0f;

    private void Awake()
    {
        lane = GetComponent<LaneBound>();
        Assert.IsNotNull(lane);
    }

    private void Update()
    {
        if (PlayerController.AnyPlayerInLane((int)lane.LaneIndex) && lane.LaneDistance >= shotDistanceThreshold)  // TODO no need to cast once lane PR is merged
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
        projectile.Initialize(LaneConfigSO.Instance.GetLanePosition(lane.LaneIndex, PlayerController.PlayerLine) - spawnPoint.position);
        Stunner stunner = go.GetComponent<Stunner>();
        Assert.IsNotNull(stunner);
        stunner.SetStunTime(stunTime);
    }
}
