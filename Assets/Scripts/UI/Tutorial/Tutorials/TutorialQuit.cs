public class TutorialQuit : TutorialBase
{
    protected override void StartTutorial()
    {
        // NOP
    }

    public override void OnStartPressed()
    {
        EndTutorial();
    }
}
