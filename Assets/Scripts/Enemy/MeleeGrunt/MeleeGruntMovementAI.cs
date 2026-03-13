using UnityEngine;
using UnityEngine.Assertions;

public class MeleeGruntMovementAI : MonoBehaviour
{
    [SerializeField] private float initialLaneDistance = 300f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float speed = 1f;

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
        laneBound.LaneDistance = initialLaneDistance;
    }

    private void Update()
    {
        laneBound.LaneDistance -= speed * Time.deltaTime;

        if (laneBound.LaneDistance <= LaneSet.HeartLine)
        {
            playerStats.TakeDamage(damage);
        }
    }
}
