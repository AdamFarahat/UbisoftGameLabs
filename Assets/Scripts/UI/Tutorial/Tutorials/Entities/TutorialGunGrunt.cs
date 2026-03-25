using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class TutorialGunGrunt : MonoBehaviour
{
    [SerializeField] private float spawnLaneDistance = 200f;
    [SerializeField] private float spawnDuration = 2f;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float projectileSpeed = 80f;
    [SerializeField] private float projectileSpawnDelay = 1f;

    private LaneBound lane;
    private bool alive = false;
    private float age = 0f;

    private void Awake()
    {
        lane = GetComponent<LaneBound>();
        Assert.IsNotNull(lane);

        Assert.IsNotNull(projectileSpawnPoint);
    }

    private void Update()
    {
        if (alive)
        {
            age += Time.deltaTime;
            if (age >= projectileSpawnDelay)
            {
                age %= projectileSpawnDelay;
                Shoot();
            }
        }
    }

    public void Spawn()
    {
        gameObject.SetActive(true);

        IEnumerator ArrivalRoutine()
        {
            lane.LaneDistance = spawnLaneDistance;
            for (float t = 0f; t < spawnDuration; t += Time.deltaTime)
            {
                lane.LaneDistance = Mathf.Lerp(spawnLaneDistance, LaneSet.VisibleEndLine, Mathf.Clamp01(t / spawnDuration));
                yield return null;
            }
            lane.LaneDistance = LaneSet.VisibleEndLine;
            alive = true;
            age = 0f;
        }

        StartCoroutine(ArrivalRoutine());
    }

    public void Despawn()
    {
        alive = false;

        IEnumerator LeaveRoutine()
        {
            lane.LaneDistance = spawnLaneDistance;
            for (float t = 0f; t < spawnDuration; t += Time.deltaTime)
            {
                lane.LaneDistance = Mathf.Lerp(LaneSet.VisibleEndLine, spawnLaneDistance, Mathf.Clamp01(t / spawnDuration));
                yield return null;
            }
            lane.LaneDistance = spawnLaneDistance;
            gameObject.SetActive(false);
        }

        StartCoroutine(LeaveRoutine());
    }

    private void Shoot()
    {
        GameObject go = ProjectilePool.SharedInstance.Spawn(projectileSpawnPoint.position, Quaternion.identity);
        EnemyProjectile proj = go.GetComponent<EnemyProjectile>();
        Assert.IsNotNull(proj);
        proj.Initialize(null, -LaneSet.Instance.transform.forward, projectileSpeed);
    }
}
