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
            while (!superEnded)
                yield return null;

            EndTutorial();
        }

        StartCoroutine(Routine());
    }
}
