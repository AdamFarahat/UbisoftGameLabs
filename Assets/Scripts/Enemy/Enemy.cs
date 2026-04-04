using System.Collections;
using System.Collections.Generic;
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
    public UnityAction Die;

    public bool invulnerable = false;
    public float deathAnimationDuration = 0.1f;

    private LaneBound laneBound;
    private SpriteRenderer spriteRenderer;
    private EnergyShield energyShield;

    private void Awake()
    {
        laneBound = GetComponent<LaneBound>();
        Assert.IsNotNull(laneBound);
        spriteRenderer = this.GetComponentInHierarchy<SpriteRenderer>();
        energyShield = this.GetComponentInHierarchy<EnergyShield>();
    }

    private void Start()
    {
        if (enemyPool == null)
            ResetState();
    }

    private void Update()
    {
        if (laneBound.LaneDistance <= LaneSet.HeartLine)
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

        laneBound.LaneIndex = Random.Range(0, LaneSet.LaneCount);
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.EnemySpawn, transform.position);
    }

    // returns true if enemy was killed by damage
    public bool TakeDamage(int damage)
    {
        if (Dead || invulnerable) return false;

        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.EnemyHurt, transform.position);
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

        IEnumerator DeathRoutine()
        {
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                yield return FadeAnimation.FadeOutRoutine(spriteRenderer, deathAnimationDuration);
                spriteRenderer.color = color;
            }
        }

        if (TryGetComponent(out Poolable poolable))
            poolable.Death(DeathRoutine());
        else
        {
            IEnumerator DestroyRoutine()
            {
                yield return DeathRoutine();
                Destroy(gameObject);
            }

            StartCoroutine(DestroyRoutine());
        }

        Die?.Invoke();
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
