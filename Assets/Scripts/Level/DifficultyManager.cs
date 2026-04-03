using UnityEngine;

/// <summary>
/// Singleton. Tracks global difficulty (0–1) that advances on a sigmoid
/// over wave count. Enemy scripts query this for their speed multiplier.
/// </summary>
public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [Header("Global Difficulty Sigmoid")]
    [Tooltip("Wave number at which difficulty reaches ~50% of max.")]
    [SerializeField] private float sigmoidMidpointWave = 10f;
    [Tooltip("Controls how steeply difficulty ramps up around the midpoint.")]
    [SerializeField] private float sigmoidSteepness = 0.4f;

    public float Difficulty { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void OnWaveStarted(int waveNumber)
    {
        Difficulty = Sigmoid(waveNumber);
        Debug.Log($"[Difficulty] Wave {waveNumber} → difficulty = {Difficulty:F3}");
    }

    private float Sigmoid(float x)
    {
        return 1f / (1f + Mathf.Exp(-sigmoidSteepness * (x - sigmoidMidpointWave)));
    }
}