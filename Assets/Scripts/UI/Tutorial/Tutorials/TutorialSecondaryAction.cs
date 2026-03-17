using System.Collections;

public class TutorialSecondaryAction : TutorialBase
{
    private bool pressedThrow = true;
    private bool pressedBlock = true;

    // TODO hide cooldown UI and show it here.
    // TODO combine all controller sprites so there's only one per tutorial - that will give the description more room.
    protected override void StartTutorial()
    {
        GunPlayerController gunPlayer = GunPlayerController.Instance;
        if (gunPlayer != null)
        {
            gunPlayer.shootEnabled = true;
            pressedThrow = false;
            gunPlayer.PressedThrow += () => { pressedThrow = true; };
        }

        SwordPlayerController swordPlayer = SwordPlayerController.Instance;
        if (swordPlayer != null)
        {
            swordPlayer.slashEnabled = true;
            pressedBlock = false;
            swordPlayer.PressedBlock += () => { pressedBlock = true; };
        }

        IEnumerator Routine()
        {
            // TODO spawn enemies. Count enemies killed by the gun player, and projectiles parried by the sword player (use stationary flyer enemy).

            while (!pressedThrow || !pressedBlock)
                yield return null;

            EndTutorial();
        }

        StartCoroutine(Routine());
    }
}
