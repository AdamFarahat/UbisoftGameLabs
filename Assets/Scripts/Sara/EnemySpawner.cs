using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
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
            3,
            3
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
        DemoEnemy demo = enemy.GetComponent<DemoEnemy>();
        if (demo != null)
            demo.ResetState();
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

        if (Random.value < 0.5f)
            prefabToSpawn = shooterEnemyPrefab;
        else
            prefabToSpawn = demoEnemyPrefab;

        GameObject enemy = Instantiate(prefabToSpawn);

        IPoolable poolable = enemy.GetComponent<IPoolable>();
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
