using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletDetector : MonoBehaviour
{
    private List<Bullet> bulletsNearby = new();
    public List<Bullet> NearbyBullets => bulletsNearby;

    private List<ShotgunBlast> nearbyShotgunBlasts = new();
    public List<ShotgunBlast> NearbyShotgunBlasts => nearbyShotgunBlasts;

    private void Awake()
    {
        this.GetComponentInHierarchy<Enemy>().OnTakeFromPool += ResetState;
    }

    private void ResetState()
    {
        bulletsNearby.Clear();
        nearbyShotgunBlasts.Clear();
    }

    private void Update()
    {
        bulletsNearby = bulletsNearby.Where(b => b != null).ToList();
        nearbyShotgunBlasts = nearbyShotgunBlasts.Where(s => s != null).ToList();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Bullet b))
            bulletsNearby.Add(b);
        else if (other.TryGetComponent(out ShotgunBlast s))
            nearbyShotgunBlasts.Add(s);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Bullet b))
            bulletsNearby.Remove(b);
        else if (other.TryGetComponent(out ShotgunBlast s))
            nearbyShotgunBlasts.Remove(s);
    }
}
