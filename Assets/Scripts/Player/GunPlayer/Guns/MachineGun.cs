using UnityEngine;

public class MachineGun : Gun
{
    [Header("Machine Gun")]
    [SerializeField] private float maxSpreadAngle = 1f;
    [SerializeField] private float spreadReduction = 2f;

    private float holdingCooldown = 0f;

    public override void StartFiring()
    {
        if (!PreStartFiring())
            return;
        
        Debug.Log("StartFiring machine gun!");
        float spread = Random.Range(-1f, 1f);
        spread = maxSpreadAngle * Mathf.Sign(spread) * (1f - Mathf.Pow(1f - Mathf.Abs(spread), spreadReduction));
        InstantiateBullet().transform.forward = Quaternion.Euler(0f, spread, 0f) * transform.forward;
        holdingCooldown = firingCooldown;
    }

    public override void KeepFiring()
    {
        holdingCooldown -= Time.deltaTime;
        if (holdingCooldown <= 0f)
            StartFiring();
    }

    public override void CancelFiring()
    {
        base.CancelFiring();
        holdingCooldown = firingCooldown;
    }
}
