using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;

    [SerializeField] private int maxEnemies = 12;

    [SerializeField] private float minSpawnTime = 0.5f;
    [SerializeField] private float maxSpawnTime = 4f;

    [SerializeField] private float sigmoidSteepness = 0.06f;
    [SerializeField] private float sigmoidMidpoint = 90f;

    private float timeSinceLastSpawn;

    private IObjectPool<GameObject> enemyPool;

    private int currentEnemies = 0;

    private float Sigmoid(float t)
    {
        return 1f / (1f + Mathf.Exp(-sigmoidSteepness * (t - sigmoidMidpoint)));
    }

    private float GetSpawnInterval()
    {
        float s = Sigmoid(Time.time);
        return Mathf.Lerp(maxSpawnTime, minSpawnTime, s);
    }

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

            float spawnInterval = GetSpawnInterval();
            timeSinceLastSpawn = Time.time + spawnInterval;
        }
    }
}
