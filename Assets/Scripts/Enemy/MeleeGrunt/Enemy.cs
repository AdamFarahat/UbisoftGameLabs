using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int health = 10;

    private bool dead = false;
    public bool Dead => dead;

    public void TakeDamage(int damage)
    {
        if (Dead) return;

        health = System.Math.Max(health - damage, 0);
        if (health == 0)
            OnDeath();
    }

    public int GetHealth()
    {
        return health;
    }

    public void OnParried()
    {
        TakeDamage(health);
    }

    private void OnDeath()
    {
        dead = true;
        Poolable poolable = GetComponent<Poolable>();
        if (poolable != null)
            poolable.Death();
        else
            Destroy(gameObject);
    }
}
