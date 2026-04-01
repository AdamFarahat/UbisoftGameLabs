[System.Serializable]
public class Wave
{
    public string label;
    public float difficulty;
    public int enemyCount;
    public int[] enemyTypes;
}

[System.Serializable]
public class WaveList
{
    public Wave[] prefixWaves;
    public Wave[] waves;
}