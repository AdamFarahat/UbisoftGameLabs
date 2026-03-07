using UnityEngine;
using UnityEngine.Assertions;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 10f;
    private Vector3 direction;

    public void Initialize(Vector3 dir)
    {
        direction = dir.normalized;
    }

    private void Awake()
    {
        Stunner stunner = GetComponent<Stunner>();
        Assert.IsNotNull(stunner);
        stunner.OnStun += OnStun;
    }

    void Update()
    {
        transform.position += speed * Time.deltaTime * direction;
    }

    private void OnStun()
    {
        gameObject.SetActive(false);
    }

    public void FlipDirection()
    {
        direction *= -1;
    }
}
