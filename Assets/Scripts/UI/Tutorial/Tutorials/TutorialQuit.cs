using System.Collections;
using UnityEngine;

public class TutorialQuit : TutorialBase
{
    [SerializeField] private float duration = 3f;

    protected override void StartTutorial()
    {
        IEnumerator Routine()
        {
            yield return new WaitForSeconds(duration);
            EndTutorial();
        }

        StartCoroutine(Routine());
    }
}
