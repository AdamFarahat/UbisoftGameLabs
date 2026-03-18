using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class TutorialBase : MonoBehaviour
{
    [Header("Tutorial Base")]
    [SerializeField] private float transitionDuration = 0.3f;
    [SerializeField] private float paddingDuration = 0.5f;

    protected TutorialManager manager;

    protected virtual void Awake()
    {
        manager = GetComponentInParent<TutorialManager>();
        Assert.IsNotNull(manager);
    }

    public void DoTutorial()
    {
        IEnumerator Routine()
        {
            yield return FadeInRoutine(gameObject);
            yield return new WaitForSeconds(paddingDuration);
            StartTutorial();
        }

        gameObject.SetActive(true);
        StartCoroutine(Routine());
    }

    protected IEnumerator FadeInRoutine(GameObject go)
    {
        yield return FadeRoutine(go, 0f, 1f);
    }

    protected virtual void StartTutorial()
    {
        throw new NotImplementedException();
    }

    protected void EndTutorial()
    {
        IEnumerator Routine()
        {
            yield return new WaitForSeconds(paddingDuration);
            yield return FadeOutRoutine(gameObject);
            gameObject.SetActive(false);
            manager.NextTutorial();
        }

        StartCoroutine(Routine());
    }

    protected IEnumerator FadeOutRoutine(GameObject go)
    {
        yield return FadeRoutine(go, 1f, 0f);
    }

    private IEnumerator FadeRoutine(GameObject go, float iy, float fy)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.localScale = new(1f, iy, 1f);

        for (float t = 0f; t < transitionDuration; t += Time.deltaTime)
        {
            float y = Mathf.Lerp(iy, fy, Mathf.Clamp01(t / transitionDuration));
            rt.localScale = new(1f, y, 1f);

            yield return null;
        }

        rt.localScale = new(1f, fy, 1f);
    }
}
