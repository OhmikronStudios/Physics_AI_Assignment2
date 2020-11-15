using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngageState : StateMachineBehaviour
{
    bool loaded;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        loaded = true;
        animator.ResetTrigger("EngageAnimation");
        SeekerBot script = animator.GetComponent<SeekerBot>();
        script.ActivateSeek();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SeekerBot script = animator.GetComponent<SeekerBot>();
        if(script.Attack() && loaded)
        {
            FindObjectOfType<KinematicInput>().HealthChange(10);
            loaded = false;
            animator.SetTrigger("DisengageAnimation");
        }
        else if(!script.TargetIsVisible())
        {
            animator.SetTrigger("WanderAnimation");
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
