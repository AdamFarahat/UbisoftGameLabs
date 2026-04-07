using System.Collections;
using UnityEngine;

public class FadeAnimation
{
    public static IEnumerator FadeOutRoutine(SpriteRenderer spriteRenderer, float duration = 0.1f)
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

    public static IEnumerator FadeInRoutine(SpriteRenderer spriteRenderer, float duration = 0.1f)
    {
        Color endColor = spriteRenderer.color;

        for (float t = 0f; t < 0.5f * duration; t += Time.deltaTime)
        {
            spriteRenderer.color = Color.Lerp(Color.clear, Color.black, Mathf.Clamp01(2f * t / duration));
            yield return null;
        }

        for (float t = 0f; t < 0.5f * duration; t += Time.deltaTime)
        {
            spriteRenderer.color = Color.Lerp(Color.black, endColor, Mathf.Clamp01(2f * t / duration));
            yield return null;
        }
    }

    public static IEnumerator FlickerBlackRoutine(SpriteRenderer spriteRenderer, float duration = 0.1f)
    {
        Color startColor = spriteRenderer.color;

        for (float t = 0f; t < 0.5f * duration; t += Time.deltaTime)
        {
            spriteRenderer.color = Color.Lerp(startColor, Color.black, Mathf.Clamp01(2f * t / duration));
            yield return null;
        }

        for (float t = 0f; t < 0.5f * duration; t += Time.deltaTime)
        {
            spriteRenderer.color = Color.Lerp(Color.black, startColor, Mathf.Clamp01(2f * t / duration));
            yield return null;
        }

        spriteRenderer.color = startColor;
    }
}
