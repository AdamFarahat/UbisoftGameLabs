[System.Serializable]
public class Wave
{
    public string label;
    public float difficulty;
    public SpawnEntry[] spawnEntries;
}

[System.Serializable]
public class WaveList
{
    public Wave[] prefixWaves;
    public Wave[] waves;
}

[System.Serializable]
public class SpawnEntry
{
    public int enemyType;
    public int lane; // -1 for random
}