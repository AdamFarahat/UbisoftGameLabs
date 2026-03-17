using System.Collections;
using UnityEngine;

public class TutorialJump : TutorialBase
{
    [SerializeField] private float paddingAfterJump = 2f;

    private bool pressedJump = false;

    protected override void StartTutorial()
    {
        SwordPlayerController swordPlayer = SwordPlayerController.Instance;

        if (swordPlayer == null)
        {
            EndTutorial();
            return;
        }

        swordPlayer.jumpEnabled = true;
        swordPlayer.PressedJump += () => { pressedJump = true; };

        IEnumerator Routine()
        {
            // TODO spawn projectiles in all lanes to get player to practice jumping over them. Don't finish tutorial until one wave has passed the sword player and they have jumped without taking damage.

            while (!pressedJump)
                yield return null;

            yield return new WaitForSeconds(paddingAfterJump);
            EndTutorial();
        }

        StartCoroutine(Routine());
    }
}
