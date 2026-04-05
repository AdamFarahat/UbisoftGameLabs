using UnityEngine;

public class FlyerShooting : ShooterEnemy
{
    [SerializeField] private float maxShotCooldown = 3f;
    [SerializeField] private float minShotCooldown = 0.3f;
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
    }
}
