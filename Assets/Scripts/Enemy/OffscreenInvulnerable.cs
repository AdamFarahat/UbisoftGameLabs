using UnityEngine;
using UnityEngine.Assertions;

public class OffscreenInvulnerable : MonoBehaviour
{
    public float offscreenDistance = 120f;

    private Enemy enemy;
    private LaneBound lane;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        Assert.IsNotNull(enemy);

        lane = GetComponent<LaneBound>();
        Assert.IsNotNull(lane);
    }

    private void Update()
    {
        enemy.invulnerable = lane.LaneDistance > offscreenDistance;
    }
}
