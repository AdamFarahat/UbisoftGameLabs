using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 direction = Vector3.forward;
    public float velocity = 1.0f;
    public float acceleration = 0.0f;
    public float lifetime = 3.0f;

    private float age = 0.0f;

    private void Start()
    {
        direction.Normalize();
    }

    private void Update()
    {
        age += Time.deltaTime;
        if (age > lifetime)
            Destroy(gameObject);

        transform.position += Time.deltaTime * velocity * direction;
        velocity += acceleration * Time.deltaTime;
    }
}
