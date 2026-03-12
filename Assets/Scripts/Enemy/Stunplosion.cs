using UnityEngine;
using UnityEngine.Assertions;

public class Stunplosion : MonoBehaviour
{
    [SerializeField] private Transform aoe;
    public float stunTime = 1f;
    public float radius = 10f;
    public float duration = 0.3f;

    private float age = 0f;

    private void Awake()
    {
        Assert.IsNotNull(aoe);
        aoe.localScale = Vector3.zero;
    }

    private void Update()
    {
        age += Time.deltaTime;
        aoe.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * radius, Mathf.Clamp01(age / duration));

        if (age > duration)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player != null)
            player.Stun(stunTime);
    }
}
