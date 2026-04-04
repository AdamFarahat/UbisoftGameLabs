using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private TextAsset waveFile;

    [Header("Spawn Interval")]

    private float minSpawnTime;
    private float maxSpawnTime;

    [SerializeField] private float initialMinSpawnTime = 3f;
    [SerializeField] private float finalMinSpawnTime = 0.5f;
    [SerializeField] private float initialMaxSpawnTime = 8f;
    [SerializeField] private float finalMaxSpawnTime = 3f;
    [SerializeField] private float sigmoidSteepness = 0.06f;
    [SerializeField] private float sigmoidMidpoint = 90f;
    [SerializeField] private float waveStartDelay = 3f;
    private bool delayStarted = false;

    [Header("Wave Difficulty Selection")]
    [Tooltip("Waves are drawn from [globalDifficulty - range, globalDifficulty + range].")]
    [SerializeField] private float difficultySelectionRange = 0.15f;

    private WaveList waveList;
    private Queue<Wave> prefixQueue;
    private int waveNumber = 0;

    private Wave currentWave;
    private float nextSpawnTime;
    private int enemiesSpawned;
    private float waveStartTime;
    private bool waitingForClear;

    void Start()
    {
        waveList = JsonUtility.FromJson<WaveList>(waveFile.text);
        prefixQueue = new Queue<Wave>(waveList.prefixWaves ?? new Wave[0]);
    }

    public void StartWaves()
    {
        StartNextWave();
    }

    void Update()
    {
        if (currentWave == null) return;

        if (waitingForClear)
        {
            if (waitingForClear && spawner.CurrentEnemies <= 0 && !delayStarted)
            {
                delayStarted = true;
                StartCoroutine(DelayedNextWave());
            }
            return;
        }

        if (Time.time >= nextSpawnTime && enemiesSpawned < currentWave.spawnEntries.Length)
        {
            SpawnFromWave();
            enemiesSpawned++;
            nextSpawnTime = Time.time + GetSpawnInterval();

            if (enemiesSpawned >= currentWave.spawnEntries.Length)
                waitingForClear = true;
        }
    }

    IEnumerator DelayedNextWave()
    {
        yield return new WaitForSeconds(waveStartDelay);
        delayStarted = false;
    }

    void SpawnFromWave()
    {
        SpawnEntry entry = currentWave.spawnEntries[enemiesSpawned % currentWave.spawnEntries.Length];
        int lane = entry.lane == -1 ? Random.Range(0, LaneSet.LaneCount) : entry.lane;
        spawner.GetEnemy(entry.enemyType, lane);
    }

    void StartNextWave()
    {
        Wave next = PickNextWave();
        if (next == null)
        {
            Debug.Log("All waves done!");
            currentWave = null;
            return;
        }

        float d = DifficultyManager.Instance.Difficulty;
        minSpawnTime = Mathf.Lerp(initialMinSpawnTime, finalMinSpawnTime, d);
        maxSpawnTime = Mathf.Lerp(initialMaxSpawnTime, finalMaxSpawnTime, d);

        waveNumber++;
        DifficultyManager.Instance?.OnWaveStarted(waveNumber);

        currentWave = next;
        enemiesSpawned = 0;
        waitingForClear = false;
        waveStartTime = Time.time;
        nextSpawnTime = Time.time;

        Debug.Log($"Starting wave {waveNumber} '{currentWave.label}' | difficulty={currentWave.difficulty:F2}");
    }

    Wave PickNextWave()
    {
        if (prefixQueue.Count > 0)
            return prefixQueue.Dequeue();

        if (waveList.waves == null || waveList.waves.Length == 0)
            return null;

        float d = DifficultyManager.Instance != null ? DifficultyManager.Instance.Difficulty : 0f;
        float lo = d - difficultySelectionRange;
        float hi = d + difficultySelectionRange;

        List<Wave> candidates = new();
        foreach (var w in waveList.waves)
            if (w.difficulty >= lo && w.difficulty <= hi)
                candidates.Add(w);

        if (candidates.Count == 0)
        {
            Wave closest = waveList.waves[0];
            float bestDist = Mathf.Abs(closest.difficulty - d);
            foreach (var w in waveList.waves)
            {
                float dist = Mathf.Abs(w.difficulty - d);
                if (dist < bestDist) { bestDist = dist; closest = w; }
            }
            return closest;
        }

        return candidates[Random.Range(0, candidates.Count)];
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