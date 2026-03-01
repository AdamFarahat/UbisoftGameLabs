using UnityEngine;

public class ShootingBehavior : StateMachineBehaviour
{
    public string lostPlayerTrigger = "PlayerDisappeared";
    public float shootingRate = 1.0f;
    public GameObject projObj;
    private PlayerController shootingTarget;

    private int shootingIndex;
    private Transform projSpawnPoint;

    private float time;
    private bool firstShoot;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        shootingIndex = animator.GetComponent<ShooterEnemyAI>().shootingIndex;
        projSpawnPoint = animator.GetComponent<ShooterEnemyAI>().projSpawnPoint;
        shootingTarget = FindShootingTarget();
        time = 0f;
        firstShoot = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        time += Time.deltaTime;
        if (time >= shootingRate || firstShoot)
        {
            shootingTarget = FindShootingTarget();
            firstShoot = false;
            time = 0f;
            if (shootingTarget != null)
            {
                if (projObj != null)
                {
                    GameObject proj = PoolObject.SharedInstance.Spawn(projSpawnPoint.position, Quaternion.identity);
                    if (proj != null && proj.TryGetComponent(out Projectile projectileComponent))
                    {
                        Vector3 direction = (shootingTarget.transform.position - animator.transform.position).normalized;
                        projectileComponent.Initialize(direction);
                    }
                }
                else
                {
                    Debug.Log("Projectile not set.");
                }
            }
        }
        if (shootingTarget == null)
        {
            animator.SetTrigger(lostPlayerTrigger);
        }
    }

    /** 
     * TODO new way of finding target not using colliders,
     * should discuss which player to give shooting priority to,
     * for now, we first piorize shooting in the player shooter.
     * 
     * Ryan: it doesn't actually matter which instance we choose, if they're both in the same lane.
    */
    private PlayerController FindShootingTarget()
    {
        return GunPlayerController.LaneIndex == shootingIndex ? GunPlayerController.Instance :
            SwordPlayerController.LaneIndex == shootingIndex ? SwordPlayerController.Instance : null;
    }
}
