using System.Collections;

public class TutorialSwitchLanes : TutorialBase
{
    private int gunStartingIndex = -1;
    private int swordStartingIndex = -1;

    private bool gunPlayerMoved = true;
    private bool swordPlayerMoved = true;

    protected override void StartTutorial()
    {
        if (GunPlayerController.Instance != null)
        {
            gunStartingIndex = GunPlayerController.LaneIndex;
            GunPlayerController.Instance.moveEnabled = true;
            gunPlayerMoved = false;
        }

        if (SwordPlayerController.Instance != null)
        {
            swordStartingIndex = SwordPlayerController.LaneIndex;
            SwordPlayerController.Instance.moveEnabled = true;
            swordPlayerMoved = false;
        }

        IEnumerator Routine()
        {
            while (true)
            {
                if (GunPlayerController.Instance != null)
                    gunPlayerMoved |= gunStartingIndex != GunPlayerController.LaneIndex;

                if (SwordPlayerController.Instance != null)
                    swordPlayerMoved |= swordStartingIndex != SwordPlayerController.LaneIndex;

                if (gunPlayerMoved && swordPlayerMoved)
                    break;

                yield return null;
            }

            EndTutorial();
        }

        StartCoroutine(Routine());
    }
}
