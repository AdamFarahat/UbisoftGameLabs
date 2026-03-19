using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class TutorialSecondaryAction : TutorialBase
{
    [SerializeField] private GameObject grenadeEntitiesRoot;
    [SerializeField] private GameObject parryEntitiesRoot;
    [SerializeField] private float spawnLaneDistance = 200f;
    [SerializeField] private float spawnDuration = 2f;
    [SerializeField] private float cooldownLength = 1.5f;

    private LaneBound[] meleeGrunts;
    private LaneBound[] flyerGrunts;

    private bool pressedThrow = true;
    private bool pressedBlock = true;

    protected override void Awake()
    {
        base.Awake();
        Assert.IsNotNull(grenadeEntitiesRoot);
        Assert.IsNotNull(parryEntitiesRoot);

        meleeGrunts = grenadeEntitiesRoot.GetComponentsInChildren<LaneBound>();
        flyerGrunts = parryEntitiesRoot.GetComponentsInChildren<LaneBound>();
    }

    private void OnDisable()
    {
        foreach (LaneBound meleeGrunt in meleeGrunts)
            if (meleeGrunt != null)
                meleeGrunt.gameObject.SetActive(false);

        foreach (LaneBound flyerGrunt in flyerGrunts)
            if (flyerGrunt != null)
                flyerGrunt.gameObject.SetActive(false);
    }

    protected override void StartTutorial()
    {
        GunPlayerController gunPlayer = GunPlayerController.Instance;
        if (gunPlayer != null)
        {
            gunPlayer.throwEnabled = true;
            gunPlayer.GrenadeBelt.throwCooldown = cooldownLength;
            pressedThrow = false;
            gunPlayer.PressedThrow += () => { pressedThrow = true; };

            manager.GunPlayerCooldownUI.SetActive(true);

            foreach (LaneBound meleeGrunt in meleeGrunts)
                SpawnGrunt(meleeGrunt);
        }

        SwordPlayerController swordPlayer = SwordPlayerController.Instance;
        if (swordPlayer != null)
        {
            swordPlayer.blockEnabled = true;
            swordPlayer.blockCooldown = cooldownLength;
            pressedBlock = false;
            swordPlayer.PressedBlock += () => { pressedBlock = true; };

            manager.SwordPlayerCooldownUI.SetActive(true);

            foreach (LaneBound flyerGrunt in flyerGrunts)
                SpawnGrunt(flyerGrunt);
        }

        IEnumerator Routine()
        {
            while (!pressedThrow || !pressedBlock
                    || meleeGrunts.Any(g => g != null && g.isActiveAndEnabled)
                    || flyerGrunts.Any(g => g != null && g.isActiveAndEnabled))
                yield return null;

            EndTutorial();
        }

        StartCoroutine(Routine());
    }

    private void SpawnGrunt(LaneBound grunt)
    {
        grunt.gameObject.SetActive(true);

        IEnumerator ArrivalRoutine()
        {
            float arrivalDistance = grunt.LaneDistance;
            grunt.LaneDistance = spawnLaneDistance;
            for (float t = 0f; t < spawnDuration; t += Time.deltaTime)
            {
                grunt.LaneDistance = Mathf.Lerp(spawnLaneDistance, arrivalDistance, Mathf.Clamp01(t / spawnDuration));
                yield return null;
            }
            grunt.LaneDistance = arrivalDistance;
        }

        StartCoroutine(ArrivalRoutine());
    }
}
