using System.Runtime.CompilerServices;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float velocity = 100f;
    public float acceleration = 0f;
    public float range = 100f;
    public int damage = 10;

    private float distance = 0f;

    private void Update()
    {
        float deltaDistance = velocity * Time.deltaTime;
        distance += deltaDistance;
        if (distance > range)
            Destroy(gameObject);

        transform.position += deltaDistance * transform.forward;
        velocity += acceleration * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<DemoEnemy>() != null)
        {
            other.GetComponentInParent<DemoEnemy>().Death();
            Destroy(gameObject);
        }
    }
}
