using UnityEngine;

public class MachineGun : Gun
{
    [Header("Machine Gun")]
    [SerializeField] private float maxSpreadAngle = 1f;
    [SerializeField] private float spreadReduction = 2f;

    private float holdingCooldown = 0f;

    public override void StartFiring(GunPlayerController gunPlayerController)
    {
        if (!PreStartFiring())
            return;
        
        Debug.Log("StartFiring machine gun!");
        float spread = Random.Range(-1f, 1f);
        spread = maxSpreadAngle * Mathf.Sign(spread) * (1f - Mathf.Pow(1f - Mathf.Abs(spread), spreadReduction));
        InstantiateBullet(gunPlayerController).transform.forward = Quaternion.Euler(0f, spread, 0f) * transform.forward;
        holdingCooldown = firingCooldown;
    }

    public override void KeepFiring(GunPlayerController gunPlayerController)
    {
        holdingCooldown -= Time.deltaTime;
        if (holdingCooldown <= 0f)
            StartFiring(gunPlayerController);
    }
}
