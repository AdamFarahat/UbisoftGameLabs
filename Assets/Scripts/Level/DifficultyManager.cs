using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [Header("Global Difficulty Sigmoid")]
    [Tooltip("Wave number at which difficulty reaches ~50% of max.")]
    [SerializeField] private float sigmoidMidpointWave = 10f;
    [Tooltip("Controls how steeply difficulty ramps up around the midpoint.")]
    [SerializeField] private float sigmoidSteepness = 0.4f;

    public float Difficulty { get; private set; }
    public float DifficultyMultiplier => GetDifficultyMultiplier();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void OnWaveStarted(int waveNumber)
    {
        Difficulty = Sigmoid(waveNumber) * GetDifficultyMultiplier();
        Difficulty = Mathf.Clamp01(Difficulty);
        Debug.Log($"[Difficulty] Wave {waveNumber} → difficulty = {Difficulty:F3}");
    }
    private float Sigmoid(float x)
    {
        return 1f / (1f + Mathf.Exp(-sigmoidSteepness * (x - sigmoidMidpointWave)));
    }

    public enum GameDifficulty { Easy, Medium, Hard }

    [SerializeField] private GameDifficulty gameDifficulty = GameDifficulty.Medium;

    private float GetDifficultyMultiplier()
    {
        return gameDifficulty switch
        {
            GameDifficulty.Easy => 0.25f,
            GameDifficulty.Medium => 1f,
            GameDifficulty.Hard => 1.75f,
            _ => 1f
        };
    }
}
