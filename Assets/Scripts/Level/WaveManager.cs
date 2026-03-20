using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private TextAsset waveFile;

    [SerializeField] private float minSpawnTime = 0.5f;
    [SerializeField] private float maxSpawnTime = 4f;
    [SerializeField] private float sigmoidSteepness = 0.06f;
    [SerializeField] private float sigmoidMidpoint = 90f;

    private WaveList waveList;
    private int waveIndex = 0;
    private Wave currentWave;

    private float nextSpawnTime;
    private int enemiesSpawned;
    private float waveStartTime;   // sigmoid resets each wave
    private bool waitingForClear; 

    void Start()
    {
        waveList = JsonUtility.FromJson<WaveList>(waveFile.text);
        StartNextWave();
    }

    void Update()
    {
        if (currentWave == null) return;

        if (waitingForClear)
        {
            if (spawner.CurrentEnemies <= 0)
                StartNextWave();
            return;
        }

        if (Time.time >= nextSpawnTime && enemiesSpawned < currentWave.enemyCount)
        {
            SpawnFromWave();
            enemiesSpawned++;
            nextSpawnTime = Time.time + GetSpawnInterval();

            if (enemiesSpawned >= currentWave.enemyCount)
                waitingForClear = true;
        }
    }

    void SpawnFromWave()
    {
        int type = currentWave.enemyTypes[Random.Range(0, currentWave.enemyTypes.Length)];
        spawner.GetEnemy(type);
    }

    void StartNextWave()
    {
        if (waveIndex >= waveList.waves.Length)
        {
            Debug.Log("All waves done!");
            currentWave = null;
            return;
        }

        currentWave = waveList.waves[waveIndex];
        waveIndex++;
        enemiesSpawned = 0;
        waitingForClear = false;
        waveStartTime = Time.time;
        nextSpawnTime = Time.time;

        Debug.Log($"Starting wave {waveIndex}");
    }

    private float Sigmoid(float t)
    {
        return 1f / (1f + Mathf.Exp(-sigmoidSteepness * (t - sigmoidMidpoint)));
    }

    private float GetSpawnInterval()
    {
        float elapsed = Time.time - waveStartTime;
        float s = Sigmoid(elapsed);
        return Mathf.Lerp(maxSpawnTime, minSpawnTime, s);
    }
}