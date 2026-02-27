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

    private GameObject[] lanes;
    private GameObject chosenLane;
    private GameObject playerShooter;
    private GameObject playerMelee;
    private Transform lookPoint;
    private bool found;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        lanes = GameObject.FindGameObjectsWithTag(LaneTag);
        ShooterEnemyAI component;
        if (animator.gameObject.TryGetComponent<ShooterEnemyAI>(out component)) { 
            playerMelee = component.playerMelee;
            playerShooter = component.playerShooter;
        }
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (chosenLane is null) {
            chosenLane = lanes[Random.Range(0, lanes.Length)];
            lookPoint = chosenLane.transform.Find(laneDestinationName);
        }
        
        if (lookPoint is not null)
        {
            animator.transform.position = Vector3.Lerp(animator.transform.position
                , lookPoint.position, Time.deltaTime);
            if (Vector3.Distance(animator.transform.position, lookPoint.position) <= distanceTreshold) {
                LaneBound shooterPlayerLane;
                LaneBound meleePlayerLane;
                IndexPoint indexPoint;
                if (playerShooter.TryGetComponent<LaneBound>(out shooterPlayerLane) && playerMelee.TryGetComponent<LaneBound>(out meleePlayerLane) && lookPoint.gameObject.TryGetComponent<IndexPoint>(out indexPoint))
                {
                    if (shooterPlayerLane.LaneIndex == indexPoint.Index) {
                        setLane(animator);
                        animator.SetTrigger(foundTriggerName);
                    } else if (meleePlayerLane.LaneIndex == indexPoint.Index) {
                        setLane(animator);
                        animator.SetTrigger(foundTriggerName);
                    } else
                    {
                        lookPoint = null;
                        chosenLane = null;
                    }

                }
                
                
                
            }
        }
        else {
            Debug.Log(laneDestinationName + " does not exist as the child of the LaneCollider");
        }

    }
    void setLane(Animator animator) {
        ShooterEnemyAI shooterAI;
        if (animator.TryGetComponent<ShooterEnemyAI>(out shooterAI))
        {
            shooterAI.shootingLane = chosenLane;
        }
        else
        {
            Debug.Log("ShooterEnemyAI script not attached to shooter animation controller.");
        }
    }
    /*if (chosenLane.TryGetComponent<SearchCollider>(out var searchCollider))
                {
                    if (searchCollider.players.Count != 0)
                    {
                        ShooterEnemyAI shooterAI;
                        if (animator.TryGetComponent<ShooterEnemyAI>(out shooterAI))
                        {
                            shooterAI.shootingLane = chosenLane;
                        }

                        animator.SetTrigger(foundTriggerName);

                    }
                }
                else {
                    Debug.Log("search collider script is not set line 41 SearchingBehavior.cs");
                }*/
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
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
