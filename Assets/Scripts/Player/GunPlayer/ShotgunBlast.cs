using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ShotgunBlast : MonoBehaviour
{
    [SerializeField] private new ParticleSystem particleSystem;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask nearLayer;
    [SerializeField] private Collider[] ignoreColliders;
    [SerializeField] private Collider movingCollider;
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
        Assert.IsNotNull(movingCollider);

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
        particleSystem.transform.localScale *= range;
    }

    private void Update()
    {
        if (particleSystem.isStopped)
            Destroy(gameObject);

        age += Time.deltaTime;

        float a = Mathf.Clamp01(age * invDuration);

        Vector3 pos = movingCollider.transform.position;
        pos.z = Mathf.Lerp(0, range, a);
        movingCollider.transform.position = pos;

        float w = Mathf.Tan(Mathf.Deg2Rad * coneAngle) * pos.z;
        Vector3 scale = movingCollider.transform.localScale;
        scale.x = w;
        scale.y = w * heightScale;
        movingCollider.transform.localScale = scale;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);

        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null && !enemiesHit.Contains(enemy) && !enemy.HasShield())
        {
            enemiesHit.Add(enemy);
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerRevolverImpact, transform.position);
            if (enemy.TryGetComponentInHierarchy(out ShotgunImmune si) && si.isActiveAndEnabled)
                si.HitByShotgun?.Invoke();
            else if (enemy.TakeDamage(damage))
                OnEnemyKill(enemy);
        }
    }

    private void OnEnemyKill(Enemy enemy)
    {
        GunPlayerController.Instance.AddContinuousMultiplier(GunPlayerController.Instance.GunKillMultiplierGain);
        GunPlayerController.Instance.AddScore(enemy.Score);
    }
}
