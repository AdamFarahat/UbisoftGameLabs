using System;
using UnityEngine;
using UnityEngine.Assertions;

public class GrenadeLauncher : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Transform throwPosition;
    [SerializeField] private int grenadeDamage = 10;
    [SerializeField] protected float throwingCooldown = 0.5f;

    private float cooldown = 0.0f;

    private void Awake()
    {
        Assert.IsNotNull(grenadePrefab);
    }

    private void Update()
    {
        if (cooldown > 0.0f)
            cooldown -= Time.deltaTime;
    }

    public virtual void Throw()
    {
        throw new NotImplementedException();
    }

    public virtual void HoldInput()
    {
    }

    public virtual void ReleaseInput()
    {
    }

    protected bool PreThrow()
    {
        if (cooldown <= 0.0f)
        {
            cooldown = throwingCooldown;
            return true;
        }
        return false;
    }

    protected Grenade InstantiateGrenade()
    {
        GameObject go = Instantiate(grenadePrefab);
        Grenade grenade = go.GetComponent<Grenade>();
        Assert.IsNotNull(grenade);

        grenade.transform.position = throwPosition.position;
        grenade.damage = grenadeDamage;
        return grenade;
    }
}
