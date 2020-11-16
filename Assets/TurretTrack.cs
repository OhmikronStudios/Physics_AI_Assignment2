using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretTrack : StateMachineBehaviour
{
    float timer;
    float timeToFire = 3.0f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0.0f;
        animator.ResetTrigger("TrackingState");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        TurretAI script = animator.GetComponent<TurretAI>();
        script.transform.LookAt(script.seekObject.transform);
        if (timer >= timeToFire)
        {
            FindObjectOfType<KinematicInput>().HealthChange(5);
            timer = 0.0f;
        }
        else if (!script.TargetIsVisible())
        {
            animator.SetTrigger("SearchingState");
            timer = 0.0f;
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
