// TODO controller sprite for multiplier/score
public class TutorialScore : TutorialBase
{
    protected override void StartTutorial()
    {
        GunPlayerController gunPlayer = GunPlayerController.Instance;
        if (gunPlayer != null)
            manager.GunPlayerMultiplierUI.SetActive(true);

        SwordPlayerController swordPlayer = SwordPlayerController.Instance;
        if (swordPlayer != null)
            manager.SwordPlayerMultiplierUI.SetActive(true);

        manager.ScoreUI.SetActive(true);

        // TODO may need to reset score/multiplier here first
        // TODO wait for score/multiplier to build from both the gun player and the sword player. Could increase multiplier by full bars here.
        EndTutorial();
    }
}
