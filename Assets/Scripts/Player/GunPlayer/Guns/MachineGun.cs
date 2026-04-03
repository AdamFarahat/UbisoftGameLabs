using System.Collections;
using UnityEngine;

public class MachineGun : Gun
{
    [Header("Machine Gun")]
    [SerializeField] private float maxSpreadAngle = 0.5f;
    [SerializeField] private float spreadReduction = 2f;

    [SerializeField] private float overheatIncreaseRate = 0.2f; // 1 / duration
    [SerializeField] private float overheatDecreaseRate = 0.4f; // 1 / duration
    [SerializeField] private float overheatCooldown = 2f;

    [SerializeField] private float speed = 200f;

    private bool shooting = false;
    private float overheatLevel = 0f;
    private bool overheating = false;

    protected override void Update()
    {
        base.Update();

        if (overheating)
            return;

        if (shooting)
            overheatLevel += Time.deltaTime * overheatIncreaseRate;
        else
            overheatLevel -= Time.deltaTime * overheatDecreaseRate;

        if (overheatLevel < 0f)
            overheatLevel = 0f;
        else if (overheatLevel >= 1f)
        {
            overheatLevel = 1f;
            StopFiring();

            IEnumerator Overheat()
            {
                overheatLevel = 0f;
                overheating = true;
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerMachinegunOverheat, transform.position);
                yield return new WaitForSeconds(overheatCooldown);
                overheating = false;
            }

            StartCoroutine(Overheat());
        }
    }

    public override void StartFiring()
    {
        if (overheating)
            return;

        if (!PreStartFiring())
            return;
        
        float spread = Random.Range(-1f, 1f);
        spread = maxSpreadAngle * Mathf.Sign(spread) * (1f - Mathf.Pow(1f - Mathf.Abs(spread), spreadReduction));
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerMachinegunShot, transform.position);

        Bullet bullet = InstantiateShot<Bullet>();
        bullet.Initialize(null, Quaternion.Euler(0f, spread, 0f) * transform.forward,speed,Bullet.ProjectileState.ShotByPlayer);
        bullet.damage = bulletDamage;
        bullet.transform.forward = bullet.Direction;

        shooting = true;
    }

    public override void KeepFiring()
    {
        StartFiring();
    }

    public override void StopFiring()
    {
        base.StopFiring();
        shooting = false;
    }

    public override void CancelFiring()
    {
        base.CancelFiring();
        shooting = false;
    }
}
