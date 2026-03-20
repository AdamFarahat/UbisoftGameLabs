[System.Serializable]
public class Wave
{
    public int enemyCount;
    public int[] enemyTypes;
}

[System.Serializable]
public class WaveList
{
    public Wave[] waves;
}