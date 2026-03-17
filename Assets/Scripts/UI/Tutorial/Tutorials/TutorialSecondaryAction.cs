using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class TutorialSecondaryAction : TutorialBase
{
    [SerializeField] private GameObject grenadeEntitiesRoot;
    [SerializeField] private float spawnLaneDistance = 200f;
    [SerializeField] private float spawnDuration = 2f;
    [SerializeField] private float cooldownLength = 1.5f;

    private MeleeGruntMovementAI[] meleeGrunts;

    private bool pressedThrow = true;
    private bool pressedBlock = true;

    protected override void Awake()
    {
        base.Awake();
        Assert.IsNotNull(grenadeEntitiesRoot);

        meleeGrunts = grenadeEntitiesRoot.GetComponentsInChildren<MeleeGruntMovementAI>();
        Assert.IsTrue(meleeGrunts.Length == LaneSet.LaneCount);
    }

    private void OnDisable()
    {
        foreach (MeleeGruntMovementAI meleeGrunt in meleeGrunts)
            meleeGrunt.gameObject.SetActive(false);
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
        }

        SwordPlayerController swordPlayer = SwordPlayerController.Instance;
        if (swordPlayer != null)
        {
            swordPlayer.blockEnabled = true;
            swordPlayer.blockCooldown = cooldownLength;
            pressedBlock = false;
            swordPlayer.PressedBlock += () => { pressedBlock = true; };

            manager.SwordPlayerCooldownUI.SetActive(true);
        }

        IEnumerator Routine()
        {
            // TODO spawn enemies. Count enemies killed by the gun player, and projectiles parried by the sword player (use stationary flyer enemy).

            foreach (MeleeGruntMovementAI meleeGrunt in meleeGrunts)
                SpawnMeleeGrunt(meleeGrunt);

            while (!pressedThrow || !pressedBlock || meleeGrunts.Any(g => g != null))
                yield return null;

            EndTutorial();
        }

        StartCoroutine(Routine());
    }

    private void SpawnMeleeGrunt(MeleeGruntMovementAI grunt)
    {
        grunt.gameObject.SetActive(true);
        LaneBound lane = grunt.GetComponent<LaneBound>();
        Assert.IsNotNull(lane);

        IEnumerator ArrivalRoutine()
        {
            float arrivalDistance = lane.LaneDistance;
            lane.LaneDistance = spawnLaneDistance;
            for (float t = 0f; t < spawnDuration; t += Time.deltaTime)
            {
                lane.LaneDistance = Mathf.Lerp(spawnLaneDistance, arrivalDistance, Mathf.Clamp01(t / spawnDuration));
                yield return null;
            }
            lane.LaneDistance = arrivalDistance;
        }

        StartCoroutine(ArrivalRoutine());
    }
}
