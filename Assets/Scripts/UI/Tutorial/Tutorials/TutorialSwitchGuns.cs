using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSwitchGuns : TutorialBase
{
    [SerializeField] private float paddingAfterLastGun = 2f;

    private readonly HashSet<int> gunsNotSeen = new();

    protected override void StartTutorial()
    {
        GunPlayerController gunner = GunPlayerController.Instance;

        if (gunner == null)
        {
            EndTutorial();
            return;
        }

        for (int i = 0; i < gunner.Holster.NumberOfGuns; i++)
        {
            if (i != gunner.Holster.ActiveGunIndex)
                gunsNotSeen.Add(i);
        }

        gunner.toggleGunEnabled = true;

        IEnumerator Routine()
        {
            while (gunsNotSeen.Count > 0)
            {
                gunsNotSeen.Remove(gunner.Holster.ActiveGunIndex);
                yield return null;
            }

            yield return new WaitForSeconds(paddingAfterLastGun);
            EndTutorial();
        }

        StartCoroutine(Routine());
    }
}
