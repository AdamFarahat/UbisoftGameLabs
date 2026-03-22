using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyProjectile : MonoBehaviour
{
    private Billboard sprite;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float parryColliderScaleUp = 1.5f;

    private SphereCollider sphereCollider;
    private float normalColliderRadius;
    private Vector3 direction;
    private float speed = 80f;
    private bool parried = false;
    private Transform origin;

    private void Awake()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyWeaponShot, transform.position);
        sprite = GetComponentInChildren<Billboard>();
        Assert.IsNotNull(sprite);

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Assert.IsNotNull(spriteRenderer);

        sphereCollider = GetComponent<SphereCollider>();
        Assert.IsNotNull(sphereCollider);
        normalColliderRadius = sphereCollider.radius;

        Stunner stunner = GetComponent<Stunner>();
        Assert.IsNotNull(stunner);
        stunner.OnStun += OnStun;
    }

    public void Initialize(Transform origin, Vector3 direction, float speed)
    {
        this.origin = origin;
        sprite.rotation = LaneSet.ScreenAngleOfVector(direction);
        this.direction = direction.normalized;
        this.speed = speed;
        parried = false;
        sphereCollider.radius = normalColliderRadius;
        enabled = true;
    }

    void Update()
    {
        transform.position += speed * Time.deltaTime * direction;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!parried)
            return;

        if (other.TryGetComponentInHierarchy(out Enemy enemy))
        {
            if (enemy.OnParried())
                SwordPlayerController.Instance.OnBulletParryKill(enemy.Score);
            Despawn();
        }
    }

    private void OnStun()
    {
        Despawn();
    }

    private void Despawn()
    {
        enabled = false;

        IEnumerator Routine()
        {
            Color color = spriteRenderer.color;
            yield return FadeOutAnimation.Routine(spriteRenderer);
            spriteRenderer.color = color;
            gameObject.SetActive(false);
        }

        // TODO sfx ?
        StartCoroutine(Routine());
    }

    public void Parry(Transform newOrigin, float speedMult)
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.playerSwordParry, transform.position);
        if (origin != null)
            direction = (origin.position - transform.position).normalized;
        else
            direction *= -1;

        origin = newOrigin;
        speed *= speedMult;
        sprite.rotation = LaneSet.ScreenAngleOfVector(direction);
        parried = true;
        sphereCollider.radius = parryColliderScaleUp * normalColliderRadius;
    }
}
