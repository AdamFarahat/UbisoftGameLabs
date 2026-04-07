using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class CountdownManager : MonoBehaviour
{
    [Header("Fade In")]
    [SerializeField] private RawImage fadeInImage;
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float fadeInExponent = 2f;

    [Header("Countdown")]
    [SerializeField] private float fadeInDelayFactor = 0.5f;
    [SerializeField] private float countdownIndividualDuration = 1f;
    [SerializeField] private float heartSlideOffDuration = 1f;
    [SerializeField] private float heartSlideOffSpeed = 30f;
    [SerializeField] private float playerSpriteFadeInDuration = 0.5f;

    [Header("References")]
    [SerializeField] private Transform heartRoot;
    [SerializeField] private SpriteRenderer backSprite;
    [SerializeField] private SpriteRenderer rightSprite;
    [SerializeField] private SpriteRenderer leftSprite;
    [SerializeField] private SpriteRenderer fullSprite;

    private SpriteRenderer[] swordPlayerSpriteRenderers;
    private SpriteRenderer[] gunPlayerSpriteRenderers;

    private void Awake()
    {
        Assert.IsNotNull(fadeInImage);
        Assert.IsNotNull(heartRoot);
        Assert.IsNotNull(backSprite);
        Assert.IsNotNull(rightSprite);
        Assert.IsNotNull(leftSprite);
        Assert.IsNotNull(fullSprite);
    }

    private void Start()
    {
        if (SwordPlayerController.Instance != null)
            swordPlayerSpriteRenderers = SwordPlayerController.Instance.GetComponentsInChildren<SpriteRenderer>();
        else
            swordPlayerSpriteRenderers = new SpriteRenderer[0];

        if (GunPlayerController.Instance != null)
            gunPlayerSpriteRenderers = GunPlayerController.Instance.GetComponentsInChildren<SpriteRenderer>();
        else
            gunPlayerSpriteRenderers = new SpriteRenderer[0];

        foreach (var sr in swordPlayerSpriteRenderers)
            sr.enabled = false;

        foreach (var sr in gunPlayerSpriteRenderers)
            sr.enabled = false;

        leftSprite.enabled = false;
        rightSprite.enabled = false;
        fullSprite.enabled = false;

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
        yield return new WaitForSeconds(fadeInDuration * fadeInDelayFactor);;
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UICountdown, Vector3.zero);
        rightSprite.enabled = true;

        foreach (var sr in swordPlayerSpriteRenderers)
        {
            sr.enabled = true;
            FadeAnimation.FadeInRoutine(sr, playerSpriteFadeInDuration);
        }

        yield return new WaitForSeconds(countdownIndividualDuration);
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UICountdown, Vector3.zero);
        leftSprite.enabled = true;

        foreach (var sr in gunPlayerSpriteRenderers)
        {
            sr.enabled = true;
            FadeAnimation.FadeInRoutine(sr, playerSpriteFadeInDuration);
        }

        yield return new WaitForSeconds(countdownIndividualDuration);
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UICountdown, Vector3.zero);
        rightSprite.enabled = false;
        leftSprite.enabled = false;
        fullSprite.enabled = true;

        yield return new WaitForSeconds(countdownIndividualDuration);

        for (float t = 0f; t < heartSlideOffDuration; t += Time.deltaTime)
        {
            Vector3 pos = heartRoot.position;
            pos.y -= heartSlideOffSpeed * Time.deltaTime;
            heartRoot.position = pos;
            yield return null;
        }

        backSprite.enabled = false;
        fullSprite.enabled = false;

        // TODO slide HUD up

        StartGame();
    }

    private void StartGame()
    {
        // TODO level start sfx
        FindFirstObjectByType<WaveManager>().StartWaves();
    }
}
