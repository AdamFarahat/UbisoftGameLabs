public class TutorialScore : TutorialBase
{
    protected override void StartTutorial()
    {
        GunPlayerController gunPlayer = GunPlayerController.Instance;
        if (gunPlayer != null)
        {
            gunPlayer.SetContinuousMultiplier(1f);
            gunPlayer.ResetScore();
            manager.GunPlayerMultiplierUI.SetActive(true);
        }

        SwordPlayerController swordPlayer = SwordPlayerController.Instance;
        if (swordPlayer != null)
        {
            swordPlayer.SetContinuousMultiplier(1f);
            swordPlayer.ResetScore();
            manager.SwordPlayerMultiplierUI.SetActive(true);
        }

        manager.ScoreUI.SetActive(true);

        // TODO wait for score/multiplier to build from both the gun player and the sword player. Could increase multiplier by full bars here.
        EndTutorial();
    }
}
