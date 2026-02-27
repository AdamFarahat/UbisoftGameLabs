using UnityEngine;
using UnityEngine.Pool;

public class ShooterEnemyAI : Poolable
{
    public GameObject shootingLane;
    public PlayerController playerShooter;
    public PlayerController playerMelee;
    public override void OnTakeFromPool()
    {
        
    }

}
