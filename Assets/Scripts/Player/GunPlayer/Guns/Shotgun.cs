using System;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun
{
    [Header("Shotgun")]
    [SerializeField] private float spreadAngle = 45f;

    [Serializable]
    public class AltShot
    {
        public float heightScale;
        public float chargeTime;
    }

    [SerializeField] private List<AltShot> altShots = new();

    private bool charging = false;
    private float chargeStartTime = 0f;

    private void Start()
    {
        altShots.Sort((a, b) => a.chargeTime.CompareTo(b.chargeTime));
    }

    public override void StartFiring()
    {
        charging = PreStartFiring();
        chargeStartTime = Time.time;
        // TODO play charge up animation
        // TODO play charge up sfx
    }

    public override void StopFiring()
    {
        if (!charging)
            return;
        charging = false;

        ShotgunBlast blast = InstantiateShot<ShotgunBlast>();
        AudioManager.instance.PlayOneShot(FMODEvents.instance.playerShotgunShot, transform.position);
        blast.damage = bulletDamage;
        blast.coneAngle = spreadAngle;

        float chargeTime = Time.time - chargeStartTime;
        foreach (var altShot in altShots)
        {
            if (chargeTime < altShot.chargeTime)
                break;
            blast.heightScale = altShot.heightScale;
        }
    }

    public override void CancelFiring()
    {
        charging = false;
    }
}
