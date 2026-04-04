using UnityEngine;
using UnityEngine.Assertions;

public class EnemySwordHitbox : MonoBehaviour
{
    public SamuraiEnemy samuraiEnemy;

    private void Awake()
    {
        Assert.IsNotNull(samuraiEnemy);
    }

    void Start()
    {
        gameObject.SetActive(false); // Start with the hitbox disabled
    }

    private void OnTriggerEnter(Collider collider)
    {
        samuraiEnemy.OnSwordHitBoxTriggerEnter(collider);
    }
}
