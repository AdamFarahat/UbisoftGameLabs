using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.VFX;

[ExecuteAlways]
public class FireUpdater : MonoBehaviour
{
    private VisualEffect fireVFX;

    private void Awake()
    {
        fireVFX = GetComponent<VisualEffect>();
        Assert.IsNotNull(fireVFX);
    }

    void Update()
    {
        fireVFX.SetFloat("UnscaledTime", Time.unscaledTime);
    }
}
