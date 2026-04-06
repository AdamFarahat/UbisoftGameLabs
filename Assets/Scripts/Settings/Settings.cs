using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public float fontSizePourcentage = 1f;
    public float uiScalingPourcentage = 1f;
    public float enemyEmissionIntensity = 1f;
    public float uiEmissionIntensity = 1f;

    private readonly Dictionary<Material, Color> defaultEnemyEmissions = new();
    private readonly Dictionary<Material, Color> multipliedEnemyEmissions = new();
    private readonly Dictionary<Material, Color> defaultUIEmissions = new();
    private readonly Dictionary<Material, Color> multipliedUIEmissions = new();

    public static Settings Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        foreach (var m in defaultEnemyEmissions.Keys)
        {
            m.SetColor("_EmissionColor", Instance.defaultEnemyEmissions[m]);
        }
    }

    public static void LoadEnemyMaterialIfNotProcessed(Material m)
    {
        if (!Instance.defaultEnemyEmissions.ContainsKey(m))
        {
            Instance.defaultEnemyEmissions[m] = m.GetColor("_EmissionColor");
        }
        Instance.multipliedEnemyEmissions[m] = ScaleIntensity(Instance.defaultEnemyEmissions[m], Instance.enemyEmissionIntensity);
        m.SetColor("_EmissionColor", Instance.multipliedEnemyEmissions[m]);
    }

    public static void OnUpdateEnemyEmissionPercentage()
    {
        foreach (var m in Instance.multipliedEnemyEmissions.Keys.ToList())
        {
            Instance.multipliedEnemyEmissions[m] = Instance.defaultEnemyEmissions[m] * Instance.enemyEmissionIntensity;
            m.SetColor("_EmissionColor", Instance.multipliedEnemyEmissions[m]);
        }
    }

    public static void LoadUIMaterialIfNotProcessed(Material m)
    {
        if (!Instance.defaultUIEmissions.ContainsKey(m))
        {
            Instance.defaultUIEmissions[m] = m.GetColor("_EmissionColor");
        }
        Instance.multipliedUIEmissions[m] = ScaleIntensity(Instance.defaultUIEmissions[m], Instance.uiEmissionIntensity);
        m.SetColor("_EmissionColor", Instance.multipliedUIEmissions[m]);
    }

    public static void OnUpdateUIEmissionPercentage()
    {
        foreach (var m in Instance.multipliedUIEmissions.Keys.ToList())
        {
            Instance.multipliedUIEmissions[m] = Instance.defaultUIEmissions[m] * Instance.uiEmissionIntensity;
            m.SetColor("_EmissionColor", Instance.multipliedUIEmissions[m]);
        }
    }

    private static Color ScaleIntensity(Color emissionColor, float intensityScale)
    {
        float intensity = emissionColor.maxColorComponent;
        Color baseColor = emissionColor / intensity;
        float newIntensity = intensity * intensityScale;
        return baseColor * newIntensity;
    }
}
