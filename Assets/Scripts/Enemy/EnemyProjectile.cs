using UnityEngine;
using UnityEngine.Assertions;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private Billboard sprite;

    private float normalSpriteRotation;
    private Vector3 direction;
    private float speed = 80f;

    private void Awake()
    {
        sprite = GetComponentInChildren<Billboard>();
        Assert.IsNotNull(sprite);
        normalSpriteRotation = sprite.rotation;

        Stunner stunner = GetComponent<Stunner>();
        Assert.IsNotNull(stunner);
        stunner.OnStun += OnStun;
    }

    public void Initialize(Vector3 direction, float speed)
    {
        sprite.rotation = normalSpriteRotation;
        this.direction = direction.normalized;
        this.speed = speed;
    }

    void Update()
    {
        transform.position += speed * Time.deltaTime * direction;
    }

    private void OnStun()
    {
        gameObject.SetActive(false);
    }

    public void Parry(float speedMult)
    {
        direction *= -1;
        speed *= speedMult;
        sprite.rotation += 180;
    }
}
