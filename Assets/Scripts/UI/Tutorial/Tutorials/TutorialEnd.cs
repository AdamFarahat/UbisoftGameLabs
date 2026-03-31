using UnityEngine;
public class TutorialEnd : TutorialBase
{
    protected override void StartTutorial()
    {
        // do nothing
    }

    public override void OnStartPressed()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIPress, Vector3.zero);
        EndTutorial();
    }
}
