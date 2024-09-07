using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mantisLunge : StateMachineBehaviour
{

    Rigidbody2D rb;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb.gravityScale = 0;
        int dir = mantisBoss.Instance.isFacingRight ? 1 : -1;
        rb.velocity = new Vector2(dir * (mantisBoss.Instance.speed * 5f), 0);

        if (Vector2.Distance(playerScript.Instance.transform.position,rb.position) <= mantisBoss.Instance.attackRange && !mantisBoss.Instance.damagedPlayer)
        {
            playerScript.Instance.hitDamage();
            mantisBoss.Instance.damagedPlayer = true;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }

}
