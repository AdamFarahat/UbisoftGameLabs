using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;

public class ProjectileBase : MonoBehaviour
{
    private Billboard sprite;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float parryColliderScaleUp = 1.5f;
    [SerializeField] private float speed = 80f;
    private SphereCollider sphereCollider;
    private float normalColliderRadius;
    private Vector3 direction = Vector3.forward;
    
    private bool parried = false;
    private bool parriedBySwordPlayer = false;
    public bool Parried => parried;
    public bool ParriedBySwordPlayer => parriedBySwordPlayer;
    private Transform origin;
    protected bool createdFromPool = true;
    protected Stunner stunner;



    public float Speed => speed;
    private void Awake()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.EnemyWeaponShot, transform.position);
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

    public void Initialize(Transform origin, Vector3 direction, float speed, bool stun = true)
    {
        this.origin = origin;
        sprite.rotation = LaneSet.ScreenAngleOfVector(direction);
        this.direction = direction.normalized;
        this.speed = speed;
        parried = false;
        sphereCollider.radius = normalColliderRadius;
        enabled = true;
        stunner.enabled = stun;
    }

    private void Update()
    {
        transform.position += speed * Time.deltaTime * direction;
    }

    
    virtual protected void OnTriggerEnter(Collider other)
    {
        if (!parried)
            return;

        if (other.TryGetComponentInHierarchy(out Enemy enemy))
        {
            if (enemy.OnParried()) { 
                //TODO: Impact Sound effect
                SwordPlayerController.Instance.OnBulletParryKill(enemy.Score);
                }
            Despawn();
        }
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

    public void Parry(Transform newOrigin, float speedMult, bool isBySwordPlayer)
    {
        parriedBySwordPlayer = isBySwordPlayer;
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerSwordParry, transform.position);
        if (origin != null)
            direction = (origin.position - transform.position).normalized;
        else
            direction *= -1;

        origin = newOrigin;
        speed *= speedMult;
        sprite.rotation = LaneSet.ScreenAngleOfVector(direction);
        parried = true;
        
        sphereCollider.radius = parryColliderScaleUp * normalColliderRadius;
        
        stunner.enabled = !stunner.enabled;
    }
}
