using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mantisBounce2 : StateMachineBehaviour
{
    Rigidbody2D rb;
    bool callOnce;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 forceDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * mantisBoss.Instance.rotationToTarget), Mathf.Sin(Mathf.Deg2Rad * mantisBoss.Instance.rotationToTarget));
        rb.AddForce(forceDirection, ForceMode2D.Impulse);

        mantisBoss.Instance.divingCollider.SetActive(true);

        if (mantisBoss.Instance.Grounded())
        {
            mantisBoss.Instance.divingCollider.SetActive(false);
            if (!callOnce)
            {
                mantisBoss.Instance.ResetAllAttacks();
                mantisBoss.Instance.CheckBounce();
                callOnce = true;
            }
            animator.SetTrigger("Grounded");
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Bounce2");
        animator.ResetTrigger("Grounded");
        callOnce = false;
    }
}
