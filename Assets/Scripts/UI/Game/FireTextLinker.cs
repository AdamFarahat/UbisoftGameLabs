using UnityEngine;
using UnityEngine.VFX; 

[ExecuteAlways]
public class FireTextLinker : MonoBehaviour
{
    public RectTransform textBoxToTrack;
    public VisualEffect fireVFX;
    
    [Header("Mapping Settings")]
    [Tooltip("Converts UI pixels into VFX scale. E.g., 0.01 turns 200 pixels into a scale of 2.0")]
    public float scaleMultiplier = 0.01f; 

    void Update()
    {
        if (textBoxToTrack != null && fireVFX != null)
        {
            float minLimit = fireVFX.GetFloat("MinFlameLength");
            float maxLimit = fireVFX.GetFloat("MaxFlameLength");
            float resetX = fireVFX.GetFloat("ResetX");

            float currentWidth = textBoxToTrack.rect.width;
            float calculatedScale = currentWidth * scaleMultiplier;
            float finalScale = Mathf.Clamp(calculatedScale, minLimit, maxLimit);

            fireVFX.SetFloat("FlameLength", finalScale);      
        }
    }
}