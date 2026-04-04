using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletDetector : MonoBehaviour
{
    private List<Bullet> bulletsNearby = new();
    public List<Bullet> NearbyBullets => bulletsNearby;

    private void Awake()
    {
        this.GetComponentInHierarchy<Enemy>().OnTakeFromPool += () => { bulletsNearby.Clear(); };
    }

    private void Update()
    {
        bulletsNearby = bulletsNearby.Where(b => b != null).ToList();
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
