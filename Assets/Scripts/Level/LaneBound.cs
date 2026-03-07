using System.Collections;
using UnityEngine;

public class LaneBound : MonoBehaviour
{
    [SerializeField] private float laneIndex = 0f;
    [SerializeField] private float laneDistance = 0f;
    [SerializeField] private float switchLaneDuration = 0.1f;

    private Coroutine switchLaneRoutine = null;
    private float switchLaneStartTime = 0f;

    public int LaneIndex
    {
        get => (int)laneIndex;
        set { SetLaneIndex(value); }
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

    private void SetLaneIndex(float index)
    {
        laneIndex = Mathf.Clamp(index, 0f, LaneConfigSO.Instance.GetNumberOfLanes() - 1);
        SyncLane();
    }

    private void SyncLane()
    {
        Vector3 position = LaneConfigSO.Instance.GetLanePosition(laneIndex, laneDistance);
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
        float fromIndex = laneIndex;
        for (float t = 0f; t < switchLaneDuration; t += Time.deltaTime)
        {
            float a = Mathf.Clamp01(t / switchLaneDuration);
            SetLaneIndex(Mathf.Lerp(fromIndex, toIndex, a));
            yield return null;
        }
        SetLaneIndex(toIndex);
        switchLaneRoutine = null;
    }

    public float SwitchLaneDurationLeft()
    {
        if (switchLaneRoutine == null)
            return 0f;
        else
            return switchLaneDuration - (Time.time - switchLaneStartTime);
    }
}
