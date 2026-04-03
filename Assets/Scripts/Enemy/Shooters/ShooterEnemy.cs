using UnityEngine;
using UnityEngine.Assertions;

public class ShooterEnemy : MonoBehaviour
{
    [SerializeField] protected float stunTime = 0.3f;
    [SerializeField] protected float shotCooldown = 0.65f;
    [SerializeField] protected float shotDistanceThreshold = 20f;  // minimum distance to shoot
    [SerializeField] protected float shootingRange = 10000f;  // max distance to shoot
    [SerializeField] protected float bulletSpeed = 80f;
    [SerializeField] protected Transform spawnPoint;

    protected LaneBound lane;

    protected virtual void Awake()
    {
        lane = GetComponent<LaneBound>();
        Assert.IsNotNull(lane);
    }

    protected bool IsInShootingRange()
    {
        return PlayerController.AnyPlayerInLane(lane.LaneIndex)
            && lane.LaneDistance >= Mathf.Max(shotDistanceThreshold, LaneSet.EnemyShootBufferLine)
            && lane.LaneDistance <= Mathf.Min(shootingRange, LaneSet.VisibleEndLine);
    }

    protected void Shoot()
    {
        GameObject go = ProjectilePool.SharedInstance.Spawn(spawnPoint.position, Quaternion.identity);
        Assert.IsNotNull(go);

        Bullet projectile = go.GetComponent<Bullet>();
        Assert.IsNotNull(projectile);

        Vector3 playerPosition = LaneSet.Instance.GetLanePosition(lane.LaneIndex, LaneSet.PlayerLine);
        playerPosition.y = LaneSet.PlayerTargetHeight;
        Vector3 direction = playerPosition - spawnPoint.position;
        projectile.Initialize(spawnPoint, direction, bulletSpeed, Bullet.ProjectileState.ShotByEnemy);

        Stunner stunner = go.GetComponent<Stunner>();
        Assert.IsNotNull(stunner);
        stunner.stunTime = stunTime;
    }
}
