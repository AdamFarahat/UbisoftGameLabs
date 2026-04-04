using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class LaserShot : MonoBehaviour
{
    [SerializeField] private float duration = 0.4f;
    [SerializeField] private int fullDamage = 50;
    [SerializeField] private int penetrationDamageLoss = 6;

    public Transform fakeParent;
    private readonly HashSet<Enemy> enemiesHitThisFrame = new();

    private void Start()
    {
        IEnumerator Routine()
        {
            yield return new WaitForSeconds(duration);
            Destroy(gameObject);
        }

        StartCoroutine(Routine());
    }

    private void Update()
    {
        transform.position = fakeParent.position;
    }

    private void LateUpdate()
    {
        if (enemiesHitThisFrame.Count == 0)
            return;

        int enemiesHit = 0;
        foreach (Enemy enemy in enemiesHitThisFrame.OrderBy(c => c.transform.position.z))
        {
            int damage = fullDamage - (enemiesHit++) * penetrationDamageLoss;
            if (damage <= 0)
                break;

            if (enemy.TakeDamage(damage))
            {
                GunPlayerController.Instance.AddContinuousMultiplier(GunPlayerController.Instance.GunKillMultiplierGain);
                GunPlayerController.Instance.AddScore(enemy.Score);
            }
        }

        enemiesHitThisFrame.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null && !enemy.HasShield() && !other.GetComponentInParent<LaserImmune>())
            enemiesHitThisFrame.Add(enemy);
    }
}
