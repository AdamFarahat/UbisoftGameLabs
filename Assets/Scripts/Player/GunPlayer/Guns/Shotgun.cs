using UnityEngine;

public class Shotgun : Gun
{
    [Header("Shotgun")]
    [SerializeField] private float spreadAngle = 45f;
    [SerializeField] private float regHeightScale = 0.2f;
    [SerializeField] private float altHeightScale = 0.8f;
    [SerializeField] private float altChargeTime = 1f;

    private bool charging = false;
    private float chargeStartTime = 0f;

    public override void StartFiring()
    {
        charging = PreStartFiring();
        chargeStartTime = Time.time;
        // TODO start charge sfx + animation
    }

    public override void StopFiring()
    {
        if (!charging)
            return;
        charging = false;

        ShotgunBlast blast = InstantiateShotgunBlast();
        blast.coneAngle = spreadAngle;
        if (Time.time - chargeStartTime < altChargeTime)
            blast.heightScale = regHeightScale;
        else
            blast.heightScale = altHeightScale;
    }

    public override void CancelFiring()
    {
        charging = false;
    }
}
