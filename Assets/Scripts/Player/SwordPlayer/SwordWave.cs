using UnityEngine;

public class SwordWave : MonoBehaviour
{
    public float velocity = 100f;
    [SerializeField] private float acceleration = 0f;
    [SerializeField] private float range = 400f;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            return;
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy == null)
            return;
        enemy.OnParried(); // Instantly kill the enemy hit by the sword wave
    }
}
