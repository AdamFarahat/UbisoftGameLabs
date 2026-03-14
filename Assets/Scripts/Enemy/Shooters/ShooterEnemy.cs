using UnityEngine;
using UnityEngine.Assertions;

public class ShooterEnemy : MonoBehaviour
{
    [SerializeField] protected float stunTime = 0.3f;
    [SerializeField] protected float shotCooldown = 0.65f;
    [SerializeField] protected float shotDistanceThreshold = 20f;
    [SerializeField] protected float bulletSpeed = 80f;
    [SerializeField] protected Transform spawnPoint;

    protected LaneBound lane;

    protected void Shoot()
    {
        GameObject go = ProjectilePool.SharedInstance.Spawn(spawnPoint.position, Quaternion.identity);
        Assert.IsNotNull(go);

        EnemyProjectile projectile = go.GetComponent<EnemyProjectile>();
        Assert.IsNotNull(projectile);

        Vector3 playerPosition = LaneSet.Instance.GetLanePosition(lane.LaneIndex, LaneSet.PlayerLine);
        playerPosition.y = LaneSet.PlayerTargetHeight;
        Vector3 direction = playerPosition - spawnPoint.position;
        projectile.Initialize(spawnPoint, direction, bulletSpeed);

        Stunner stunner = go.GetComponent<Stunner>();
        Assert.IsNotNull(stunner);
        stunner.stunTime = stunTime;
    }
}
