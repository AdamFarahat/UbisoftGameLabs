using UnityEngine;

public class TutorialIntro : TutorialBase
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
