using UnityEngine;
using UnityEngine.Pool;

public abstract class Poolable : MonoBehaviour
{
    protected IObjectPool<GameObject> enemyPool;

    public void SetPool(IObjectPool<GameObject> pool) 
    {
        enemyPool = pool;
    }

    public void Death()
    {
        enemyPool.Release(gameObject);
    }
    
    public virtual void TakeFromPool()
    {
        // Default: do nothing
    }
}
