using System.Collections;
using UnityEngine;

public class GunGruntEnemyAI : MonoBehaviour
{
    [SerializeField] private float spawnLaneDistance = 300f;
    [SerializeField] private float spawnArrivalDuration = 3f;

    [SerializeField] private float minLaneDistance = 100f;
    [SerializeField] private float maxLaneDistance = 150f;
    [SerializeField] private float researchCooldown = 3f;

    [SerializeField] private float shootingCooldown = 1f;
    public float ShootingCooldown => shootingCooldown;
    [SerializeField] private float bulletSpeed = 80f;
    public float BulletSpeed => bulletSpeed;

    public int shootingIndex;
    public Transform projSpawnPoint;
    public float ResearchCooldown => researchCooldown;
    
    private Coroutine arrivalRoutine = null;
    public bool ActiveAI => arrivalRoutine == null;

    private void Awake()
    {
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

        float targetDistance = Random.Range(minLaneDistance, maxLaneDistance);

        IEnumerator ArrivalRoutine()
        {
            LaneBound laneBound = GetComponent<LaneBound>();
            laneBound.LaneDistance = spawnLaneDistance;
            for (float t = 0f; t < spawnArrivalDuration; t += Time.deltaTime)
            {
                yield return null;
                laneBound.LaneDistance = Mathf.Lerp(spawnLaneDistance, targetDistance, Mathf.Clamp01(t / spawnArrivalDuration));
            }
            laneBound.LaneDistance = targetDistance;
            arrivalRoutine = null;
        }

        arrivalRoutine = StartCoroutine(ArrivalRoutine());
    }
}
