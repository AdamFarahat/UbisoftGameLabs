using UnityEngine;
using UnityEngine.Assertions;

public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePosition;
    [SerializeField] protected float bulletVelocity = 100.0f;
    [SerializeField] protected float bulletAcceleration = 10.0f;
    [SerializeField] protected float bulletLifetime = 3.0f;
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

    public virtual bool Fire()
    {
        if (cooldown <= 0.0f)
        {
            cooldown = firingCooldown;
            InstantiateBullet();
            return true;
        }
        return false;
    }

    protected Bullet InstantiateBullet()
    {
        GameObject go = Instantiate(bulletPrefab);
        Bullet bullet = go.GetComponent<Bullet>();
        Assert.IsNotNull(bullet);

        bullet.transform.position = firePosition.position;
        bullet.velocity = bulletVelocity;
        bullet.acceleration = bulletAcceleration;
        bullet.lifetime = bulletLifetime;
        return bullet;
    }
}
