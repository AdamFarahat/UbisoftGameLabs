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
    [SerializeField] private float heartSlideOffDistance = -300f;
    [SerializeField] private float playerSpriteFadeInDuration = 0.5f;
    [SerializeField] private float uiSlideInDuration = 1f;

    [Header("Sprite References")]
    [SerializeField] private Transform heartRoot;
    [SerializeField] private SpriteRenderer backSprite;
    [SerializeField] private SpriteRenderer rightSprite;
    [SerializeField] private SpriteRenderer leftSprite;
    [SerializeField] private SpriteRenderer fullSprite;

    [Header("UI References")]
    [SerializeField] private GameObject[] bottomBar;
    [SerializeField] private float bottomBarOffset = -200f;
    [SerializeField] private GameObject[] topBar;
    [SerializeField] private float topBarOffset = 200f;

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

        Assert.IsTrue(bottomBar.Length > 0);
        Assert.IsTrue(topBar.Length > 0);
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

        foreach (var go in bottomBar)
            go.transform.position += bottomBarOffset * Vector3.up;

        foreach (var go in topBar)
            go.transform.position += topBarOffset * Vector3.up;

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

        Vector3 startingPos = heartRoot.position;
        for (float t = 0f; t < heartSlideOffDuration; t += Time.deltaTime)
        {
            Vector3 pos = startingPos;
            pos.y += Mathf.Clamp01(t / heartSlideOffDuration) * heartSlideOffDistance;
            heartRoot.position = pos;
            yield return null;
        }
        heartRoot.position = startingPos + heartSlideOffDistance * Vector3.up;

        backSprite.enabled = false;
        fullSprite.enabled = false;

        Vector3[] bottomBarStartingPositions = new Vector3[bottomBar.Length];
        for (int i = 0; i < bottomBar.Length; i++)
            bottomBarStartingPositions[i] = bottomBar[i].transform.position;

        Vector3[] topBarStartingPositions = new Vector3[topBar.Length];
        for (int i = 0; i < topBar.Length; i++)
            topBarStartingPositions[i] = topBar[i].transform.position;

        for (float t = 0f; t < uiSlideInDuration; t += Time.deltaTime)
        {
            for (int i = 0; i < bottomBar.Length; i++)
            {
                Vector3 pos = bottomBarStartingPositions[i];
                pos.y -= Mathf.Clamp01(t / uiSlideInDuration) * bottomBarOffset;
                bottomBar[i].transform.position = pos;
            }

            for (int i = 0; i < topBar.Length; i++)
            {
                Vector3 pos = topBarStartingPositions[i];
                pos.y -= Mathf.Clamp01(t / uiSlideInDuration) * topBarOffset;
                topBar[i].transform.position = pos;
            }

            yield return null;
        }

        for (int i = 0; i < bottomBar.Length; i++)
            bottomBar[i].transform.position = bottomBarStartingPositions[i] - bottomBarOffset * Vector3.up;

        for (int i = 0; i < topBar.Length; i++)
            topBar[i].transform.position = topBarStartingPositions[i] - topBarOffset * Vector3.up;
        
        StartGame();
    }

    private void StartGame()
    {
        // TODO replace level start sfx
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UICountdown, Vector3.zero);
        FindFirstObjectByType<WaveManager>().StartWaves();
    }
}
