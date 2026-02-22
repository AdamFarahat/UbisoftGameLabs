using UnityEngine;

public class LaneBound : MonoBehaviour
{
    [SerializeField] private float laneIndex = 0f;
    [SerializeField] private float laneDistance = 0f;

    public float LaneIndex
    {
        get => laneIndex;
        set { laneIndex = Mathf.Clamp(value, 0f, LaneConfigSO.Instance.GetNumberOfLanes() - 1); SyncLane(); }
    }

    public float LaneDistance
    {
        get => laneDistance;
        set { laneDistance = value; SyncLane(); }
    }

    private void OnValidate()
    {
        SyncLane();
    }

    private void SyncLane()
    {
        transform.position = LaneConfigSO.Instance.GetLanePosition(laneIndex, laneDistance);
    }
}
