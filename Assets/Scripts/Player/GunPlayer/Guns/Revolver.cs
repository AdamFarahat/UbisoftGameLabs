using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class Revolver : Gun
{
    [Header("Revolver")]
    [SerializeField] private GameObject laserShotPrefab;
    [SerializeField] private float laserChargeTime = 0.4f;
    [SerializeField] private float laserCooldownTime = 0.6f;

    private bool charging = false;
    private float chargeStartTime = 0f;
    private bool onCooldown = false;

    protected override void Awake()
    {
        base.Awake();

        Assert.IsNotNull(laserShotPrefab);
    }

    public override void StartFiring()
    {
        if (onCooldown){
            
            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerRevolverNotReady, transform.position);
            return;  // TODO denied sfx (here and in other guns)
        }

        charging = PreStartFiring();
        chargeStartTime = Time.time;
        // TODO start charge up animation
        // TODO start charge up sfx
    }

    public override void StopFiring()
    {
        if (!charging)
            return;
        charging = false;

        if (Time.time - chargeStartTime < laserChargeTime){
            InstantiateShot<Bullet>().damage = bulletDamage;
            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerRevolverShot, transform.position);
        }
        else
        {
            InstantiateShot<LaserShot>(laserShotPrefab).fakeParent = FirePosition;
            onCooldown = true;

            IEnumerator Cooldown()
            {
                yield return new WaitForSeconds(laserCooldownTime);
                onCooldown = false;
            }

            StartCoroutine(Cooldown());
        }
    }

    public override void CancelFiring()
    {
        charging = false;
    }
}
