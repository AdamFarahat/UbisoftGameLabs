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
        collider = GetComponentInChildren<Collider>();
        Assert.IsNotNull(collider);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Assert.IsNotNull(spriteRenderer);

        enemy.OnTakeFromPool += TakeFromPool;
        enemy.Die += Die;
    }

    private void Start()
    {
        if (!enemy.TryGetComponent(out Poolable poolable) || !poolable.HasPool)
        {
            gameObject.SetActive(Random.value < probabilityToSpawn);
            shieldHealth = shieldMaxHealth;
        }
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

        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.EnemyShieldHit, transform.position);    
        shieldHealth -= damage;
        if (shieldHealth <= 0)
            Die();
    }

    private void Die()
    {
        IEnumerator Routine()
        {
            collider.enabled = false;
            yield return FadeAnimation.FadeOutRoutine(spriteRenderer);
            collider.enabled = true;
            gameObject.SetActive(false);
        }

        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.EnemyShieldBroken, transform.position);
        if (gameObject.activeSelf)
            StartCoroutine(Routine());
    }
}
