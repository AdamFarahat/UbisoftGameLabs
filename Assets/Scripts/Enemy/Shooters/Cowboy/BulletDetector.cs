using System.Collections.Generic;
using UnityEngine;

public class BulletDetector : MonoBehaviour
{
    private List<Bullet> bulletsNearby = new List<Bullet>();

    public List<Bullet> NearbyBullets => bulletsNearby;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Bullet>(out Bullet b)) 
        {
            bulletsNearby.Add(b);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Bullet>(out Bullet b))
        {
            bulletsNearby.Remove(b);
        }
    }
    
}
