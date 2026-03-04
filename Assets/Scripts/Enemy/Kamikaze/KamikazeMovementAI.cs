using UnityEngine;
using UnityEngine.Assertions;

public class KamikazeMovementAI : MonoBehaviour
{
    [SerializeField] private float initialLaneDistance = 300f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float laneStayPeriod = 1.5f;

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

        // TODO switch lanes randomly

        if(laneBound.LaneDistance <= 0f)
        {
            // TODO stun explosion
        }
    }
}
