using FMODUnity;
using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

public class Bullet : MonoBehaviour
{
    [SerializeField] private bool canPenetrateShield = false;
    public int damage = 10;



    public EventReference impactEvent;

    private Billboard sprite;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float parryColliderScaleUp = 1.5f;
    [SerializeField] private float speed = 80f;
    private SphereCollider sphereCollider;
    private float normalColliderRadius;
    private Vector3 direction = Vector3.forward;
    public Vector3 Direction => direction;
    public enum ProjectileState {
        ShotByPlayer,
        ShotByEnemy,
        ParriedByPlayer,
        ParriedByEnemy
    };
    protected ProjectileState state;
    public ProjectileState State => state;
    private Transform origin;
    protected bool createdFromPool = true;
    protected Stunner stunner;



    public float Speed => speed;
    private void Awake()
    {
        sprite = GetComponentInChildren<Billboard>();
        Assert.IsNotNull(sprite);

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Assert.IsNotNull(spriteRenderer);

        sphereCollider = GetComponent<SphereCollider>();
        Assert.IsNotNull(sphereCollider);
        normalColliderRadius = sphereCollider.radius;

        stunner = GetComponent<Stunner>();
        Assert.IsNotNull(stunner);
        stunner.OnStun += OnStun;
    }
   

    public void Initialize(Transform origin, Vector3 direction, float speed, ProjectileState initialState)
    {
        this.origin = origin;
        sprite.rotation = LaneSet.ScreenAngleOfVector(direction);
        this.direction = direction.normalized;
        this.speed = speed;
        state = initialState;
        sphereCollider.radius = normalColliderRadius;
        enabled = true;
        createdFromPool = (state == ProjectileState.ShotByEnemy);
        stunner.enabled = (state == ProjectileState.ShotByEnemy || state == ProjectileState.ParriedByEnemy);
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.EnemyWeaponShot, transform.position);
    }

    private void Update()
    {
        transform.position += speed * Time.deltaTime * direction;
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            return;

        if (state == ProjectileState.ParriedByEnemy || state == ProjectileState.ShotByEnemy)
        {
            return;
        }
        

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


    private void OnStun()
    {
        Despawn();
    }

    protected void Despawn()
    {
        enabled = false;

        IEnumerator Routine()
        {
            Color color = spriteRenderer.color;
            yield return FadeAnimation.FadeOutRoutine(spriteRenderer);
            spriteRenderer.color = color;
            if (createdFromPool)
            {
                gameObject.SetActive(false);
            }
            else { 
            Destroy(gameObject);
            }
        }

        // TODO sfx ?
        StartCoroutine(Routine());
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
        sprite.rotation = LaneSet.ScreenAngleOfVector(direction);
        
        sphereCollider.radius = parryColliderScaleUp * normalColliderRadius;
        
        stunner.enabled = !stunner.enabled;
    }


    
    private void OnEnemyKill(Enemy enemy)
    {
        // TODO handle more complex gun player multiplier logic
        GunPlayerController.Instance.AddContinuousMultiplier(GunPlayerController.Instance.GunKillMultiplierGain);
        GunPlayerController.Instance.AddScore(enemy.Score);
    }
}
