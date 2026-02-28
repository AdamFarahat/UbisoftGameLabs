using UnityEngine;
using UnityEngine.Pool;

public class ShooterEnemyAI : Poolable
{
    private GameObject shootingLane;
    public GameObject playerShooter;
    public GameObject playerMelee;
    public GameObject[] lanes;
    public int shootingIndex;
    public GameObject ShootingLane
    {
        get => shootingLane;
        set { shootingLane = value; }
    }

    public override void OnTakeFromPool()
    {
        
    }

}
