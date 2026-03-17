using System.Collections;

public class TutorialPrimaryAction : TutorialBase
{
    private bool pressedShoot = true;
    private bool pressedSlash = true;

    protected override void StartTutorial()
    {
        GunPlayerController gunPlayer = GunPlayerController.Instance;
        if (gunPlayer != null)
        {
            gunPlayer.shootEnabled = true;
            pressedShoot = false;
            gunPlayer.PressedShoot += () => { pressedShoot = true; };
        }

        SwordPlayerController swordPlayer = SwordPlayerController.Instance;
        if (swordPlayer != null)
        {
            swordPlayer.slashEnabled = true;
            pressedSlash = false;
            swordPlayer.PressedSlash += () => { pressedSlash = true; };
        }

        IEnumerator Routine()
        {
            while (!pressedShoot || !pressedSlash)
                yield return null;

            EndTutorial();
        }

        StartCoroutine(Routine());
    }
}
