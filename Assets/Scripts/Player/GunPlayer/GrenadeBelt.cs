using System;
using UnityEngine;
using UnityEngine.Assertions;

public class GrenadeBelt : MonoBehaviour
{
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Transform crosshairs;
    [SerializeField] private float maxChargeTime = 1f;
    [SerializeField] private float minThrowRange = 50f;
    [SerializeField] private float maxThrowRange = 100f;
    [SerializeField] private float throwCooldown = 3f;
    [SerializeField] private float grenadeGravity = 100f;
    [SerializeField] private Vector3 grenadeInitialDirection = new(0f, 1f, 1f);

    public Action OnCooldownReady;
    private bool throwing = false;
    private float throwChargeTime = 0f;
    private float cooldown = 0f;

    private void Awake()
    {
        Assert.IsNotNull(grenadePrefab);
        grenadeInitialDirection.Normalize();
    }

    private void Start()
    {
        SetThrowing(false);
    }

    private void SetThrowing(bool throwing)
    {
        this.throwing = throwing;
        crosshairs.gameObject.SetActive(throwing);
    }

    private void Update()
    {
        if (throwing)
            throwChargeTime += Time.deltaTime;
        else if (cooldown > 0f)
        {
            cooldown -= Time.deltaTime;

        SyncCrosshairsPosition();
    }

    public void ChargeThrow()
    {
        if (cooldown <= 0f && !throwing)
        {
            SetThrowing(true);
            throwChargeTime = 0f;
            cooldown = throwCooldown;
        }
    }

    public void CancelThrow()
    {
        SetThrowing(false);
    }

    public void Throw()
    {
        if (!throwing)
            return;

        SetThrowing(false);
        GameObject go = Instantiate(grenadePrefab);
        Grenade grenade = go.GetComponent<Grenade>();
        Assert.IsNotNull(grenade);

        grenade.transform.position = transform.position;
        grenade.gravity = grenadeGravity;
        grenade.initialDirection = grenadeInitialDirection;
        grenade.range = CalcGrenadeRange();
    }

    private float CalcGrenadeRange()
    {
        float a = (throwChargeTime / maxChargeTime) % 2f;
        if (a > 1f)
            a = 2f - a;
        return Mathf.Lerp(minThrowRange, maxThrowRange, Mathf.Clamp01(a));
    }

    private void SyncCrosshairsPosition()
    {
        Vector3 position = crosshairs.position;
        position.z = CalcGrenadeRange();
        crosshairs.position = position;
    }

    public float GetCooldownPercent()
    {
        return Mathf.Clamp01(cooldown / throwCooldown);
    }
}
