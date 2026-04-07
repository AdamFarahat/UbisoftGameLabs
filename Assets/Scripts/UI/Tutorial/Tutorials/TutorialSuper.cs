using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TutorialSuper : TutorialBase
{
    [Header("General")]
    [SerializeField] private float fillSuperDuration = 0.5f;
    [SerializeField] private float deathAnimationDuration = 0.5f;
    [SerializeField] private float spawnDistance = 200f;

    [Header("Melee Grunts")]
    [SerializeField] private GameObject meleeGruntPrefab;
    [SerializeField] private float meleeGruntSpawnDelay = 1f;
    [SerializeField] private float meleeGruntSpeed = 24f;

    private readonly List<Enemy> spawnedGrunts = new();

    private bool superActivated = false;
    private bool superEnded = false;

    protected override void Awake()
    {
        base.Awake();
        Assert.IsTrue(StartingText != EndingText);
        Assert.IsNotNull(meleeGruntPrefab);

        EndingText.gameObject.SetActive(false);
    }

    protected override void StartTutorial()
    {
        if (GunPlayerController.Instance == null || SwordPlayerController.Instance == null)
        {
            EndTutorial();
            return;
        }

        PlayerStats.Instance.superEnabled = true;
        PlayerStats.Instance.SuperStarted += ShowSecondDescription;
        PlayerStats.Instance.SuperEnded += () => { superEnded = true; };

        IEnumerator Routine()
        {
            while (!superActivated)
                yield return null;

            // TODO there's barely any time to see the super ability in action - increase super ability length, particularly for tutorial

            float age = 0f;
            while (!superEnded)
            {
                age += Time.deltaTime;
                if (age >= meleeGruntSpawnDelay)
                {
                    age %= meleeGruntSpawnDelay;
                    SpawnMeleeWave();
                }

                yield return null;
            }

            foreach (Enemy grunt in spawnedGrunts)
                if (grunt != null)
                    grunt.OnParried();

            EndTutorial();
        }

        StartCoroutine(Routine());
    }

    protected override void PreTutorial()
    {
        PlayerStats.Instance.FillGunSuper(fillSuperDuration);
        PlayerStats.Instance.FillSwordSuper(fillSuperDuration);
    }

    private void SpawnMeleeWave()
    {
        for (int i = 0; i < LaneSet.LaneCount; i++)
            SpawnMeleeGrunt(i);
    }

    private void SpawnMeleeGrunt(int laneIndex)
    {
        GameObject go = Instantiate(meleeGruntPrefab);

        MeleeGruntMovementAI movement = go.GetComponent<MeleeGruntMovementAI>();
        Assert.IsNotNull(movement);
        movement.speed = meleeGruntSpeed;

        LaneBound lane = go.GetComponent<LaneBound>();
        lane.LaneIndex = laneIndex;
        lane.LaneDistance = spawnDistance;

        Enemy enemy = go.GetComponent<Enemy>();
        Assert.IsNotNull(enemy);
        enemy.deathAnimationDuration = deathAnimationDuration;
        spawnedGrunts.Add(enemy);
    }
    
    private void ShowSecondDescription()
    {
        if (superActivated)
            return;
        superActivated = true;

        IEnumerator Transition()
        {
            yield return StartingText.DespawnRoutine();
            yield return EndingText.SpawnRoutine();
        }

        StartCoroutine(Transition());
    }
}
