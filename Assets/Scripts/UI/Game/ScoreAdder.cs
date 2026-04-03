using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class ScoreAdder : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float stayDuration = 0.5f;
    [SerializeField] private float fadeDuration = 0.5f;

    private int score = 0;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        Assert.IsNotNull(text);
    }

    private void Start()
    {
        text.alpha = 0f;

        if (GunPlayerController.Instance != null)
            GunPlayerController.Instance.ScoreAdded += ScoreAdded;

        if (SwordPlayerController.Instance != null)
            SwordPlayerController.Instance.ScoreAdded += ScoreAdded;
    }

    private void ScoreAdded(int delta)
    {
        score += delta;
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        text.alpha = 1f;
        text.text = $"+{score}";
        yield return new WaitForSeconds(stayDuration);

        for (float t = 0f; t < fadeDuration; t += Time.deltaTime)
        {
            text.alpha = Mathf.Clamp01(1f - t / fadeDuration);
            yield return null;
        }

        text.alpha = 0f;
        score = 0;
        fadeCoroutine = null;
    }
}
