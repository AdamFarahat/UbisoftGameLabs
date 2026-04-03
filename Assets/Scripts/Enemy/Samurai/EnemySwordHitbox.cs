using UnityEngine;

public class EnemySwordHitbox : MonoBehaviour
{
    public SamuraiEnemy samuraiEnemy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.transform.position = samuraiEnemy.transform.position + samuraiEnemy.transform.forward * 2.5f + samuraiEnemy.transform.up * 2.5f;
        gameObject.SetActive(false); // Start with the hitbox disabled
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = samuraiEnemy.transform.position + samuraiEnemy.transform.forward * 2.5f + samuraiEnemy.transform.up * 2.5f;
    }

    private void OnTriggerEnter(Collider collider)
    {
        samuraiEnemy.OnSwordHitBoxTriggerEnter(collider);
    }
}
