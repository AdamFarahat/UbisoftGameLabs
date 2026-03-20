using System.Collections;
using UnityEngine;

class LaserShot : MonoBehaviour
{
    [SerializeField] private float duration = 0.4f;
    [SerializeField] private int closestDamage = 50;
    [SerializeField] private float damageRange = 200f;
    [SerializeField] private float damageFalloffExp = 3f;  // if this is high, farther enemies take more damage

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Fire()
    {
        if (gameObject.activeSelf)
            return;

        IEnumerator Routine()
        {
            yield return new WaitForSeconds(duration);
            gameObject.SetActive(false);
        }

        gameObject.SetActive(true);
        StartCoroutine(Routine());
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null)
            return;

        float damage = 0f;
        float z = enemy.transform.position.z;
        if (z < LaneSet.PlayerLine)
            damage = closestDamage;
        else if (z < damageRange)
            damage = closestDamage * (1f - Mathf.Pow(z / damageRange, damageFalloffExp));

        if (enemy.TakeDamage(Mathf.FloorToInt(damage)))
        {
            GunPlayerController.Instance.AddContinuousMultiplier(GunPlayerController.Instance.GunKillMultiplierGain);
            GunPlayerController.Instance.AddScore(enemy.Score);
        }
    }
}
