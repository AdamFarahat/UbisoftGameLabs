using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialJump : TutorialBase
{
    [SerializeField] private float paddingAfterJump = 2f;

    private float initialY = 0f;

    protected override void StartTutorial()
    {
        SwordPlayerController swordPlayer = SwordPlayerController.Instance;

        if (swordPlayer == null)
        {
            EndTutorial();
            return;
        }

        initialY = swordPlayer.transform.position.y;
        swordPlayer.jumpEnabled = true;

        IEnumerator Routine()
        {
            // TODO spawn projectiles in all lanes to get player to practice jumping over them. Don't finish tutorial until one wave has passed the sword player and they have jumped without taking damage.

            while (swordPlayer.transform.position.y <= initialY)  // no jump
                yield return null;

            yield return new WaitForSeconds(paddingAfterJump);
            EndTutorial();
        }

        StartCoroutine(Routine());
    }
}
