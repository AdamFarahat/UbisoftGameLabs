using UnityEngine;
using FMODUnity;

public class Bullet : MonoBehaviour
{
    public float velocity = 100f;
    [SerializeField] private float acceleration = 0f;
    [SerializeField] private float range = 400f;
    [SerializeField] private bool canPenetrateShield = false;
    public int damage = 10;

    private float distance = 0f;
    private bool dead = false;

    public bool IsDead => dead;

    public EventReference impactEvent;

    private void Update()
    {
        if (dead)
            return;

        float deltaDistance = velocity * Time.deltaTime;
        distance += deltaDistance;
        if (distance > range)
            Destroy(gameObject);

        transform.position += deltaDistance * transform.forward;
        velocity += acceleration * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
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


        dead = true;
        // TODO uncomment once bullet sprites are uploaded
        // IEnumerator FadeOutRoutine()
        // {
        //     yield return FadeAnimation.FadeOutRoutine(GetComponentInChildren<SpriteRenderer>());
        //     Destroy(gameObject);  // TODO sfx ?
        // }
        // StartCoroutine(FadeOutRoutine());
        Destroy(gameObject);  // TODO sfx ?
    }
    private void OnEnemyKill(Enemy enemy)
    {
        // TODO handle more complex gun player multiplier logic
        GunPlayerController.Instance.AddContinuousMultiplier(GunPlayerController.Instance.GunKillMultiplierGain);
        GunPlayerController.Instance.AddScore(enemy.Score);
    }
    public void ReverseSpeed() {
        this.velocity *= -1;
    }
}
