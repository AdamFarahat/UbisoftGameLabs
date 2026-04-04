using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class Bullet : MonoBehaviour
{
    [SerializeField] private bool canPenetrateShield = false;
    public int damage = 10;

    private LaneBound lane;

    public EventReference impactEvent;

    private Billboard[] sprites;
    private SpriteRenderer[] spriteRenderers;
    [SerializeField] private float parryColliderScaleUp = 1.5f;
    [SerializeField] private float speed = 80f;
    private SphereCollider sphereCollider;
    private float normalColliderRadius;
    private Vector3 direction = Vector3.forward;
    public Vector3 Direction => direction;

    public enum ProjectileState
    {
        ShotByPlayer,
        ShotByEnemy,
        ParriedByPlayer,
        ParriedByEnemy
    };
    private ProjectileState state;
    public ProjectileState State => state;

    private Transform origin;
    private bool createdFromPool = true;
    private Stunner stunner;

    public float Speed => speed;

    private void Awake()
    {
        lane = GetComponent<LaneBound>();
        Assert.IsNotNull(lane);

        sprites = GetComponentsInChildren<Billboard>();
        Assert.IsTrue(sprites.Length > 0);

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        Assert.IsTrue(spriteRenderers.Length > 0);

        sphereCollider = GetComponent<SphereCollider>();
        Assert.IsNotNull(sphereCollider);
        normalColliderRadius = sphereCollider.radius;

        stunner = GetComponent<Stunner>();
        Assert.IsNotNull(stunner);
        stunner.OnStun += OnStun;
    }

    public void Initialize(Transform origin, Vector3 position, Vector3 direction, float speed, ProjectileState initialState)
    {
        this.origin = origin;
        transform.position = position;
        lane.LaneIndex = LaneSet.Instance.GetLaneIndex(position.x);
        lane.LaneDistance = position.z;
        lane.PerpendicularOffset = position.x - transform.position.x;

        foreach (var sprite in sprites)
            sprite.rotation = LaneSet.ScreenAngleOfVector(direction);

        this.direction = direction.normalized;
        this.speed = speed;
        state = initialState;
        sphereCollider.radius = normalColliderRadius;
        sphereCollider.enabled = true;
        enabled = true;
        createdFromPool = (state == ProjectileState.ShotByEnemy);
        stunner.enabled = (state == ProjectileState.ShotByEnemy || state == ProjectileState.ParriedByEnemy);
        stunner.ResetState();
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.EnemyWeaponShot, transform.position);
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        pos.y += speed * Time.deltaTime * direction.y;
        transform.position = pos;

        lane.LaneDistance += speed * Time.deltaTime * direction.z;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            return;

        if (state == ProjectileState.ParriedByEnemy || state == ProjectileState.ShotByEnemy)
            return;

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null || enemy.immuneToBullet?.Invoke(this) == true)
            return;

        AudioManager.Instance.PlayOneShot(impactEvent, transform.position);

        EnergyShield shield = enemy.GetShield();
        if (shield != null)
        {
            if (canPenetrateShield)
                shield.TakeDamage(damage);
        }
        else if (enemy.TakeDamage(damage))
            OnEnemyKill(enemy);

        Despawn();
    }

    private void OnStun()
    {
        Despawn();
    }

    public void Despawn()
    {
        if (!enabled)
            return;

        enabled = false;
        sphereCollider.enabled = false;

        IEnumerator Routine(SpriteRenderer spriteRenderer)
        {
            Color color = spriteRenderer.color;
            yield return FadeAnimation.FadeOutRoutine(spriteRenderer);
            spriteRenderer.color = color;
            if (createdFromPool)
                gameObject.SetActive(false);
            else
                Destroy(gameObject);
        }

        // TODO sfx ?
        foreach (var spriteRenderer in spriteRenderers)
            StartCoroutine(Routine(spriteRenderer));
    }

    public void Parry(Transform newOrigin, float speedMult, ProjectileState projectileState)
    {
        state = projectileState;

        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerSwordParry, transform.position);
        if (origin != null)
            direction = (origin.position - transform.position).normalized;
        else
            direction *= -1;

        origin = newOrigin;
        speed *= speedMult;

        foreach (var sprite in sprites)
            sprite.rotation = LaneSet.ScreenAngleOfVector(direction);

        sphereCollider.radius = parryColliderScaleUp * normalColliderRadius;

        stunner.enabled = !stunner.enabled;

        // TODO flash vfx
    }

    private void OnEnemyKill(Enemy enemy)
    {
        if (state == ProjectileState.ShotByPlayer)
        {
            GunPlayerController.Instance.AddContinuousMultiplier(GunPlayerController.Instance.GunKillMultiplierGain);
            GunPlayerController.Instance.AddScore(enemy.Score);
            PlayerStats.Instance.AddGunSuper(2f);
        }
        else if (state == ProjectileState.ParriedByPlayer)
        {
            SwordPlayerController.Instance.AddContinuousMultiplier(SwordPlayerController.Instance.BulletParryMultiplierGain);
            SwordPlayerController.Instance.AddScore(enemy.Score);
            PlayerStats.Instance.AddSwordSuper(2f);
        }
    }

    public bool IsComingFromPlayer()
    {
        return state == ProjectileState.ShotByPlayer || state == ProjectileState.ParriedByPlayer;
    }
}
