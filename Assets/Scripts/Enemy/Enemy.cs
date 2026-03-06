using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class Enemy : Poolable
{
    [SerializeField] protected int maxHealth = 10;
    [SerializeField] protected int health = 10;
    [SerializeField] protected int score = 10;

    public int Health => health;
    public int Score => score;

    public UnityAction OnTakeFromPool;

    private bool dead = false;
    public bool Dead => dead;

    private LaneBound laneBound;
    private EnergyShield energyShield;

    private void Awake()
    {
        laneBound = GetComponent<LaneBound>();
        Assert.IsNotNull(laneBound);
        energyShield = this.GetComponentInHierarchy<EnergyShield>();
    }

    private void Start()
    {
        if (enemyPool != null)
            ResetState();
    }

    private void Update()
    {
        if (laneBound.LaneDistance <= 0f)
            Death();
    }

    public override void TakeFromPool()
    {
        base.TakeFromPool();
        ResetState();
        OnTakeFromPool?.Invoke();
    }

    private void ResetState()
    {
        health = maxHealth;
        dead = health <= 0;

        laneBound.LaneIndex = Random.Range(0, LaneConfigSO.Instance.GetNumberOfLanes());
    }

    // returns true if enemy was killed by damage
    public bool TakeDamage(int damage)
    {
        if (Dead) return false;

        health = System.Math.Max(health - damage, 0);
        if (health == 0)
        {
            OnDeath();
            return true;
        }
        else
            return false;
    }

    public bool OnParried()
    {
        return TakeDamage(health);
    }

    public void Kill()
    {
        TakeDamage(health);
    }

    private void OnDeath()
    {
        if (dead)
            return;

        dead = true;
        // TODO Play Death Animation
        if (TryGetComponent(out Poolable poolable))
            poolable.Death();
        else
            Destroy(gameObject);
    }

    public EnergyShield GetShield()
    {
        if (HasShield())
            return energyShield;
        else
            return null;
    }

    public bool HasShield()
    {
        return energyShield != null && energyShield.isActiveAndEnabled;
    }
}
