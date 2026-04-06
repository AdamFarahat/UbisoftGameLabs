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
            if (waitingForClear && spawner.CurrentEnemies <= 1 && !delayStarted)
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
        StartNextWave();
    }

    void SpawnFromWave()
    {
        SpawnEntry entry = currentWave.spawnEntries[enemiesSpawned % currentWave.spawnEntries.Length];
        int lane = entry.lane == -1 ? Random.Range(0, LaneSet.LaneCount) : entry.lane;
        int type = entry.enemyType;
        if (type == -1)
        {
            int randomInt = Random.Range(0, 101); 
            if (randomInt <= 25)
                type = 0;
            else if (randomInt <=50) 
                type = 1;
            else if (randomInt <=65) 
                type = 2;
            else if (randomInt <=80) 
                type = 3;
            else if (randomInt <=90) 
                type = 4; 
            else 
                type = 5;
        }
        spawner.GetEnemy(type, lane);
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
        float m = DifficultyManager.Instance.DifficultyMultiplier;
        minSpawnTime = Mathf.Lerp(initialMinSpawnTime, finalMinSpawnTime, d) / m;
        maxSpawnTime = Mathf.Lerp(initialMaxSpawnTime, finalMaxSpawnTime, d) / m;

        waveNumber++;
        DifficultyManager.Instance?.OnWaveStarted(waveNumber);

        currentWave = next;
        enemiesSpawned = 0;
        waitingForClear = false;
        waveStartTime = Time.time;
        nextSpawnTime = Time.time + GetSpawnInterval();

        Debug.Log($"Starting wave {waveNumber} '{currentWave.label}' | difficulty={currentWave.difficulty:F2}");
    }

    Wave PickNextWave()
    {
        if (prefixQueue.Count > 0)
            return prefixQueue.Dequeue();

        if (waveList.waves == null || waveList.waves.Length == 0)
            return null;

        if (PlayerStats.Instance.IsSuperActive())
        {
            List<Wave> bossWaves = new();
            foreach (var w in waveList.waves)
                if (w.isBossWave)
                    bossWaves.Add(w);

            if (bossWaves.Count > 0)
                return bossWaves[Random.Range(0, bossWaves.Count)];
        }

        float d = DifficultyManager.Instance != null ? DifficultyManager.Instance.Difficulty : 0f;
        float lo = d - difficultySelectionRange;
        float hi = d + difficultySelectionRange;

        List<Wave> candidates = new();
        foreach (var w in waveList.waves)
            if (w.difficulty >= lo && w.difficulty <= hi && !w.isBossWave)
                candidates.Add(w);

        if (candidates.Count == 0)
        {
            Wave closest = null;
            float bestDist = float.MaxValue;
            foreach (var w in waveList.waves)
            {
                if (w.isBossWave) continue;
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