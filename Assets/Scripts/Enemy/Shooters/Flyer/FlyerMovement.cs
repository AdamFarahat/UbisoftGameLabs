using UnityEngine;
using UnityEngine.Assertions;

public class FlyerMovement : MonoBehaviour, ISpeedRefreshable
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float damage = 12f;

    private LaneBound lane;

    [SerializeField] private float surpassingAcceleration = 50f;
    private bool playersSurpassed = false;

    private void Awake()
    {
        lane = GetComponent<LaneBound>();
        Assert.IsNotNull(lane);

        Enemy enemy = GetComponent<Enemy>();
        enemy.OnTakeFromPool += ResetState;
        enemy.SurpassedPlayers += () => { playersSurpassed = true; };
    }

    private void ResetState()
    {
        lane.LaneDistance = LaneSet.SpawnLine;
        playersSurpassed = false;
    }

    private void Update()
    {
        if (playersSurpassed)
            speed += surpassingAcceleration * Time.deltaTime;

        lane.LaneDistance -= speed * Time.deltaTime;
        
        if (lane.LaneDistance <= LaneSet.HeartLine)
            PlayerStats.Instance.TakeDamage(damage);
    }
    
    public void RefreshSpeed()
    {
        float d = DifficultyManager.Instance.Difficulty;
        if (TryGetComponent(out EnemySpeedConfig cfg))
            speed = cfg.EvaluateSpeed(d);
        if (TryGetComponent(out FlyerShooting flyerShooting))
            flyerShooting.RefreshShotCooldown(d);
    }
}
