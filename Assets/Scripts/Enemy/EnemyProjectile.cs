using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemyProjectile : MonoBehaviour
{
    private Billboard sprite;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float parryColliderScaleUp = 1.5f;

    private float normalSpriteRotation;
    private SphereCollider sphereCollider;
    private float normalColliderRadius;
    private Vector3 direction;
    private float speed = 80f;
    private bool parried = false;

    private void Awake()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyWeaponShot, transform.position);
        sprite = GetComponentInChildren<Billboard>();
        Assert.IsNotNull(sprite);
        normalSpriteRotation = sprite.rotation;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Assert.IsNotNull(spriteRenderer);

        sphereCollider = GetComponent<SphereCollider>();
        Assert.IsNotNull(sphereCollider);
        normalColliderRadius = sphereCollider.radius;

        Stunner stunner = GetComponent<Stunner>();
        Assert.IsNotNull(stunner);
        stunner.OnStun += OnStun;
    }

    public void Initialize(Vector3 direction, float speed)
    {
        sprite.rotation = normalSpriteRotation;
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

    public void Parry(float speedMult)
    {
        direction *= -1;
        speed *= speedMult;
        sprite.rotation += 180;
        parried = true;
        sphereCollider.radius = parryColliderScaleUp * normalColliderRadius;
    }
}
