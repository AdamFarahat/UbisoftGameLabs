using UnityEngine;
using UnityEngine.Assertions;

public class GrenadeBelt : MonoBehaviour
{
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private float maxChargeTime = 1f;
    [SerializeField] private int grenadeDamage = 10;

    private bool throwing = false;
    private float throwChargeTime = 0f;

    private void Awake()
    {
        Assert.IsNotNull(grenadePrefab);
    }

    private void Update()
    {
        if (throwing)
            throwChargeTime = Mathf.Min(throwChargeTime + Time.deltaTime, maxChargeTime);
    }

    public void ChargeThrow()
    {
        throwing = true;
    }

    public void Throw()
    {
        throwing = false;
        GameObject go = Instantiate(grenadePrefab);
        Grenade grenade = go.GetComponent<Grenade>();
        Assert.IsNotNull(grenade);

        grenade.transform.position = transform.position;
        grenade.damage = grenadeDamage;
        // TODO use charge time
    }
}
