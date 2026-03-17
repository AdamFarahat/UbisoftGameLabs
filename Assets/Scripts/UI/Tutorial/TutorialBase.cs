using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class TutorialBase : MonoBehaviour
{
    [SerializeField] private float transitionDuration = 0.3f;
    [SerializeField] private float paddingDuration = 0.5f;

    protected TutorialManager manager;

    private void Awake()
    {
        manager = GetComponentInParent<TutorialManager>();
        Assert.IsNotNull(manager);
    }

    public void DoTutorial()
    {
        IEnumerator Routine()
        {
            RectTransform rt = GetComponent<RectTransform>();
            rt.localScale = new(1f, 0f, 1f);

            for (float t = 0f; t < transitionDuration; t += Time.deltaTime)
            {
                yield return null;

                float y = Mathf.Clamp01(t / transitionDuration);
                rt.localScale = new(1f, y, 1f);
            }

            rt.localScale = Vector3.one;
            yield return new WaitForSeconds(paddingDuration);
            StartTutorial();
        }

        gameObject.SetActive(true);
        StartCoroutine(Routine());
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
            RectTransform rt = GetComponent<RectTransform>();

            for (float t = 0f; t < transitionDuration; t += Time.deltaTime)
            {
                yield return null;

                float y = Mathf.Clamp01(1f - t / transitionDuration);
                rt.localScale = new(1f, y, 1f);
            }

            gameObject.SetActive(false);
            manager.NextTutorial();
        }

        StartCoroutine(Routine());
    }
}
