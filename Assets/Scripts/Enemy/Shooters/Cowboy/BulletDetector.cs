using System.Collections.Generic;
using UnityEngine;

public class BulletDetector : MonoBehaviour
{
    private readonly List<Bullet> bulletsNearby = new();
    private readonly List<ShotgunBlast> shotgunBlast = new();
    public List<Bullet> NearbyBullets => bulletsNearby;

    private void Update()
    {
        List<int> oldBullets = new();
        for (int i = 0; i < bulletsNearby.Count; i++)
            if (bulletsNearby[i] == null)
                oldBullets.Add(i);

        oldBullets.Reverse();
        foreach (int index in oldBullets)
            bulletsNearby.RemoveAt(index);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Bullet b))
            bulletsNearby.Add(b);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Bullet b))
            bulletsNearby.Remove(b);
    }
}
