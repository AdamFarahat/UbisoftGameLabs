using UnityEngine;
using FMODUnity;

public class Bullet : ProjectileBase
{
    [SerializeField] private bool canPenetrateShield = false;
    public int damage = 10;

  

    public EventReference impactEvent;
    
    private void Start()
    {

        stunner.enabled = false;
        createdFromPool = false;
    }
    
    
    override protected void OnTriggerEnter(Collider other)
    {
        if (Parried)
        {
            base.OnTriggerEnter(other);
            return;
        }

        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            return;

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null)
            return;

        AudioManager.Instance.PlayOneShot(impactEvent, transform.position);

        EnergyShield shield = enemy.GetShield();
        if (shield != null)
        {
            if (canPenetrateShield)
                shield.TakeDamage(damage);
        }
        else if (enemy.TakeDamage(damage))
        {
            OnEnemyKill(enemy);
            PlayerStats.Instance.AddGunSuper(2f);
        }
         
        foreach (Collider collider in GetComponentsInChildren<Collider>())
            collider.enabled = false;


        Despawn();
    }
    private void OnEnemyKill(Enemy enemy)
    {
        // TODO handle more complex gun player multiplier logic
        GunPlayerController.Instance.AddContinuousMultiplier(GunPlayerController.Instance.GunKillMultiplierGain);
        GunPlayerController.Instance.AddScore(enemy.Score);
    }
}
