using UnityEngine;

public class TutorialIntro : TutorialBase
{
    protected override void StartTutorial()
    {
        manager.PowerBarUI.SetActive(true);
        manager.HeartUI.SetActive(true);
    }

    public override void OnStartPressed()
    {
        
        EndTutorial();
    }
}
