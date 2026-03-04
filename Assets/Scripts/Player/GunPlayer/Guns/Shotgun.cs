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
        // TODO start charge sfx + animation
    }

    public override void StopFiring()
    {
        if (!charging)
            return;
        charging = false;

        ShotgunBlast blast = InstantiateShotgunBlast();
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
