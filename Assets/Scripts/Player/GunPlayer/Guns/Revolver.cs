using System;
using System.Collections.Generic;
using UnityEngine;

public class Revolver : Gun
{
    [Serializable]
    public class AltShot
    {
        public float velocity;
        public int damage;
        public float chargeTime;
    }

    [Header("Revolver")]
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

        Bullet bullet = InstantiateBullet();

        float chargeTime = Time.time - chargeStartTime;
        foreach (var altShot in altShots)
        {
            if (chargeTime < altShot.chargeTime)
                break;
            bullet.velocity = altShot.velocity;
            bullet.damage = altShot.damage;
        }
    }

    public override void CancelFiring()
    {
        charging = false;
    }
}
