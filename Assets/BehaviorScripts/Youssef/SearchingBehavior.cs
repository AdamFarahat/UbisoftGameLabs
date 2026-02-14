using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SearchingBehavior : StateMachineBehaviour
{
    public string LaneTag = "LaneCollider";
    public string laneDestinationName = "LookTransform";
    public string foundTriggerName = "PlayerSeen";
    public float distanceTreshold = 2f;
    [SerializeField]
    private GameObject[] lanes;
    [SerializeField]
    private GameObject chosenLane;
    [SerializeField]
    private int currentLaneIndex = -1;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        lanes = GameObject.FindGameObjectsWithTag(LaneTag);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (chosenLane is null) {
            //making sure to not stay in the same lane
            int lastLaneIndex = currentLaneIndex;
            do {
                currentLaneIndex = Random.Range(0, lanes.Length);
            } while (currentLaneIndex == lastLaneIndex);
            
            chosenLane = lanes[currentLaneIndex]; 
        }
        Transform lookPoint = chosenLane.transform.Find(laneDestinationName);
        if (lookPoint is not null)
        {
            animator.transform.position = Vector3.Lerp(animator.transform.position
                , lookPoint.position, Time.deltaTime);
            if (Vector3.Distance(animator.transform.position, lookPoint.position) <= distanceTreshold) {
                if (chosenLane.TryGetComponent<SearchCollider>(out var searchCollider))
                {
                    if (searchCollider.players.Count != 0)
                    {
                        animator.SetTrigger(foundTriggerName);
                    }
                }
                else {
                    Debug.Log("search collider script is not set line 41 SearchingBehavior.cs");
                }
                    chosenLane = null;
            }
        }
        else {
            Debug.Log(laneDestinationName + " does not exist as the child of the LaneCollider");
        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<ShooterEnemyAI>().shootingLane = chosenLane;
    }

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
