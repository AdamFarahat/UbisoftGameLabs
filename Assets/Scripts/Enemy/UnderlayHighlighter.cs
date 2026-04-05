using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[ExecuteAlways]
public class UnderlayHighlighter : MonoBehaviour
{
    [SerializeField] private LaneBound lane;
    [SerializeField] private SpriteRenderer underlaySprite;
    [SerializeField] private List<Material> materials;

    private void Awake()
    {
        Assert.IsNotNull(lane);
        Assert.IsNotNull(underlaySprite);
        Assert.IsTrue(materials.Count == LaneSet.LaneCount);
    }

    private void Start()
    {
        foreach (var material in materials)
        {
            Settings.LoadMaterialIfNotProcessed(material);
        }
        underlaySprite.material = materials[lane.LaneIndex];
    }

    private void Update()
    {
        underlaySprite.material = materials[lane.LaneIndex];
    }
}
