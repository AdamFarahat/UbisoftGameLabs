using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class GunGruntEnemyAI : MonoBehaviour
{
    [SerializeField] private float spawnArrivalDuration = 3f;

    [SerializeField] private float minLaneDistance = 100f;
    [SerializeField] private float researchCooldown = 3f;

    [SerializeField] private float shootingCooldown = 1f;
    public float ShootingCooldown => shootingCooldown;
    [SerializeField] private float bulletSpeed = 80f;
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
}
