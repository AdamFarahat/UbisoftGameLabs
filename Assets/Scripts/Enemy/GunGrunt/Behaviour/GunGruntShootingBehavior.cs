using UnityEngine;

public class GunGruntShootingBehavior : StateMachineBehaviour
{
    public string lostPlayerTrigger = "PlayerDisappeared";
    public float shootingRate = 1.0f;
    public GameObject projObj;
    private PlayerController shootingTarget;

    private GunGruntEnemyAI shooterAI;

    private float time;
    private bool firstShoot;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        shooterAI = animator.GetComponent<GunGruntEnemyAI>();
        shootingTarget = FindShootingTarget();
        time = 0f;
        firstShoot = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!shooterAI.ActiveAI)
            return;

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
                    GameObject proj = PoolObject.SharedInstance.Spawn(shooterAI.projSpawnPoint.position, Quaternion.identity);
                    if (proj != null && proj.TryGetComponent(out EnemyProjectile projectileComponent))
                    {
                        Vector3 direction = shootingTarget.transform.position - animator.transform.position;
                        direction.y = 0f;
                        projectileComponent.Initialize(direction.normalized);
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

    private PlayerController FindShootingTarget()
    {
        return GunPlayerController.LaneIndex == shooterAI.shootingIndex ? GunPlayerController.Instance :
            SwordPlayerController.LaneIndex == shooterAI.shootingIndex ? SwordPlayerController.Instance : null;
    }
}
