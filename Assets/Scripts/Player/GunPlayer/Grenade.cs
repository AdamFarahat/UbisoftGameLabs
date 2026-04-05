using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Grenade : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private Transform colliderRoot;
    [SerializeField] private GameObject vfx;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float aoeRadiusScale = 100f;
    [SerializeField] private float explosionDuration = 0.5f;
    [SerializeField] private float gravity = 300f;

    public Vector3 initialDirection = new(0f, 1f, 1f);
    public float range = 100f;
    private float verticalVelocity = 0f;
    private float forwardVelocity = 0f;
    private bool exploding = false;

    private readonly HashSet<Enemy> hitEnemies = new();

    private void Awake()
    {
        Assert.IsNotNull(colliderRoot);
        Assert.IsNotNull(vfx);
        Assert.IsNotNull(spriteRenderer);

        vfx.SetActive(false);
    }

    private void Start()
    {
        initialDirection.Normalize();

        float denominator = 2f * (range * initialDirection.y * initialDirection.z + transform.position.y * initialDirection.z * initialDirection.z);
        float velocity = range * Mathf.Sqrt(gravity / denominator);

        verticalVelocity = velocity * initialDirection.y;
        forwardVelocity = velocity * initialDirection.z;
    }

    private void Update()
    {
        if (exploding) return;

        if (transform.position.y <= 0f)
        {
            Explode();
            return;
        }

        Vector3 position = transform.position;
        position.z += forwardVelocity * Time.deltaTime;
        position.y = Mathf.Max(position.y + verticalVelocity * Time.deltaTime, 0f);
        transform.position = position;
        verticalVelocity -= gravity * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<GrenadeImmune>())
            return;

        if (exploding)
        {
            Enemy enemy = other.GetComponentInParent<Enemy>();
            if (enemy != null && !hitEnemies.Contains(enemy))
            {
                hitEnemies.Add(enemy);
                if (enemy.HasShield())
                {
                    Vector3 axis = LaneSet.Instance.GetLaneDirection() * Vector3.forward;
                    float myPosition = Vector3.Dot(transform.position, axis);
                    float shieldPosition = Vector3.Dot(enemy.GetShield().transform.position, axis);
                    if (myPosition <= shieldPosition)
                        return;
                }

                if (enemy.TakeDamage(damage))
                {
                    PlayerStats.Instance.AddGunSuper(5f);
                    OnEnemyKill(enemy);
                }
            }
        }
        else if (!other.GetComponentInParent<PlayerController>())
            Explode();
    }

    private void Explode()
    {
        IEnumerator Explosion()
        {
            spriteRenderer.gameObject.SetActive(false);
            vfx.SetActive(true);

            for (float t = 0f; t < explosionDuration; t += Time.deltaTime)
            {
                float scale = Mathf.Lerp(1f, aoeRadiusScale, Mathf.Clamp01(t / explosionDuration));
                colliderRoot.localScale = new(scale, scale, scale);
                yield return null;
            }

            Destroy(gameObject);
        }

        exploding = true;
        transform.position = new(transform.position.x, 0f, transform.position.z);

        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerGrenadeExplode, transform.position);

        StartCoroutine(Explosion());
    }

    private void OnEnemyKill(Enemy enemy)
    {
        GunPlayerController.Instance.AddContinuousMultiplier(GunPlayerController.Instance.GrenadeKillMultiplierGain);
        GunPlayerController.Instance.AddScore(enemy.Score);
    }
}
