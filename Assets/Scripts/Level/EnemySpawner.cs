using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;

    [SerializeField] private float timeBetweenSpawns = 5;
    private float timeSinceLastSpawn;

    private IObjectPool<GameObject> enemyPool;

    [SerializeField] private int maxEnemies = 3;
    private int currentEnemies = 0;

    private void Awake()
    {
        Assert.IsTrue(enemyPrefabs.Length > 0);

        enemyPool = new ObjectPool<GameObject>(
            CreateEnemy,
            OnTakeFromPool,
            OnReturnToPool,
            OnDestroyPoolObject,
            true,
            maxEnemies,
            maxEnemies
        );
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

    private GameObject CreateEnemy()
    {
        GameObject prefabToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        GameObject enemy = Instantiate(prefabToSpawn);

        if (enemy.TryGetComponent(out Poolable poolable))
            poolable.SetPool(enemyPool);

        return enemy;
    }

    void Update()
    {
        if (Time.time > timeSinceLastSpawn && currentEnemies < maxEnemies)
        {
            enemyPool.Get();
            currentEnemies++;
            timeSinceLastSpawn = Time.time + timeBetweenSpawns;
        }
    }
}
