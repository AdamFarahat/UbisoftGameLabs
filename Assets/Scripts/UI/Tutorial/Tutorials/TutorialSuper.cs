using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

// TODO spawn TutorialGunGrunts as well
public class TutorialSuper : TutorialBase
{
    [Header("General")]
    [SerializeField] private float deathAnimationDuration = 0.5f;

    [Header("Descriptions")]
    [SerializeField] private TextMeshProUGUI firstDescription;
    [SerializeField] private TextMeshProUGUI secondDescription;

    [Header("Melee Grunts")]
    [SerializeField] private GameObject meleeGruntPrefab;
    [SerializeField] private float spawnDistance = 200f;
    [SerializeField] private float spawnDelay = 1f;
    [SerializeField] private float meleeGruntSpeed = 24f;
    private readonly List<Enemy> spawnedMeleeGrunts = new();

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
            float age = 0f;
            while (!superEnded)
            {
                age += Time.deltaTime;
                if (age >= spawnDelay)
                {
                    age %= spawnDelay;
                    SpawnMeleeWave();
                }

                yield return null;
            }

            foreach (Enemy enemy in spawnedMeleeGrunts)
                if (enemy != null)
                    enemy.OnParried();

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
        spawnedMeleeGrunts.Add(enemy);
    }

    private void ShowSecondDescription()
    {
        IEnumerator Transition()
        {
            yield return FadeOutRoutine(firstDescription.gameObject);
            yield return FadeInRoutine(secondDescription.gameObject);
        }

        StartCoroutine(Transition());
    }
}
