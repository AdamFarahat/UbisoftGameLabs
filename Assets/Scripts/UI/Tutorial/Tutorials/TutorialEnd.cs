using UnityEngine;
public class TutorialEnd : TutorialBase
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
