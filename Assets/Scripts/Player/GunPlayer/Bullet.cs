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
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            if (enemy.TakeDamage(damage))
                OnEnemyKill(enemy);
            Destroy(gameObject);
        }
    }

    private void OnEnemyKill(Enemy enemy)
    {
        GunPlayerController.Instance.UpdateScore(ScoreManager.DEFAULT_BULLET_MULTIPLIER , enemy.Score);
    }
}
