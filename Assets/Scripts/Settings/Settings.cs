using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public float fontSizePourcentage = 1f;
    public float UIScalingPourcentage = 1f;
    public float EnemyEmissionIntensity = 1f;
    public float UIEmissionIntensity = 1f;

    private readonly Dictionary<Material, Color> defaultEmissions = new();
    private readonly Dictionary<Material, Color> multipliedEmissions = new();
    
    public static Settings Instance { get; private set; }
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }
    private void OnDestroy()
    {
        foreach (var m in defaultEmissions.Keys)
        {
            m.SetColor("_EmissionColor", Instance.defaultEmissions[m]);
        }
    }

    public static void LoadMaterialIfNotProcessed(Material m)
    {
        if (!Instance.defaultEmissions.ContainsKey(m))
        {
            Instance.defaultEmissions[m] = m.GetColor("_EmissionColor");
        }
        Instance.multipliedEmissions[m] = ScaleIntensity(Instance.defaultEmissions[m], Instance.EnemyEmissionIntensity);
        m.SetColor("_EmissionColor", Instance.multipliedEmissions[m]);
    }

    public static void OnUpdateEmissionPercentage()
    {
        foreach (var m in Instance.multipliedEmissions.Keys.ToList())
        {
            Instance.multipliedEmissions[m] = Instance.defaultEmissions[m] * Instance.EnemyEmissionIntensity;
            m.SetColor("_EmissionColor", Instance.multipliedEmissions[m]);
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
