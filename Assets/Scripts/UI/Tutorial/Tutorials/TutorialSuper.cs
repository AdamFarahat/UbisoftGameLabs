using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class TutorialSuper : TutorialBase
{
    [Header("General")]
    [SerializeField] private float deathAnimationDuration = 0.5f;
    [SerializeField] private float spawnDistance = 200f;

    [Header("Descriptions")]
    [SerializeField] private TextMeshProUGUI firstDescription;
    [SerializeField] private TextMeshProUGUI secondDescription;

    [Header("Melee Grunts")]
    [SerializeField] private GameObject meleeGruntPrefab;
    [SerializeField] private float meleeGruntSpawnDelay = 1f;
    [SerializeField] private float meleeGruntSpeed = 24f;

    [Header("Gun Grunts")]
    [SerializeField] private GameObject gunGruntPrefab;
    private readonly TutorialGunGrunt[] laneToGunGrunts = new TutorialGunGrunt[LaneSet.LaneCount];

    private readonly List<Enemy> spawnedGrunts = new();

    private bool superPressed = false;
    private bool superEnded = false;

    protected override void Awake()
    {
        base.Awake();
        Assert.IsNotNull(firstDescription);
        Assert.IsNotNull(secondDescription);
        Assert.IsNotNull(meleeGruntPrefab);

        secondDescription.GetComponent<RectTransform>().localScale = new(1f, 0f, 1f);
    }

    protected override void StartTutorial()
    {
        if (GunPlayerController.Instance == null || SwordPlayerController.Instance == null)
        {
            EndTutorial();
            return;
        }

        PlayerStats.Instance.superEnabled = true;
        PlayerStats.Instance.FillGunSuper();
        PlayerStats.Instance.FillSwordSuper();
        PlayerStats.Instance.SuperStarted += ShowSecondDescription;
        PlayerStats.Instance.SuperEnded += () => { superEnded = true; };

        IEnumerator Routine()
        {
            while (!superPressed)
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

                SpawnMissingGunGrunts();

                yield return null;
            }

            foreach (Enemy grunt in spawnedGrunts)
                if (grunt != null)
                    grunt.OnParried();

            EndTutorial();
        }

        StartCoroutine(Routine());
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

    private void SpawnMissingGunGrunts()
    {
        for (int i = 0; i < LaneSet.LaneCount; i++)
            if (laneToGunGrunts[i] == null)
                SpawnGunGrunt(i);
    }

    private void SpawnGunGrunt(int laneIndex)
    {
        GameObject go = Instantiate(gunGruntPrefab);

        LaneBound lane = go.GetComponent<LaneBound>();
        lane.LaneIndex = laneIndex;
        lane.LaneDistance = spawnDistance;

        Enemy enemy = go.GetComponent<Enemy>();
        Assert.IsNotNull(enemy);
        enemy.deathAnimationDuration = deathAnimationDuration;
        spawnedGrunts.Add(enemy);

        TutorialGunGrunt gunGrunt = go.GetComponent<TutorialGunGrunt>();
        Assert.IsNotNull(gunGrunt);
        gunGrunt.Spawn();
        laneToGunGrunts[laneIndex] = gunGrunt;
    }

    private void ShowSecondDescription()
    {
        if (superPressed)
            return;
        superPressed = true;

        IEnumerator Transition()
        {
            yield return FadeOutRoutine(firstDescription.gameObject);
            yield return FadeInRoutine(secondDescription.gameObject);
        }

        StartCoroutine(Transition());
    }
}
