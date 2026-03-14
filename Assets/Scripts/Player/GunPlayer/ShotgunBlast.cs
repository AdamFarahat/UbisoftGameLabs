using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ShotgunBlast : MonoBehaviour
{
    [SerializeField] private new ParticleSystem particleSystem;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Collider[] ignoreColliders;
    private readonly HashSet<Enemy> enemiesHit = new();

    public float coneAngle = 45f;
    public float range = 100f;
    public int damage = 10;
    public float heightScale = 0.2f;

    private float invDuration = 0f;
    private float age = 0f;
    private float invVerticalScale = 1f;

    private void Awake()
    {
        Assert.IsNotNull(particleSystem);

        foreach (Collider collider in ignoreColliders)
            collider.enabled = false;
    }

    private void Start()
    {
        var shape = particleSystem.shape;
        shape.angle = coneAngle;
        shape.scale = new(shape.scale.x, heightScale, shape.scale.z);
        
        invDuration = 1f / particleSystem.main.startLifetime.constant;
        invVerticalScale = 1f / particleSystem.shape.scale.y;

        particleSystem.Play();
        transform.localScale = new(range, range, range);
    }

    private void Update()
    {
        if (particleSystem.isStopped)
            Destroy(gameObject);

        age += Time.deltaTime;

        float a = Mathf.Clamp01(age * invDuration);
        float interpRange = Mathf.Lerp(0f, range, a);
        Vector3 forward = transform.forward;
        forward.y *= invVerticalScale;
        forward.Normalize();

        Collider[] hits = Physics.OverlapSphere(transform.position, interpRange, enemyLayer);
        foreach (Collider hit in hits)
        {
            Vector3 displacement = hit.transform.position - transform.position;
            displacement.y *= invVerticalScale;
            if (Vector3.Angle(displacement, forward) < coneAngle && Vector3.Dot(displacement, forward) <= interpRange) // enemy is within cone range
                OnTriggerEnter(hit);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null && !enemiesHit.Contains(enemy) && !enemy.HasShield())
        {
            enemiesHit.Add(enemy);
            if (enemy.TakeDamage(damage))
                OnEnemyKill(enemy);
        }
    }

    private void OnEnemyKill(Enemy enemy)
    {
        // TODO handle more complex gun player multiplier logic ?
        GunPlayerController.Instance.AddContinuousMultiplier(GunPlayerController.Instance.GunKillMultiplierGain);
        GunPlayerController.Instance.AddScore(enemy.Score);
    }
}
