using UnityEngine;
using UnityEngine.Assertions;

public class FlyerMovement : MonoBehaviour
{
    [SerializeField] private float bobAmplitude = 1f;
    [SerializeField] private float bobSpeed = 3f;
    [SerializeField] private float forwardSpeed = 10f;
    [SerializeField] private float damage = 12f;

    private float age = 0f;
    private float baselineY = 0f;

    private LaneBound lane;

    private void Awake()
    {
        lane = GetComponent<LaneBound>();
        Assert.IsNotNull(lane);
        GetComponent<Enemy>().OnTakeFromPool += ResetState;
    }

    private void Start()
    {
        baselineY = transform.position.y;
    }

    private void ResetState()
    {
        age = 0f;
    }

    private void Update()
    {
        age += Time.deltaTime;
        Vector3 position = transform.position;
        position.y = baselineY + bobAmplitude * Mathf.Sin(bobSpeed * age);
        transform.position = position;

        lane.LaneDistance -= forwardSpeed * Time.deltaTime;
        
        if (lane.LaneDistance <= 0f)
            PlayerStats.Instance.TakeDamage(damage);
    }
}
