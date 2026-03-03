using UnityEngine;
using UnityEngine.Assertions;

public class GrenadeBelt : MonoBehaviour
{
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Transform crosshairs;
    [SerializeField] private float maxChargeTime = 1f;
    [SerializeField] private float minThrowVelocity = 50f;
    [SerializeField] private float maxThrowVelocity = 100f;
    [SerializeField] private float throwCooldown = 3f;

    private bool throwing = false;
    private float throwChargeTime = 0f;
    private float cooldown = 0f;

    private void Awake()
    {
        Assert.IsNotNull(grenadePrefab);
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
            throwChargeTime = Mathf.Min(throwChargeTime + Time.deltaTime, maxChargeTime);
        else if (cooldown > 0f)
            cooldown -= Time.deltaTime;
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
        grenade.velocity = Mathf.Lerp(minThrowVelocity, maxThrowVelocity, Mathf.Clamp01(throwChargeTime / maxChargeTime));
    }

    public float GetCooldownPercent()
    {
        return Mathf.Clamp01(cooldown / throwCooldown);
    }
}
