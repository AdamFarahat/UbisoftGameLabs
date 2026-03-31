using UnityEngine;

public class TutorialIntro : TutorialBase
{
    protected override void StartTutorial()
    {
    }

    public override void OnStartPressed()
    {
        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.UIPress,Vector3.zero);
        EndTutorial();
    }
}
