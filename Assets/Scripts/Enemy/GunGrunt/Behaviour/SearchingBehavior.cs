using UnityEngine;

public class SearchingBehavior : StateMachineBehaviour
{
    public string LaneTag = "LaneCollider";
    public string foundTriggerName = "PlayerSeen";
    public float distanceTreshold = 0.1f;

    ShooterEnemyAI shooterAI;
    private float researchTime = 0f;

    private LaneBound laneBound;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        laneBound = animator.GetComponent<LaneBound>();
        shooterAI = animator.GetComponent<ShooterEnemyAI>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!shooterAI.ActiveAI)
            return;

        if (researchTime <= 0f)
        {
            researchTime = shooterAI.ResearchCooldown;
            shooterAI.shootingIndex = Random.Range(0, LaneConfigSO.Instance.GetNumberOfLanes());
            laneBound.MoveToLane(shooterAI.shootingIndex);
        }

        if (Mathf.Abs(laneBound.LaneIndex - shooterAI.shootingIndex) <= distanceTreshold)
        {
            if (GunPlayerController.LaneIndex == shooterAI.shootingIndex || SwordPlayerController.LaneIndex == shooterAI.shootingIndex)
            {
                animator.SetTrigger(foundTriggerName);
            }
            else
            {
                researchTime -= Time.deltaTime;
            }
        }
    }
}
