using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class TutorialPrimaryAction : TutorialBase
{
    [SerializeField] private GameObject meleeGruntPrefab;
    [SerializeField] private float spawnDistance = 200f;
    [SerializeField] private float spawnDelay = 1.5f;
    [SerializeField] private float meleeGruntSpeed = 30f;
    [SerializeField] private int spawnCount = 3;

    private bool pressedShoot = true;
    private bool pressedSlash = true;
    private int enemyDeathsLeft = 0;

    protected override void Awake()
    {
        base.Awake();

        Assert.IsNotNull(meleeGruntPrefab);
    }

    protected override void StartTutorial()
    {
        GunPlayerController gunPlayer = GunPlayerController.Instance;
        if (gunPlayer != null)
        {
            gunPlayer.shootEnabled = true;
            pressedShoot = false;
            gunPlayer.PressedShoot += () => { pressedShoot = true; };
            enemyDeathsLeft += spawnCount;
        }

        SwordPlayerController swordPlayer = SwordPlayerController.Instance;
        if (swordPlayer != null)
        {
            swordPlayer.slashEnabled = true;
            pressedSlash = false;
            swordPlayer.PressedSlash += () => { pressedSlash = true; };
            enemyDeathsLeft += spawnCount;
        }

        IEnumerator Routine()
        {
            while (!pressedShoot || !pressedSlash)
                yield return null;

            SpawnMeleeGruntWave();
            float age = 0f;
            while (spawnCount > 0)
            {
                age += Time.deltaTime;
                if (age > spawnDelay)
                {
                    age %= spawnDelay;
                    SpawnMeleeGruntWave();
                }

                yield return null;
            }

            while (enemyDeathsLeft > 0)
                yield return null;

            EndTutorial();
        }

        StartCoroutine(Routine());
    }

    private void SpawnMeleeGruntWave()
    {
        if (GunPlayerController.Instance != null)
            SpawnMeleeGrunt(Mathf.RoundToInt(GunPlayerController.LaneIndex));

        if (SwordPlayerController.Instance != null)
            SpawnMeleeGrunt(Mathf.RoundToInt(SwordPlayerController.LaneIndex));

        spawnCount--;
    }

    private void SpawnMeleeGrunt(int laneIndex)
    {
        GameObject go = Instantiate(meleeGruntPrefab);
        
        LaneBound lane = go.GetComponent<LaneBound>();
        Assert.IsNotNull(lane);
        lane.LaneIndex = laneIndex;
        lane.LaneDistance = spawnDistance;

        MeleeGruntMovementAI movement = go.GetComponent<MeleeGruntMovementAI>();
        Assert.IsNotNull(movement);
        movement.speed = meleeGruntSpeed;

        TutorialEnemyLife tutorialEnemy = go.GetComponent<TutorialEnemyLife>();
        tutorialEnemy.Die += () => { enemyDeathsLeft--; };
    }
}
