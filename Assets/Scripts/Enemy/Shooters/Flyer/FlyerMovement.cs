using UnityEngine;
using UnityEngine.Assertions;

public class FlyerMovement : MonoBehaviour
{
    [SerializeField] private float forwardSpeed = 10f;
    [SerializeField] private float damage = 12f;

    private LaneBound lane;

    private void Awake()
    {
        lane = GetComponent<LaneBound>();
        Assert.IsNotNull(lane);
        GetComponent<Enemy>().OnTakeFromPool += ResetState;
    }

    private void ResetState()
    {
        lane.LaneDistance = LaneSet.SpawnLine;
    }

    private void Update()
    {
        lane.LaneDistance -= forwardSpeed * Time.deltaTime;
        
        if (lane.LaneDistance <= LaneSet.HeartLine)
            PlayerStats.Instance.TakeDamage(damage);
    }
}
