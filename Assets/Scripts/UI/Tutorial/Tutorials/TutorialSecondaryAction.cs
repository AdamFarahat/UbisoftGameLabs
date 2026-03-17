using System.Collections;

public class TutorialSecondaryAction : TutorialBase
{
    private bool pressedThrow = true;
    private bool pressedBlock = true;

    protected override void StartTutorial()
    {
        GunPlayerController gunPlayer = GunPlayerController.Instance;
        if (gunPlayer != null)
        {
            gunPlayer.throwEnabled = true;
            pressedThrow = false;
            gunPlayer.PressedThrow += () => { pressedThrow = true; };

            manager.GunPlayerCooldownUI.SetActive(true);
        }

        SwordPlayerController swordPlayer = SwordPlayerController.Instance;
        if (swordPlayer != null)
        {
            swordPlayer.blockEnabled = true;
            pressedBlock = false;
            swordPlayer.PressedBlock += () => { pressedBlock = true; };

            manager.SwordPlayerCooldownUI.SetActive(true);
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
