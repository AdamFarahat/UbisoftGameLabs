using System;
using System.Collections.Generic;
using UnityEngine;

public class UIEmissionRegister : MonoBehaviour
{
    [Serializable]
    public class MaterialProperty
    {
        public Material mat;
        public string property;
    }

    [SerializeField] private List<MaterialProperty> materials;

    private void Start()
    {
        foreach (var material in materials)
            Settings.LoadUIMaterialIfNotProcessed(material.mat, material.property);
    }
}
