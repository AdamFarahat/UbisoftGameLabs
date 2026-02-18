using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float gravity = 100f;
    public float velocity = 100f;
    [SerializeField] private Vector3 initialDirection = new(0f, 1f, 1f);

    private float verticalVelocity = 0f;
    private float forwardVelocity = 0f;

    private void Start()
    {
        initialDirection.Normalize();
        verticalVelocity = velocity * initialDirection.y;
        forwardVelocity = velocity * initialDirection.z;
    }

    private void Update()
    {
        if (transform.position.y <= 0f)
            Explode();

        Vector3 position = transform.position;
        position.z += forwardVelocity * Time.deltaTime;
        position.y += verticalVelocity * Time.deltaTime;
        transform.position = position;
        verticalVelocity -= gravity * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            Explode();
    }

    private void Explode()
    {
        // TODO get references to enemies/obstacles inside AOE.
        Destroy(gameObject);
    }
}
