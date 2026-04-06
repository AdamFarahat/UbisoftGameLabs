using System.Collections.Generic;
using UnityEngine;

public class UIEmissionRegister : MonoBehaviour
{
    [SerializeField] private List<Material> materials;

    private void Start()
    {
        foreach (var material in materials)
            Settings.LoadUIMaterialIfNotProcessed(material);
    }
}
