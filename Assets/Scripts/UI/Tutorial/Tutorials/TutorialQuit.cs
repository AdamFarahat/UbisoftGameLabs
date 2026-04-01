using UnityEngine;

public class TutorialQuit : TutorialBase
{
    protected override void StartTutorial()
    {
        // NOP
    }

    public override void OnStartPressed()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIPress, Vector3.zero);
        EndTutorial();
    }
}
