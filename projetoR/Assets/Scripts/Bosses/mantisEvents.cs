using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mantisEvents : MonoBehaviour
{
    void SlashDamagePlayer()
    {
        if(playerScript.Instance.transform.position.x > transform.position.x || playerScript.Instance.transform.position.x < transform.position.x)
        {
            Hit(mantisBoss.Instance.attackTransform, mantisBoss.Instance.attackArea);
        }
    }

    void Hit(Transform attackTransform, Vector2 attackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(attackTransform.position, attackArea, 0);
        for(int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<playerScript>() != null)
            {
                objectsToHit[i].GetComponent<playerScript>().hitDamage();
            }
        }
    }

    void BendCheck()
    {
        if (mantisBoss.Instance.bounceJumpAttack)
        {
            mantisBoss.Instance.mantisAnimator.SetTrigger("Bounce1");
        }
    }

    void DestroyAfterDead()
    {
        mantisBoss.Instance.DestroyAfterDead();
    }
}
