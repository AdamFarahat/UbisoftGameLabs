using System;
using UnityEngine;
using UnityEngine.Assertions;

public class GrenadeBelt : MonoBehaviour
{
    [Header("Grenade")]
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private float minThrowRange = 50f;
    [SerializeField] private float maxThrowRange = 100f;
    [SerializeField] private float throwCooldown = 3f;
    [SerializeField] private Vector3 grenadeInitialDirection = new(0f, 1f, 1f);

    [Header("Crosshairs")]
    [SerializeField] private Transform crosshairs;
    [SerializeField] private float crosshairsStart = 0.5f;
    [SerializeField] private float crosshairsVelocity = -1f;
    [SerializeField] private float crosshairsRotationVelocity = 90f;

    public Action OnCooldownReady;
    private bool throwing = false;
    private float throwChargeTime = 0f;
    private float cooldown = 0f;
    private Billboard crosshairsBillboard;

    private void Awake()
    {
        Assert.IsNotNull(grenadePrefab);
        Assert.IsNotNull(crosshairs);
        grenadeInitialDirection.Normalize();

        crosshairsBillboard = crosshairs.GetComponentInChildren<Billboard>();
        Assert.IsNotNull(crosshairsBillboard);
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
            if (!PlayerStats.Instance.IsSuperActive())
                cooldown -= Time.deltaTime;
            else
                cooldown = 0f;
        }

        SyncCrosshairsPosition();
        crosshairsBillboard.rotation += crosshairsRotationVelocity * Time.deltaTime;
    }

    public void ChargeThrow()
    {
        if (cooldown <= 0f && !throwing)
        {
            SetThrowing(true);
            throwChargeTime = 0f;
            if (!PlayerStats.Instance.IsSuperActive())
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

        AudioManager.instance.PlayOneShot(FMODEvents.instance.playerGrenadeThrow, transform.position);

        grenade.transform.position = transform.position;
        grenade.initialDirection = grenadeInitialDirection;
        grenade.range = CalcGrenadeRange();
    }

    private float CalcGrenadeRange()
    {
        float t = 2f * crosshairsVelocity * throwChargeTime + crosshairsStart;
        float cos = Mathf.Cos(Mathf.PI * t);
        return 0.5f * (minThrowRange - maxThrowRange) * cos + 0.5f * (minThrowRange + maxThrowRange);
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
