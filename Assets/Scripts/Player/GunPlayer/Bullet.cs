using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float velocity = 100f;
    [SerializeField] private float acceleration = 0f;
    [SerializeField] private float range = 400f;
    [SerializeField] private bool canPenetrateShield = false;
    public int damage = 10;

    private float distance = 0f;

    private void Update()
    {
        float deltaDistance = velocity * Time.deltaTime;
        distance += deltaDistance;
        if (distance > range)
            Destroy(gameObject);

        transform.position += deltaDistance * transform.forward;
        velocity += acceleration * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null)
            return;

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
        Destroy(gameObject);  // TODO sfx/animation
    }

    private void OnEnemyKill(Enemy enemy)
    {
        // TODO handle more complex gun player multiplier logic
        GunPlayerController.Instance.AddContinuousMultiplier(GunPlayerController.Instance.GunKillMultiplierGain);
        GunPlayerController.Instance.AddScore(enemy.Score);
    }
}
