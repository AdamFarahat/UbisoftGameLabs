using UnityEngine;
using UnityEngine.Assertions;

public class Revolver : Gun
{
    [Header("Revolver")]
    [SerializeField] private LaserShot laserShot;
    [SerializeField] private float laserChargeTime = 0.6f;

    private bool charging = false;
    private float chargeStartTime = 0f;

    protected override void Awake()
    {
        base.Awake();

        Assert.IsNotNull(laserShot);
    }

    public override void StartFiring()
    {
        charging = PreStartFiring();
        chargeStartTime = Time.time;
        // TODO start charge sfx + animation
    }

    public override void StopFiring()
    {
        if (!charging)
            return;
        charging = false;

        if (Time.time - chargeStartTime < laserChargeTime)
            InstantiateShot<Bullet>().damage = bulletDamage;
        else
            laserShot.Fire();
    }

    public override void CancelFiring()
    {
        charging = false;
    }
}
