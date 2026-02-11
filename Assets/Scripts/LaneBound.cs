using UnityEngine;

public class LaneBound : MonoBehaviour
{
    [SerializeField] private int laneIndex = 0;
    [SerializeField] private float laneDistance = 0f;

    public int LaneIndex
    {
        get => laneIndex;
        set { laneIndex = System.Math.Clamp(value, 0, LaneConfigSO.Instance.GetNumberOfLanes() - 1); SyncLane(); }
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
