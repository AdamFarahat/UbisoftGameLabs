using System.Collections;

public class TutorialSuper : TutorialBase
{
    private bool superEnded = false;

    protected override void StartTutorial()
    {
        if (GunPlayerController.Instance == null || SwordPlayerController.Instance == null)
        {
            EndTutorial();
            return;
        }

        PlayerStats.Instance.superEnabled = true;
        PlayerStats.Instance.FillGunSuper();
        PlayerStats.Instance.FillSwordSuper();
        PlayerStats.Instance.SuperEnded += () => { superEnded = true; };

        IEnumerator Routine()
        {
            // TODO spawn a million enemies until super ends -> have them all explode at that point instead of dealing with a bunch of leftovers

            while (!superEnded)
                yield return null;

            EndTutorial();
        }

        StartCoroutine(Routine());
    }
}
