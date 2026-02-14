using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    private Vector3 direction;

    public void Initialize(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.position += speed * Time.deltaTime * direction;
    }
}