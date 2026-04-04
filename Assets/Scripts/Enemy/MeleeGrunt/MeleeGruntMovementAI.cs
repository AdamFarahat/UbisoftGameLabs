using UnityEngine;
using UnityEngine.Assertions;

public class MeleeGruntMovementAI : MonoBehaviour, ISpeedRefreshable
{
    [SerializeField] private float damage = 10f;
    public float speed = 1f;
    public float Speed => speed;

    private LaneBound laneBound;

    private PlayerStats playerStats;

    [SerializeField] private float surpassingAcceleration = 50f;
    private bool playersSurpassed = false;

    private void Awake()
    {
        laneBound = GetComponent<LaneBound>();
        Assert.IsNotNull(laneBound);
        playerStats = FindFirstObjectByType<PlayerStats>();
        Assert.IsNotNull(playerStats);

        Enemy enemy = GetComponent<Enemy>();
        enemy.OnTakeFromPool += ResetState;
        enemy.SurpassedPlayers += () => { playersSurpassed = true; };
    }

    private void ResetState()
    {
        laneBound.LaneDistance = LaneSet.SpawnLine;
        playersSurpassed = false;
    }

    private void Update()
    {
        if (playersSurpassed)
            speed += surpassingAcceleration * Time.deltaTime;

        laneBound.LaneDistance -= speed * Time.deltaTime;

        if (laneBound.LaneDistance <= LaneSet.HeartLine)
        {
            playerStats.TakeDamage(damage);
        }
    }

    public void RefreshSpeed()
    {
        if (TryGetComponent(out EnemySpeedConfig cfg))
            speed = cfg.EvaluateSpeed(DifficultyManager.Instance.Difficulty);
    }
}
