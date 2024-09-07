using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mantisJump : StateMachineBehaviour
{
    Rigidbody2D rb;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        DiveAttack();
    }

    void DiveAttack()
    {
        if(mantisBoss.Instance.specialDiveAttack)
        {
            mantisBoss.Instance.Flip();

            Vector2 newPosition = Vector2.Lerp(rb.position, mantisBoss.Instance.moveToPosition, mantisBoss.Instance.speed * 10 * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);

            float distance = Vector2.Distance(rb.position, newPosition);
            if(distance < 0.03f)
            {
                mantisBoss.Instance.Dive();
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }
}
