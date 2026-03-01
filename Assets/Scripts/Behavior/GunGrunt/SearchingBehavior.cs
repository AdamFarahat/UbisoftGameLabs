using UnityEngine;

public class SearchingBehavior : StateMachineBehaviour
{
    public string LaneTag = "LaneCollider";
    public string foundTriggerName = "PlayerSeen";
    public float distanceTreshold = 0.1f;

    private float researchCooldown = 0f;
    private int searchIndex;
    private float researchTime = 0f;

    private LaneBound laneBound;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        laneBound = animator.GetComponent<LaneBound>();
        researchCooldown = animator.GetComponent<ShooterEnemyAI>().ResearchCooldown;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (researchTime <= 0f)
        {
            researchTime = researchCooldown;
            searchIndex = Random.Range(0, LaneConfigSO.Instance.GetNumberOfLanes());
            laneBound.MoveToLane(searchIndex);
            animator.GetComponent<ShooterEnemyAI>().shootingIndex = searchIndex;
        }

        if (Mathf.Abs(laneBound.LaneIndex - searchIndex) <= distanceTreshold)
        {
            if (GunPlayerController.LaneIndex == searchIndex || SwordPlayerController.LaneIndex == searchIndex)
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
