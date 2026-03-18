using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSwitchGuns : TutorialBase
{
    [SerializeField] private float paddingAfterLastGun = 2f;

    private readonly HashSet<int> gunsNotSeen = new();

    protected override void StartTutorial()
    {
        GunPlayerController gunPlayer = GunPlayerController.Instance;

        if (gunPlayer == null)
        {
            EndTutorial();
            return;
        }

        for (int i = 0; i < gunPlayer.Holster.NumberOfGuns; i++)
        {
            if (i != gunPlayer.Holster.ActiveGunIndex)
                gunsNotSeen.Add(i);
        }

        gunPlayer.toggleGunEnabled = true;

        IEnumerator Routine()
        {
            // TODO spawn enemies slowly so player can test the different guns, including shield enemy. switch descriptions after toggling for the first time to give more details on the shotgun and machine gun.

            while (gunsNotSeen.Count > 0)
            {
                gunsNotSeen.Remove(gunPlayer.Holster.ActiveGunIndex);
                yield return null;
            }

            yield return new WaitForSeconds(paddingAfterLastGun);
            EndTutorial();
        }

        StartCoroutine(Routine());
    }
}
