using UnityEngine;
using UnityEngine.Pool;

public abstract class Poolable : MonoBehaviour
{
    protected IObjectPool<GameObject> enemyPool;

    public void SetPool(IObjectPool<GameObject> pool) 
    {
        enemyPool = pool;
    }

    protected void Release()
    {
        enemyPool.Release(gameObject);
    }
    
    public virtual void OnTakeFromPool()
    {
        // Default: do nothing
    }
}
