using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public float fontSizePourcentage = 1f;
    public float uiScalingPourcentage = 1f;
    private float enemyEmissionIntensity = 1f;
    private float uiEmissionIntensity = 1f;

    private class UIEmission
    {
        public Color color;
        public string property;

        public UIEmission(Material m, string property)
        {
            color = m.GetColor(property);
            this.property = property;
        }
    }

    private readonly Dictionary<Material, UIEmission> defaultEnemyEmissions = new();
    private readonly Dictionary<Material, UIEmission> multipliedEnemyEmissions = new();
    private readonly Dictionary<Material, UIEmission> defaultUIEmissions = new();
    private readonly Dictionary<Material, UIEmission> multipliedUIEmissions = new();

    private static Settings _instance;
    public static Settings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<Settings>();

                if (_instance == null)
                {
                    GameObject go = new("Settings");
                    _instance = go.AddComponent<Settings>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
        private set => _instance = value;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        foreach (var m in defaultEnemyEmissions)
        {
            if (m.Key != null)
                m.Key.SetColor(m.Value.property, m.Value.color);
        }

        foreach (var m in defaultUIEmissions)
        {
            if (m.Key != null)
                m.Key.SetColor(m.Value.property, m.Value.color);
        }
    }

    private static void ApplyEnemyMultipliedMaterial(Material m)
    {
        UIEmission e = Instance.multipliedEnemyEmissions[m];
        e.color = ScaleIntensity(Instance.defaultEnemyEmissions[m].color, Instance.enemyEmissionIntensity);
        m.SetColor(e.property, e.color);
    }

    public static void LoadEnemyMaterialIfNotProcessed(Material m, string emissionProperty = "_EmissionColor")
    {
        if (!Instance.defaultEnemyEmissions.ContainsKey(m))
        {
            Instance.defaultEnemyEmissions[m] = new UIEmission(m, emissionProperty);
            Instance.multipliedEnemyEmissions[m] = new UIEmission(m, emissionProperty);
        }
        ApplyEnemyMultipliedMaterial(m);
    }

    public static void OnUpdateEnemyEmissionPercentage(float value)
    {
        Instance.enemyEmissionIntensity = value;
        foreach (var m in Instance.multipliedEnemyEmissions.Keys)
            ApplyEnemyMultipliedMaterial(m);
    }

    private static void ApplyUIMultipliedMaterial(Material m)
    {
        UIEmission e = Instance.multipliedUIEmissions[m];
        e.color = ScaleIntensity(Instance.defaultUIEmissions[m].color, Instance.uiEmissionIntensity);
        m.SetColor(e.property, e.color);
    }

    public static void LoadUIMaterialIfNotProcessed(Material m, string emissionProperty)
    {
        if (!Instance.defaultUIEmissions.ContainsKey(m))
        {
            Instance.defaultUIEmissions[m] = new UIEmission(m, emissionProperty);
            Instance.multipliedUIEmissions[m] = new UIEmission(m, emissionProperty);
        }
        ApplyUIMultipliedMaterial(m);
    }

    public static void OnUpdateUIEmissionPercentage(float value)
    {
        Instance.uiEmissionIntensity = value;
        foreach (var m in Instance.multipliedUIEmissions.Keys)
            ApplyUIMultipliedMaterial(m);
    }

    private static Color ScaleIntensity(Color emissionColor, float intensityScale)
    {
        float intensity = emissionColor.maxColorComponent;
        Color baseColor = emissionColor / intensity;
        float newIntensity = intensity * intensityScale;
        return baseColor * newIntensity;
    }
}
