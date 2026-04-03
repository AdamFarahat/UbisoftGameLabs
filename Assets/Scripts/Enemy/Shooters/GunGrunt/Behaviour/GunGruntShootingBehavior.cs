using UnityEngine;

public class GunGruntShootingBehavior : StateMachineBehaviour
{
    public string lostPlayerTrigger = "PlayerDisappeared";
    public GameObject projObj;
    private bool shootingTarget = false;

    private GunGruntEnemyAI shooterAI;

    private float time;
    private bool firstShoot;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        shooterAI = animator.GetComponent<GunGruntEnemyAI>();
        shootingTarget = PlayerController.AnyPlayerInLane(shooterAI.shootingIndex);
        time = 0f;
        firstShoot = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!shooterAI.ActiveAI)
            return;

        time += Time.deltaTime;
        if (time >= shooterAI.ShootingCooldown || firstShoot)
        {
            shootingTarget = PlayerController.AnyPlayerInLane(shooterAI.shootingIndex);
            firstShoot = false;
            time = 0f;
            if (shootingTarget)
            {
                if (projObj != null)
                {
                    GameObject proj = ProjectilePool.SharedInstance.Spawn(shooterAI.projSpawnPoint.position, Quaternion.identity);
                    if (proj != null && proj.TryGetComponent(out Bullet projectileComponent))
                    {
                        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.EnemyWeaponShot, shooterAI.transform.position);
                        Vector3 direction = LaneSet.Instance.GetLanePosition(animator.GetComponent<LaneBound>().LaneIndex, LaneSet.PlayerLine) - shooterAI.projSpawnPoint.position;
                        direction.y = 0f;
                        projectileComponent.Initialize(shooterAI.projSpawnPoint, direction, shooterAI.BulletSpeed, Bullet.ProjectileState.ShotByEnemy);
                    }
                }
                else
                {
                    Debug.Log("Projectile not set.");
                }
            }
        }

        if (!shootingTarget)
            animator.SetTrigger(lostPlayerTrigger);
    }
}
