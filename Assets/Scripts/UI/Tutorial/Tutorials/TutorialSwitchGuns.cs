using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class TutorialSwitchGuns : TutorialBase
{
    [Header("General")]
    [SerializeField] private GameObject meleeGruntsRoot;

    [Header("Descriptions")]
    [SerializeField] private float secondDescriptionWait = 1f;

    private TutorialEnemyLife[] meleeGrunts;

    private readonly HashSet<int> gunsNotSeen = new();
    private bool transitionsSwitched = false;

    protected override void Awake()
    {
        base.Awake();
        
        Assert.IsNotNull(meleeGruntsRoot);
        meleeGrunts = meleeGruntsRoot.GetComponentsInChildren<TutorialEnemyLife>();

        Assert.IsTrue(StartingText != EndingText);
        EndingText.enabled = false;

        foreach (TutorialEnemyLife meleeGrunt in meleeGrunts)
            meleeGrunt.gameObject.SetActive(false);
    }

    protected override void StartTutorial()
    {
        GunPlayerController gunPlayer = GunPlayerController.Instance;

        if (gunPlayer == null)
        {
            EndTutorial();
            return;
        }

        gunPlayer.PressedShoot += PressedShoot;

        for (int i = 0; i < gunPlayer.Holster.NumberOfGuns; i++)
        {
            if (i != gunPlayer.Holster.ActiveGunIndex)  // player already used revolver, no need to check for it here
                gunsNotSeen.Add(i);
        }

        gunPlayer.toggleGunEnabled = true;
        gunPlayer.PressedToggle += ShowSecondDescription;

        float age = Time.time;
        IEnumerator Routine()
        {
            while (gunsNotSeen.Count > 0)
                yield return null;

            foreach (TutorialEnemyLife meleeGrunt in meleeGrunts)
                meleeGrunt.gameObject.SetActive(true);

            yield return new WaitUntil(() => meleeGrunts.All(g => g == null || g.Dead));

            EndTutorial();
        }

        StartCoroutine(Routine());
    }

    private void ShowSecondDescription()
    {
        if (transitionsSwitched)
            return;
        transitionsSwitched = true;

        IEnumerator Transition()
        {
            yield return new WaitForSeconds(secondDescriptionWait);
            yield return StartingText.DespawnRoutine();
            yield return EndingText.SpawnRoutine();
        }

        StartCoroutine(Transition());
    }

    private void PressedShoot()
    {
        gunsNotSeen.Remove(GunPlayerController.Instance.Holster.ActiveGunIndex);
    }
}
