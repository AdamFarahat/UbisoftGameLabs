using UnityEngine;
using UnityEngine.Assertions;

public class MeleeMovementAI : MonoBehaviour
{
    private float initialLaneDistance;

    [SerializeField] private float speed = 1f;

    private LaneBound laneBound;

    private void Awake()
    {
        laneBound = GetComponent<LaneBound>();
        Assert.IsNotNull(laneBound);

        initialLaneDistance = laneBound.LaneDistance;

        if (TryGetComponent(out Enemy enemy))
            enemy.OnTakeFromPool += OnTakeFromPool;
    }

    private void OnTakeFromPool()
    {
        ResetState();
    }

    public void ResetState()
    {
        laneBound.LaneDistance = initialLaneDistance;
    }

    private void Update()
    {
        laneBound.LaneDistance -= speed * Time.deltaTime;
    }
}
