using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class CountdownManager : MonoBehaviour
{
    // TODO fade out of tutorial
    [Header("Fade In")]
    [SerializeField] private RawImage fadeInImage;
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float fadeInExponent = 2f;

    [Header("Countdown")]
    [SerializeField] private float fadeInDelayFactor = 0.5f;
    [SerializeField] private Image countdownImage;
    [SerializeField] private Sprite countdown3;
    [SerializeField] private Sprite countdown2;
    [SerializeField] private Sprite countdown1;
    [SerializeField] private float countdownIndividualDuration = 1f;

    private void Awake()
    {
        Assert.IsNotNull(fadeInImage);
        Assert.IsNotNull(countdownImage);
        Assert.IsNotNull(countdown3);
        Assert.IsNotNull(countdown2);
        Assert.IsNotNull(countdown1);
    }

    private void Start()
    {
        countdownImage.gameObject.SetActive(false);
        fadeInImage.gameObject.SetActive(true);
        StartCoroutine(FadeIn());
        StartCoroutine(Countdown());
    }

    private IEnumerator FadeIn()
    {
        for (float t = 0f; t < fadeInDuration; t += Time.deltaTime)
        {
            float a = Mathf.Clamp01(1f - Mathf.Pow(t / fadeInDuration, fadeInExponent));
            fadeInImage.color = new Color(0f, 0f, 0f, a);

            yield return null;
        }

        fadeInImage.gameObject.SetActive(false);
    }

    private IEnumerator Countdown()
    {
        yield return new WaitForSeconds(fadeInDuration * fadeInDelayFactor);

        countdownImage.gameObject.SetActive(true);
        countdownImage.sprite = countdown3;  // TODO countdown sfx
        yield return new WaitForSeconds(countdownIndividualDuration);
        countdownImage.sprite = countdown2;  // TODO countdown sfx
        yield return new WaitForSeconds(countdownIndividualDuration);
        countdownImage.sprite = countdown1;  // TODO countdown sfx
        yield return new WaitForSeconds(countdownIndividualDuration);
        countdownImage.gameObject.SetActive(false);

        StartGame();
    }

    private void StartGame()
    {
        // TODO level start sfx
        // TODO start wave spawner
    }
}
