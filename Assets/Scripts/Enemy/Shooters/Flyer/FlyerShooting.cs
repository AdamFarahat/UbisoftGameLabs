using UnityEngine;

public class FlyerShooting : ShooterEnemy
{
    [SerializeField] private float maxShotCooldown = 3f;
    [SerializeField] private float minShotCooldown = 0.3f;
    [SerializeField] private float minBulletSpeed = 40f;
    [SerializeField] private float maxBulletSpeed = 120f;
    private float lastShotTime = 0f;

    private void Update()
    {
        if (IsInShootingRange())
        {
            if (Time.time - lastShotTime > shotCooldown)
            {
                Shoot();
                lastShotTime = Time.time;
            }
        }
    }

    public void RefreshShotCooldown(float difficulty)
    {
        shotCooldown = Mathf.Lerp(maxShotCooldown, minShotCooldown, difficulty);
        bulletSpeed = Mathf.Lerp(minBulletSpeed, maxBulletSpeed, difficulty);
    }
}
