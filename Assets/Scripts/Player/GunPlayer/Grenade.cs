using System.Collections;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float gravity = 100f;
    public float velocity = 100f;
    [SerializeField] private Vector3 initialDirection = new(0f, 1f, 1f);
    [SerializeField] private float aoeRadiusScale = 100f;
    [SerializeField] private float explosionDuration = 0.5f;

    private float verticalVelocity = 0f;
    private float forwardVelocity = 0f;
    private bool dead = false;

    private void Start()
    {
        initialDirection.Normalize();
        verticalVelocity = velocity * initialDirection.y;
        forwardVelocity = velocity * initialDirection.z;
    }

    private void Update()
    {
        if (dead) return;

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
        IEnumerator Explosion()
        {
            for (float t = 0f; t < explosionDuration; t += Time.deltaTime)
            {
                float scale = Mathf.Lerp(1f, aoeRadiusScale, Mathf.Clamp01(t / explosionDuration));
                transform.localScale = new(scale, scale, scale);
                yield return null;
            }

            // TODO get references to enemies/obstacles inside AOE.
            Destroy(gameObject);
        }

        dead = true;
        StartCoroutine(Explosion());
    }
}
