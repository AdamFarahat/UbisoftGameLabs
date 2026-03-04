using UnityEngine;
using UnityEngine.Assertions;

public class KamikazeMovementAI : MonoBehaviour
{
    [SerializeField] private float initialLaneDistance = 300f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float laneStayPeriod = 1.5f;

    private LaneBound laneBound;

    private float age = 0f;
    private float nextLaneSwitchTime = 0f;

    private void Awake()
    {
        laneBound = GetComponent<LaneBound>();
        Assert.IsNotNull(laneBound);

        GetComponent<Enemy>().OnTakeFromPool += ResetState;
    }

    private void ResetState()
    {
        laneBound.LaneDistance = initialLaneDistance;
        age = 0f;
        nextLaneSwitchTime = laneStayPeriod;
    }

    private void Start()
    {
        nextLaneSwitchTime = laneStayPeriod;
    }

    private void Update()
    {
        age += Time.deltaTime;
        laneBound.LaneDistance -= speed * Time.deltaTime;

        if (age >= nextLaneSwitchTime)
        {
            nextLaneSwitchTime += laneStayPeriod;

            if (laneBound.LaneIndex == 0)
                laneBound.MoveToLane(laneBound.LaneIndex + 1);
            else if (laneBound.LaneIndex == LaneConfigSO.Instance.GetNumberOfLanes() - 1)
                laneBound.MoveToLane(laneBound.LaneIndex - 1);
            else
                laneBound.MoveToLane(laneBound.LaneIndex + Random.Range(0, 2) * 2 - 1);
        }

        if (laneBound.LaneDistance <= )
        {
            // TODO stun explosion
        }
    }
}
