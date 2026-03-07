using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Gun : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePosition;
    [SerializeField] protected int bulletDamage = 10;
    [SerializeField] protected float firingCooldown = 0.5f;

    private float cooldown = 0.0f;

    protected virtual void Awake()
    {
        Assert.IsNotNull(bulletPrefab);
    }

    protected virtual void Update()
    {
        if (cooldown > 0.0f)
            cooldown -= Time.deltaTime;
    }

    public virtual void StartFiring()
    {
        throw new NotImplementedException();
    }

    public virtual void KeepFiring()
    {
    }

    public virtual void StopFiring()
    {
    }

    public virtual void CancelFiring()
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

    protected T InstantiateShot<T>() where T : class
    {
        GameObject go = Instantiate(bulletPrefab);
        go.transform.position = firePosition.position;
        T shot = go.GetComponent<T>();
        Assert.IsNotNull(shot);
        return shot;
    }
}
