using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class Revolver : Gun
{
    [Header("Revolver")]
    [SerializeField] private GameObject laserShotPrefab;
    [SerializeField] private float laserChargeTime = 0.4f;
    [SerializeField] private float laserCooldownTime = 0.6f;
    [SerializeField] private float speed = 170f;

    private bool charging = false;
    private float chargeStartTime = 0f;
    private bool onCooldown = false;

    protected override void Awake()
    {
        base.Awake();

        Assert.IsNotNull(laserShotPrefab);
    }

    public override void StartFiring()
    {
        if (onCooldown)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerRevolverNotReady, transform.position);
            return;
        }

        charging = PreStartFiring();
        chargeStartTime = Time.time;
        // TODO start charge up animation
        // TODO start charge up sfx
    }

    public override void StopFiring()
    {
        if (!charging)
            return;
        charging = false;

        if (Time.time - chargeStartTime < laserChargeTime)
        {
            Bullet bullet = InstantiateShot<Bullet>();
            bullet.damage = bulletDamage;
            bullet.Initialize(null, transform.forward, speed, Bullet.ProjectileState.ShotByPlayer);
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerRevolverShot, transform.position);
        }
        else
        {
            InstantiateShot<LaserShot>(laserShotPrefab).fakeParent = FirePosition;
            onCooldown = true;
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerRevolverAltShot, transform.position);
            IEnumerator Cooldown()
            {
                yield return new WaitForSeconds(laserCooldownTime);
                onCooldown = false;
            }

            StartCoroutine(Cooldown());
        }
    }

    public override void CancelFiring()
    {
        charging = false;
    }
}
