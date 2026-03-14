using System.Collections;
using UnityEngine;

public class FadeOutAnimation
{
    public static IEnumerator Routine(SpriteRenderer spriteRenderer, float duration = 0.1f)
    {
        Color startColor = spriteRenderer.color;

        for (float t = 0f; t < 0.5f * duration; t += Time.deltaTime)
        {
            spriteRenderer.color = Color.Lerp(startColor, Color.black, Mathf.Clamp01(2f * t / duration));
            yield return null;
        }

        for (float t = 0f; t < 0.5f * duration; t += Time.deltaTime)
        {
            spriteRenderer.color = Color.Lerp(Color.black, Color.clear, Mathf.Clamp01(2f * t / duration));
            yield return null;
        }
    }
}
