using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ShotgunBlast : MonoBehaviour
{
    [SerializeField] private new ParticleSystem particleSystem;
    [SerializeField] private LayerMask enemyLayer;
    private readonly HashSet<Enemy> enemiesHit = new();

    public float coneAngle = 45f;
    public float range = 100f;
    public int damage = 10;

    private float invDuration = 0f;
    private float age = 0f;
    private float invVerticalScale = 1f;

    private void Awake()
    {
        Assert.IsNotNull(particleSystem);
    }

    private void Start()
    {
        var shape = particleSystem.shape;
        shape.angle = coneAngle;
        
        invDuration = 1f / particleSystem.main.duration;
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
            {
                Enemy enemy = hit.GetComponentInParent<Enemy>();
                if (enemy != null && !enemiesHit.Contains(enemy))
                {
                    enemiesHit.Add(enemy);
                    if (enemy.TakeDamage(damage))
                        OnEnemyKill(enemy);
                }
            }
        }
    }

    private void OnEnemyKill(Enemy enemy)
    {
        GunPlayerController.Instance.UpdateScore(ScoreManagerSO.Instance.SHOTGUN_BLAST_MULTIPLIER, enemy.Score);
    }
}
