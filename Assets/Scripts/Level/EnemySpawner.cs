using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;

    [SerializeField] private Transform [] spawnPoints;
    [SerializeField] private float timeBetweenSpawns = 5;
    private float timeSinceLastSpawn;

    private IObjectPool<GameObject> enemyPool;

    [SerializeField] private int maxEnemies = 3;
    private int currentEnemies = 0;

    private void Awake()
    {
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

        // spawn at random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        LaneBound lane = enemy.GetComponent<LaneBound>();

        if (lane != null)
        {
            lane.LaneIndex = Random.Range(0, LaneConfigSO.Instance.GetNumberOfLanes());
            lane.LaneDistance = 100f;
        }

        enemy.transform.rotation = spawnPoint.rotation;

        // reset DemoEnemy if it exists
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

        Poolable poolable = enemy.GetComponent<Poolable>();
        if (poolable != null)
            poolable.SetPool(enemyPool);

        return enemy;
    }


    void Update()
    {
        if (Time.time > timeSinceLastSpawn && currentEnemies < maxEnemies)
        {
            GameObject enemy = enemyPool.Get();
            currentEnemies++;
            timeSinceLastSpawn = Time.time + timeBetweenSpawns;
        }
    }
}
