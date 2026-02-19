using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    // TODO: Replace individual enemy prefab references with a GameObject[] enemyPrefabs.
    [SerializeField] private GameObject shooterEnemyPrefab;
    [SerializeField] private GameObject demoEnemyPrefab;

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
            lane.LaneDistance = 20f;
        }

        enemy.transform.rotation = spawnPoint.rotation;

        // reset DemoEnemy if it exists
        Poolable poolable = enemy.GetComponent<Poolable>();
        if (poolable != null)
            poolable.OnTakeFromPool();
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
        GameObject prefabToSpawn;

        // TODO: Replace this binary random selection with a List<GameObject> of enemy prefabs.
        if (Random.value < 0.5f)
            prefabToSpawn = shooterEnemyPrefab;
        else
            prefabToSpawn = demoEnemyPrefab;

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
