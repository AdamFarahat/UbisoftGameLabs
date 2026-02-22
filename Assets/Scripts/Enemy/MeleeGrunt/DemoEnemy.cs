using UnityEngine;
using UnityEngine.Assertions;

public class DemoEnemy : Poolable
{
    private float initialLaneDistance;

    [SerializeField] private float speed = 1f;

    private LaneBound laneBound;

    private void Awake()
    {
        laneBound = GetComponent<LaneBound>();
        Assert.IsNotNull(laneBound);

        initialLaneDistance = laneBound.LaneDistance;
    }

    public override void OnTakeFromPool()
    {
        laneBound.LaneDistance = initialLaneDistance; // Reset state
    }

    public void ResetState()
    {
        laneBound.LaneDistance = initialLaneDistance;
    }

    private void Update()
    {
        laneBound.LaneDistance -= speed * Time.deltaTime;

        if (laneBound.LaneDistance <= 0f)
        {
            Death();
        }
    }
}
