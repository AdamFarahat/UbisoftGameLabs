using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class EnergyShield : MonoBehaviour
{
    [SerializeField] private float probabilityToSpawn = 0.05f;
    [SerializeField] private int shieldMaxHealth = 5;

    private int shieldHealth = 0;
    private Enemy enemy;
    private new Collider collider;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        Assert.IsNotNull(enemy);
        collider = GetComponent<Collider>();
        Assert.IsNotNull(collider);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Assert.IsNotNull(spriteRenderer);

        enemy.OnTakeFromPool += TakeFromPool;
    }

    private void TakeFromPool()
    {
        gameObject.SetActive(Random.value < probabilityToSpawn);
        shieldHealth = shieldMaxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (shieldHealth <= 0)
            return;

        shieldHealth -= damage;
        if (shieldHealth <= 0)
        {
            IEnumerator Routine()
            {
                collider.enabled = false;
                yield return FadeOutAnimation.Routine(spriteRenderer);
                collider.enabled = true;
                gameObject.SetActive(false);
            }

            // TODO sfx ?
            StartCoroutine(Routine());
        }
    }
}
