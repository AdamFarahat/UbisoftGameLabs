using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class TutorialBase : MonoBehaviour
{
    [SerializeField] private float transitionDuration = 0.3f;

    private TutorialManager manager;

    private void Awake()
    {
        manager = GetComponentInParent<TutorialManager>();
        Assert.IsNotNull(manager);
    }

    public void DoTutorial()
    {
        IEnumerator Routine()
        {
            gameObject.SetActive(true);
            RectTransform rt = GetComponent<RectTransform>();
            rt.localScale = new(1f, 0f, 1f);

            for (float t = 0f; t < transitionDuration; t += Time.deltaTime)
            {
                yield return null;

                float y = Mathf.Clamp01(t / transitionDuration);
                rt.localScale = new(1f, y, 1f);
            }

            rt.localScale = Vector3.one;
            StartTutorial();
        }

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
