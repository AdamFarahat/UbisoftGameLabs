using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Gun : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePosition;
    [SerializeField] protected float bulletVelocity = 100f;
    [SerializeField] protected float bulletAcceleration = 10f;
    [SerializeField] protected float bulletRange = 100f;
    [SerializeField] private int bulletDamage = 10;
    [SerializeField] protected float firingCooldown = 0.5f;

    private float cooldown = 0.0f;

    private void Awake()
    {
        Assert.IsNotNull(bulletPrefab);
    }

    private void Update()
    {
        if (cooldown > 0.0f)
            cooldown -= Time.deltaTime;
    }

    public virtual void StartFiring(GunPlayerController gunPlayerController)
    {
        throw new NotImplementedException();
    }

    public virtual void KeepFiring(GunPlayerController gunPlayerController)
    {
    }

    public virtual void StopFiring()
    {
    }

    protected bool PreStartFiring()
    {
        if (cooldown <= 0.0f)
        {
            cooldown = firingCooldown;
            return true;
        }
        return false;
    }

    protected Bullet InstantiateBullet(GunPlayerController gunPlayerController)
    {
        GameObject go = Instantiate(bulletPrefab);
        Bullet bullet = go.GetComponent<Bullet>();
        Assert.IsNotNull(bullet);
        //setting owner
        bullet.owner = gunPlayerController;
        bullet.transform.position = firePosition.position;
        bullet.velocity = bulletVelocity;
        bullet.acceleration = bulletAcceleration;
        bullet.range = bulletRange;
        bullet.damage = bulletDamage;
        return bullet;
    }

    protected ShotgunBlast InstantiateShotgunBlast(GunPlayerController gunPlayerController)
    {
        GameObject go = Instantiate(bulletPrefab);
        ShotgunBlast blast = go.GetComponent<ShotgunBlast>();
        Assert.IsNotNull(blast);
        blast.gunPlayerControllerOwner = gunPlayerController;
        blast.transform.position = firePosition.position;
        blast.range = bulletRange;
        blast.damage = bulletDamage;
        return blast;
    }
}
