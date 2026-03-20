using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int maxEnemies = 12;

    private IObjectPool<GameObject>[] enemyPools;
    private int currentEnemies = 0;

    private void Awake()
    {
        Assert.IsTrue(enemyPrefabs.Length > 0);

        enemyPools = new IObjectPool<GameObject>[enemyPrefabs.Length];
        for (int i = 0; i < enemyPrefabs.Length; i++)
        {
            int typeIndex = i;
            enemyPools[i] = new ObjectPool<GameObject>(
                () => CreateEnemy(typeIndex),
                OnTakeFromPool,
                OnReturnToPool,
                OnDestroyPoolObject,
                true, maxEnemies, maxEnemies
            );
        }
    }

    public int CurrentEnemies => currentEnemies;

    public GameObject GetEnemy(int type)
    {
        currentEnemies++;
        return enemyPools[type].Get();
    }

    private GameObject CreateEnemy(int type)
    {
        GameObject enemy = Instantiate(enemyPrefabs[type]);
        if (enemy.TryGetComponent(out Poolable poolable))
            poolable.SetPool(enemyPools[type]);
        return enemy;
    }

    private void OnTakeFromPool(GameObject enemy)
    {
        enemy.SetActive(true);
        if (enemy.TryGetComponent(out Poolable poolable))
            poolable.TakeFromPool();
    }

    private void OnReturnToPool(GameObject enemy)
    {
        enemy.SetActive(false);
        currentEnemies--;
    }

    private void OnDestroyPoolObject(GameObject enemy)
    {
        Destroy(enemy);
    }
}