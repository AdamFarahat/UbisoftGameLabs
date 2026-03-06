using UnityEngine;
using UnityEngine.Assertions;

public class FlyerMovement : MonoBehaviour
{
    [SerializeField] private float bobAmplitude = 0.2f;
    [SerializeField] private float bobSpeed = 3f;
    [SerializeField] private Transform bobber;
    [SerializeField] private float forwardSpeed = 10f;
    [SerializeField] private float damage = 12f;

    private float age = 0f;

    private LaneBound lane;

    private void Awake()
    {
        Assert.IsNotNull(bobber);
        lane = GetComponent<LaneBound>();
        Assert.IsNotNull(lane);
        GetComponent<Enemy>().OnTakeFromPool += ResetState;
    }

    private void ResetState()
    {
        age = 0f;
    }

    private void Update()
    {
        age += Time.deltaTime;
        Vector3 position = bobber.localPosition;
        position.y = bobAmplitude * Mathf.Sin(bobSpeed * age);
        bobber.localPosition = position;

        lane.LaneDistance -= forwardSpeed * Time.deltaTime;
        
        if (lane.LaneDistance <= 0f)
            PlayerStats.Instance.TakeDamage(damage);
    }
}
