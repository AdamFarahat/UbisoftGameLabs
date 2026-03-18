using UnityEngine;

public class FlyerShooting : ShooterEnemy
{
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
}
