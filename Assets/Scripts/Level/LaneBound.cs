using System.Collections;
using UnityEngine;

public class LaneBound : MonoBehaviour
{
    [SerializeField] private float laneIndex = 0f;
    [SerializeField] private float laneDistance = 0f;
    [SerializeField] private float switchLaneDuration = 0.1f;

    private Coroutine switchLaneRoutine = null;

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

    private void OnDisable()
    {
        if (switchLaneRoutine != null)
        {
            StopCoroutine(switchLaneRoutine);
            switchLaneRoutine = null;
        }
    }

    private void SyncLane()
    {
        Vector3 position = LaneConfigSO.Instance.GetLanePosition(laneIndex, laneDistance);
        position.y = transform.position.y;
        transform.position = position;
    }

    public void MoveToLane(float toIndex)
    {
        if (switchLaneRoutine != null)
            StopCoroutine(switchLaneRoutine);
        switchLaneRoutine = StartCoroutine(SwitchLanesRoutine(toIndex));
    }

    private IEnumerator SwitchLanesRoutine(float toIndex)
    {
        float fromIndex = LaneIndex;
        for (float t = 0f; t < switchLaneDuration; t += Time.deltaTime)
        {
            float a = Mathf.Clamp01(t / switchLaneDuration);
            LaneIndex = Mathf.Lerp(fromIndex, toIndex, a);
            yield return null;
        }
        LaneIndex = toIndex;
        switchLaneRoutine = null;
    }
}
