using UnityEngine;
using UnityEngine.Assertions;

public class MeleeGruntMovementAI : MonoBehaviour, ISpeedRefreshable
{
    [SerializeField] private float damage = 10f;
    public float speed = 1f;
    public float Speed => speed;

    private LaneBound laneBound;

    private PlayerStats playerStats;

    private void Awake()
    {
        laneBound = GetComponent<LaneBound>();
        Assert.IsNotNull(laneBound);
        playerStats = FindFirstObjectByType<PlayerStats>();
        Assert.IsNotNull(playerStats);

        GetComponent<Enemy>().OnTakeFromPool += ResetState;
    }

    private void ResetState()
    {
        laneBound.LaneDistance = LaneSet.SpawnLine;
    }

    private void Update()
    {
        laneBound.LaneDistance -= speed * Time.deltaTime;
        // laneBound.LaneDistance -= enemy.CurrentSpeed * Time.deltaTime;

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
