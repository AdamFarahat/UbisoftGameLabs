using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "LaneConfigSO", menuName = "Scriptable Objects/LaneConfigSO")]
public class LaneConfigSO : ScriptableObject
{
    private static LaneConfigSO _instance;
    public static LaneConfigSO Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<LaneConfigSO>("LaneConfigSO");
                Assert.IsNotNull(_instance, "LaneConfigSO not found in Resources!");
            }

            Assert.IsTrue(_instance.lanePositions.Count > 0);
            _instance.laneDirection.Normalize();

            return _instance;
        }
    }

    [SerializeField] private List<Vector3> lanePositions;
    [SerializeField] private Vector3 laneDirection;

    public int GetNumberOfLanes()
    {
        return lanePositions.Count;
    }

    public Vector3 GetLanePosition(int laneIndex, float laneDistance)
    {
        return lanePositions[laneIndex] + laneDistance * laneDirection;
    }

    public Quaternion GetLaneDirection()
    {
        return Quaternion.LookRotation(laneDirection);
    }
}
