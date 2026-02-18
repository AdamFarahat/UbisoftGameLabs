using UnityEngine;
using UnityEngine.Pool;

public class ShooterEnemyAI : MonoBehaviour, IPoolable
{
    private IObjectPool<GameObject> enemyPool;

    public void SetPool(IObjectPool<GameObject> pool)
    {
        enemyPool = pool;
    }

    public void Release()
    {
        enemyPool.Release(gameObject);
    }

    public GameObject shootingLane;
}
