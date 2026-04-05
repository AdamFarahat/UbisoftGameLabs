using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class GunGruntEnemyAI : MonoBehaviour, ISpeedRefreshable
{
    [SerializeField] private float spawnArrivalDuration = 3f;

    [SerializeField] private float minLaneDistance = 100f;
    [SerializeField] private float researchCooldown = 3f;
    private float shootingCooldown;

    public float ShootingCooldown => shootingCooldown;
    private float bulletSpeed;
    [SerializeField] private float maxShootingCooldown = 2f;
    [SerializeField] private float minShootingCooldown = 0.3f;
    [SerializeField] private float minBulletSpeed = 50f;
    [SerializeField] private float maxBulletSpeed = 120f;
    public float BulletSpeed => bulletSpeed;

    [SerializeField] private float laneIndexShootingTreshold = 0.1f;
    public float LaneIndexShootingTreshold => laneIndexShootingTreshold;

    public int shootingIndex;
    public Transform projSpawnPoint;
    public float ResearchCooldown => researchCooldown;
    
    private Coroutine arrivalRoutine = null;
    public bool ActiveAI => arrivalRoutine == null;

    private void Awake()
    {
        Assert.IsTrue(minLaneDistance < LaneSet.VisibleEndLine);

        GetComponent<Enemy>().OnTakeFromPool += ResetState;
    }

    private void Start()
    {
        ResetState();
    }

    private void ResetState()
    {
        if (arrivalRoutine != null)
            StopCoroutine(arrivalRoutine);

        float targetDistance = Random.Range(minLaneDistance, LaneSet.VisibleEndLine);

        IEnumerator ArrivalRoutine()
        {
            LaneBound laneBound = GetComponent<LaneBound>();
            laneBound.LaneDistance = LaneSet.SpawnLine;
            for (float t = 0f; t < spawnArrivalDuration; t += Time.deltaTime)
            {
                yield return null;
                laneBound.LaneDistance = Mathf.Lerp(LaneSet.SpawnLine, targetDistance, Mathf.Clamp01(t / spawnArrivalDuration));
            }
            laneBound.LaneDistance = targetDistance;
            arrivalRoutine = null;
        }

        arrivalRoutine = StartCoroutine(ArrivalRoutine());
    }

public void RefreshSpeed()
    {
        float d = Mathf.Clamp01(DifficultyManager.Instance.Difficulty);
        shootingCooldown = Mathf.Lerp(maxShootingCooldown, minShootingCooldown, d);
        bulletSpeed = Mathf.Lerp(minBulletSpeed, maxBulletSpeed, d);
    }
}
