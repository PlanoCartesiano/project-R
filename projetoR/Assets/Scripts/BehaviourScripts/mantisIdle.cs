using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mantisIdle : StateMachineBehaviour
{
    Rigidbody2D rb;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       rb = animator.GetComponentInParent<Rigidbody2D>();
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb.velocity = Vector2.zero;
        RunToPlayer(animator);

        if (mantisBoss.Instance.attackCountdown <= 0)
        {
            mantisBoss.Instance.AttackController();
            mantisBoss.Instance.attackCountdown = mantisBoss.Instance.attackTimer;
        }
    }
    void RunToPlayer(Animator animator)
    {
        if(Vector2.Distance(playerScript.Instance.transform.position, rb.position) >= mantisBoss.Instance.attackRange)
        {
            animator.SetBool("Run", true);
        }
        else
        {
            return;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

}
