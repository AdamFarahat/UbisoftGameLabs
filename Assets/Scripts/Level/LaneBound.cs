using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class LaneBound : MonoBehaviour
{
    [SerializeField] private float laneIndex = 0f;
    [SerializeField] private float laneDistance = 0f;
    [SerializeField] private float switchLaneDuration = 0.1f;
    [SerializeField] private float perpendicularOffset = 0f;
    public float SwitchLaneDuration => switchLaneDuration;

    public UnityAction<float> DashStart;
    public UnityAction DashEnd;

    private Coroutine switchLaneRoutine = null;
    private float switchLaneStartTime = 0f;

    public int LaneIndex
    {
        get => Mathf.RoundToInt(laneIndex);
        set { SetLaneIndex(value); }
    }

    public float LaneDistance
    {
        get => laneDistance;
        set { laneDistance = value; SyncLane(); }
    }
    
    public float PerpendicularOffset
    {
        get => perpendicularOffset;
        set { perpendicularOffset = value; SyncLane(); }
    }

    private void Start()
    {
        SyncLane();
    }

    private void OnValidate()
    {
        if (LaneSet.Instance != null)
            SyncLane();
    }

    private void OnDisable()
    {
        if (switchLaneRoutine != null)
        {
            StopCoroutine(switchLaneRoutine);
            switchLaneRoutine = null;
        }
    }

    private void SetLaneIndex(float index)
    {
        laneIndex = Mathf.Clamp(index, 0f, LaneSet.LaneCount - 1);
        SyncLane();
    }

    private void SyncLane()
    {
        Vector3 position = LaneSet.Instance.GetLanePosition(laneIndex, laneDistance);
        position += perpendicularOffset * (LaneSet.Instance.GetLaneDirection() * Vector3.right);
        position.y = transform.position.y;
        transform.position = position;
    }

    public void MoveToLane(int toIndex)
    {
        if (switchLaneRoutine != null)
            StopCoroutine(switchLaneRoutine);
        switchLaneRoutine = StartCoroutine(SwitchLanesRoutine(toIndex));
        switchLaneStartTime = Time.time;
    }

    private IEnumerator SwitchLanesRoutine(int toIndex)
    {
        DashStart?.Invoke(toIndex - laneIndex);

        float fromIndex = laneIndex;
        for (float t = 0f; t < switchLaneDuration; t += Time.deltaTime)
        {
            float a = Mathf.Clamp01(t / switchLaneDuration);
            SetLaneIndex(Mathf.Lerp(fromIndex, toIndex, a));
            yield return null;
        }
        SetLaneIndex(toIndex);
        switchLaneRoutine = null;

        DashEnd?.Invoke();
    }

    public float SwitchLaneDurationLeft()
    {
        if (switchLaneRoutine == null)
            return 0f;
        else
            return switchLaneDuration - (Time.time - switchLaneStartTime);
    }
}
