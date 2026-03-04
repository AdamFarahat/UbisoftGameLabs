using UnityEngine;
using UnityEngine.Assertions;

public class EnergyShield : MonoBehaviour
{
    [SerializeField] private float probabilityToSpawn = 0.05f;
    [SerializeField] private int shieldMaxHealth = 5;

    private int shieldHealth = 0;
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        Assert.IsNotNull(enemy);
        enemy.OnTakeFromPool += TakeFromPool;
    }

    private void TakeFromPool()
    {
        gameObject.SetActive(Random.value < probabilityToSpawn);
        shieldHealth = shieldMaxHealth;
    }

    public void TakeDamage(int damage)
    {
        shieldHealth -= damage;
        if (shieldHealth <= 0)
            gameObject.SetActive(false);  // TODO sfx/animation
    }
}
