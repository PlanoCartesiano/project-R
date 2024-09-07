using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mantisSpecial : StateMachineBehaviour
{
    Rigidbody2D rb;
    bool callOnce;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        mantisBoss.Instance.divingCollider.SetActive(true);

        if (mantisBoss.Instance.Grounded())
        {
            if (!callOnce)
            {
                mantisBoss.Instance.divingCollider.SetActive(false);
                mantisBoss.Instance.InstantiateThornPillars();
                animator.SetBool("Special", false);
                mantisBoss.Instance.ResetAllAttacks();
                callOnce = true;
            }
        }


    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        callOnce = false;
    }
}
