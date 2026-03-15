using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Transform tipsParent;
    [SerializeField] private float tipDuration = 5f;
    [SerializeField] private float tipAnimateDuration = 0.3f;

    private GameObject[] tips;
    private Coroutine[] tipAnimations;
    private int tipIndex = 0;
    private float tipAge = 0f;

    private void Awake()
    {
        Assert.IsNotNull(tipsParent);

        List<GameObject> tipsList = new();
        foreach (Transform child in tipsParent)
        {
            child.gameObject.SetActive(false);
            tipsList.Add(child.gameObject);
        }
        tips = tipsList.ToArray();
        Assert.IsTrue(tips.Length > 0);

        tipAnimations = new Coroutine[tips.Length];
    }

    private void Start()
    {
        if (GunPlayerController.Instance != null)
            GunPlayerController.Instance.StartButtonPressed += OnStartButtonPressed;

        if (SwordPlayerController.Instance != null)
            SwordPlayerController.Instance.StartButtonPressed += OnStartButtonPressed;

        FadeIn();
    }

    private void Update()
    {
        tipAge += Time.deltaTime;
        while (tipAge >= tipDuration)
        {
            tipAge -= tipDuration;
            ShowNextTip();
        }
    }

    private void OnStartButtonPressed()
    {
        SceneManager.LoadScene("Menu");
    }

    private void ShowNextTip()
    {
        FadeOut();
        tipIndex = (tipIndex + 1) % tips.Length;
        FadeIn();
    }

    private void FadeOut()
    {
        if (tipAnimations[tipIndex] != null)
            StopCoroutine(tipAnimations[tipIndex]);

        IEnumerator Routine(int index)
        {
            RectTransform rt = tips[index].GetComponent<RectTransform>();

            for (float t = 0f; t < tipAnimateDuration; t += Time.deltaTime)
            {
                yield return null;

                float y = Mathf.Clamp01(1f - t / tipAnimateDuration);
                rt.localScale = new(1f, y, 1f);
            }

            tips[index].SetActive(false);
            tipAnimations[index] = null;
        }

        tipAnimations[tipIndex] = StartCoroutine(Routine(tipIndex));
    }

    private void FadeIn()
    {
        if (tipAnimations[tipIndex] != null)
            StopCoroutine(tipAnimations[tipIndex]);

        IEnumerator Routine(int index)
        {
            tips[index].SetActive(true);
            RectTransform rt = tips[index].GetComponent<RectTransform>();
            rt.localScale = new(1f, 0f, 1f);

            for (float t = 0f; t < tipAnimateDuration; t += Time.deltaTime)
            {
                yield return null;

                float y = Mathf.Clamp01(t / tipAnimateDuration);
                rt.localScale = new(1f, y, 1f);
            }

            rt.localScale = Vector3.one;
            tipAnimations[index] = null;
        }

        tipAnimations[tipIndex] = StartCoroutine(Routine(tipIndex));
    }
}
