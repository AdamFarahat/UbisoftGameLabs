using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 10f;
    private Vector3 direction;

    public void Initialize(Vector3 dir)
    {
        direction = dir.normalized;

        AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyWeaponShot, transform.position);
    }

    void Update()
    {
        transform.position += speed * Time.deltaTime * direction;
    }

    public void FlipDirection()
    {
        direction *= -1;
    }
}