using System.Collections;
using UnityEngine;

public class TutorialIntro : TutorialBase
{
    [SerializeField] private float duration = 10f;

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
