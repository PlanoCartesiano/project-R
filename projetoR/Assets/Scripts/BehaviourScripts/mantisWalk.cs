using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mantisWalk : StateMachineBehaviour
{
    Rigidbody2D rb;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TargetPlayerPosition(animator);

        if(mantisBoss.Instance.attackCountdown <= 0)
        {
            mantisBoss.Instance.AttackController();
            mantisBoss.Instance.attackCountdown = mantisBoss.Instance.attackTimer;
        }
    }

    void TargetPlayerPosition(Animator animator)
    {
        if (mantisBoss.Instance.Grounded())
        {
            mantisBoss.Instance.Flip();
            Vector2 target = new Vector2(playerScript.Instance.transform.position.x, rb.position.y);
            Vector2 newPosition = Vector2.MoveTowards(rb.position, target, mantisBoss.Instance.runSpeed * Time.fixedDeltaTime);
            mantisBoss.Instance.runSpeed = mantisBoss.Instance.speed;
            rb.MovePosition(newPosition);
        }
        else if (!mantisBoss.Instance.Grounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, -25);
        }

        if (Vector2.Distance(playerScript.Instance.transform.position, rb.position) <= mantisBoss.Instance.attackRange)
        {
            animator.SetBool("Run", false);
        }
        else
        {
            return;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Run", false);
    }
}
