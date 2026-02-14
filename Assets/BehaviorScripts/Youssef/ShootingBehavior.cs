using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ShootingBehavior : StateMachineBehaviour
{
    public string lostPlayerTrigger = "PlayerDisapearred";
    public float shootingRate = 1.0f;
    public GameObject projObj;
    private GameObject shootingLane;
    private GameObject shootingTarget;
    private float time;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        shootingLane = animator.GetComponent<ShooterEnemyAI>()?.shootingLane;
        shootingTarget = shootingLane?.GetComponent<SearchCollider>()?.players?.FirstOrDefault();
        time = 0f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        time += Time.deltaTime;
        if (time >= shootingRate) {
            time = 0f;
            if (shootingTarget != null)
            {
                if (projObj != null)
                {
                    GameObject proj = GameObject.Instantiate(projObj, animator.transform.position, Quaternion.identity);
                    Projectile projectileComponent;
                    if (proj != null && proj.TryGetComponent<Projectile>(out projectileComponent)) {
                        Vector3 direction = (shootingTarget.transform.position - animator.transform.position).normalized;
                        projectileComponent.Initialize(direction);
                    }
                }
                else {
                    Debug.Log("Projectile not set.");
                }
            }
            else {
                shootingTarget = shootingLane?.GetComponent<SearchCollider>()?.players?.FirstOrDefault();
                if (shootingTarget == null) {
                    animator.SetTrigger(lostPlayerTrigger);
                }
            } 
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
