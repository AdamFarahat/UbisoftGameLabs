using System.Collections.Generic;
using UnityEngine;

public class BulletDetector : MonoBehaviour
{
    public List<Bullet> bulletsNearby = new List<Bullet>();

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
