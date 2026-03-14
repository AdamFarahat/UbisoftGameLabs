using System;
using UnityEngine;
using UnityEngine.Assertions;

public class LaneSet : MonoBehaviour
{
    private static LaneSet instance;
    public static LaneSet Instance
    {
        get
        {
            if (instance == null)
                instance = FindFirstObjectByType<LaneSet>();
            return instance;
        }
    }

    public static int LaneCount => 5;

    [SerializeField] private float laneSeparation = 12f;
    [SerializeField] private Transform playerTargetHeight;

    private void OnValidate()
    {
        instance = this;
    }

    private void Awake()
    {
        instance = this;

        Assert.IsNotNull(playerTargetHeight);
    }

    public static float PlayerLine
    {
        get
        {
            float line = 0f;
            int numPlayers = 0;
            if (GunPlayerController.Instance != null)
            {
                line += GunPlayerController.Instance.GetLaneDistance();
                numPlayers++;
            }
            if (SwordPlayerController.Instance != null)
            {
                line += SwordPlayerController.Instance.GetLaneDistance();
                numPlayers++;
            }
            return numPlayers > 0 ? line / numPlayers : line;
        }
    }

    public static float HeartLine => -50f;

    public static float VisibleEndLine => 110f;

    public static float PlayerTargetHeight => Instance.playerTargetHeight.position.y;

    private Vector3 LaneStart(int laneIndex)
    {
        int offsetIndex = laneIndex - Mathf.FloorToInt(0.5f * LaneCount);
        return GetLaneDirection() * (laneSeparation * offsetIndex * Vector3.right);
    }

    public Vector3 GetLanePosition(int laneIndex, float laneDistance)
    {
        return LaneStart(laneIndex) + laneDistance * transform.forward;
    }

    public Vector3 GetLanePosition(float laneIndex, float laneDistance)
    {
        int prevIndex = Math.Clamp(Mathf.FloorToInt(laneIndex), 0, LaneCount - 1);
        int nextIndex = Math.Clamp(Mathf.CeilToInt(laneIndex), 0, LaneCount - 1);

        return Vector3.Lerp(LaneStart(prevIndex), LaneStart(nextIndex), laneIndex - Mathf.Floor(laneIndex)) + laneDistance * transform.forward;
    }

    public Quaternion GetLaneDirection()
    {
        return Quaternion.LookRotation(transform.forward);
    }
}
