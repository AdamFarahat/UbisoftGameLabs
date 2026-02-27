using UnityEngine;
using UnityEngine.Pool;

public class ShooterEnemyAI : Poolable
{
    public GameObject shootingLane;
    public GameObject playerShooter;
    public GameObject playerMelee;
    public override void OnTakeFromPool()
    {
        
    }

}
