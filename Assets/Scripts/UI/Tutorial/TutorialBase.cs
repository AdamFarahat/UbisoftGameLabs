using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class TutorialBase : MonoBehaviour
{
    [SerializeField] private HologramText startingText;
    [SerializeField] private HologramText endingText;

    protected HologramText StartingText => startingText;
    protected HologramText EndingText => endingText;

    protected TutorialManager manager;

    protected virtual void Awake()
    {
        Assert.IsNotNull(startingText);
        Assert.IsNotNull(endingText);

        manager = GetComponentInParent<TutorialManager>();
        Assert.IsNotNull(manager);
    }

    public void DoTutorial()
    {
        IEnumerator Routine()
        {
            PreTutorial();
            yield return startingText.SpawnRoutine();
            StartTutorial();
        }

        gameObject.SetActive(true);
        StartCoroutine(Routine());
    }

    protected virtual void PreTutorial()
    {
        // do nothing
    }

    protected virtual void StartTutorial()
    {
        throw new NotImplementedException();
    }

    protected void EndTutorial()
    {
        IEnumerator Routine()
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIPress, Vector3.zero);
            yield return endingText.DespawnRoutine();
            gameObject.SetActive(false);
            manager.NextTutorial();
        }
        StartCoroutine(Routine());
    }

    public virtual void OnStartPressed()
    {
        // NOP
    }
}
