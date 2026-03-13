using UnityEngine;
using UnityEngine.Assertions;

public class KamikazeMovementAI : MonoBehaviour
{
    [SerializeField] private float initialLaneDistance = 300f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float laneStayPeriod = 1.5f;

    [Header("Stunplosion")]
    [SerializeField] private GameObject stunplosion;
    [SerializeField] private float stunTime = 1f;
    [SerializeField] private float stunRadius = 10f;
    [SerializeField] private float stunDuration = 0.3f;

    private LaneBound laneBound;

    private float age = 0f;
    private float nextLaneSwitchTime = 0f;

    private void Awake()
    {
        Assert.IsNotNull(stunplosion);
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
            else if (laneBound.LaneIndex == LaneSet.LaneCount - 1)
                laneBound.MoveToLane(laneBound.LaneIndex - 1);
            else
                laneBound.MoveToLane(laneBound.LaneIndex + Random.Range(0, 2) * 2 - 1);
        }

        if (laneBound.LaneDistance <= PlayerController.PlayerLine)
        {
            GetComponent<Enemy>().Kill();
            
            GameObject go = Instantiate(stunplosion);
            Stunplosion sp = go.GetComponent<Stunplosion>();
            Assert.IsNotNull(sp);

            sp.transform.position = transform.position;
            sp.stunTime = stunTime;
            sp.radius = stunRadius;
            sp.duration = stunDuration;
        }
    }
}
