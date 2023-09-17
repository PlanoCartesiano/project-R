using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerScript : MonoBehaviour
{
    private Animator playerAnimator;
    private Rigidbody2D playerRb;
    private float h, v;
    public Collider2D standingCollider, crounchingCollider;

    public Transform interactionRayCast;
    public LayerMask RayCastLayer;
    public GameObject currentInteractObject;
    public Transform groundCheck;
    public bool attackingState;
    public float speed;
    public bool isJumping;
    public float counterJump = 0.25f;
    public bool IsFacingRight = true;
    public int idAnimation;
    public bool Grounded;
    public int combo;
    public bool doubleAtack, lockAtack = false;

    // new dash system variables
    public bool canDash = true;
    public bool isDashing = false;
    private float dashingTime = 0.05f;

    // dash system variables
    private float dashPower = 1.2f;
    private float dashCoolDown = 1.3f;

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
    }

    void Update()
    {

        if (isDashing)
        {
            return;
        };

        if (canDash)
        {
            h = Input.GetAxisRaw("Horizontal");
        };

        if (!isDashing)
        {
            v = Input.GetAxisRaw("Vertical");
        }

        if(v < 0)
        {
            idAnimation = 2;
            if(Grounded)
            {
                h = 0;
            }
        }else if(h != 0)
        {
            idAnimation = 1;
        }else
        {
            idAnimation = 0;
        };

        if (Input.GetKeyDown(KeyCode.X) && v > 0 && currentInteractObject == null)
        {
            UpAttack();
        };

        if (Input.GetKeyDown(KeyCode.X) && v < 0 && !Grounded)
        {
            DownAttack();
        };

        if (Input.GetKeyDown(KeyCode.X) && v == 0 && canDash && !lockAtack && currentInteractObject == null)
        {
            Attack();

            if (doubleAtack)
            {
                doubleAttack();
            };
        };

        if (Input.GetKeyDown(KeyCode.X) && v >= 0 && canDash && !lockAtack && currentInteractObject != null)
        {
            currentInteractObject.SendMessage("Interaction", SendMessageOptions.DontRequireReceiver);
        };

        if (Input.GetKeyDown(KeyCode.Z) && v >= 0 && Grounded && !isDashing && canDash)
        {
            isJumping = true;
        }
        if (Input.GetKey(KeyCode.Z))
        {
            counterJump -= Time.deltaTime;
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            isJumping = false;
            counterJump = 0.25f;
        };
        
        if(Input.GetKeyDown(KeyCode.C) && h != 0 && Grounded && canDash && !attackingState)
        {
            StartCoroutine(Dash());
        };

        if(attackingState && Grounded)
        {
            h = 0;
        };

        if (v < 0 && Grounded)
        {
            crounchingCollider.enabled = true;
            standingCollider.enabled = false;
        }
        else
        {
            crounchingCollider.enabled = false;
            standingCollider.enabled = true;
        }

        playerAnimator.SetBool("Grounded", Grounded);
        playerAnimator.SetInteger("idAnimation", idAnimation);
        playerAnimator.SetFloat("speedY", playerRb.velocity.y);
        //playerRb.velocity = new Vector2(h * speed, playerRb.velocity.y);
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        if (isJumping)
        {
            if(counterJump > 0)
            {
                playerRb.velocity = Vector2.up * 2f;
            }
            else
            {
                isJumping = false;
            }
        }

        if(h > 0 || h < 0)
        {
            TurnCheck();
        }

        Grounded = Physics2D.OverlapCircle(groundCheck.position, 0.02f);
        playerRb.velocity = new Vector2(h * speed, playerRb.velocity.y);


        Interact();
    }

    private void TurnCheck()
    {
        if (h > 0 && !IsFacingRight && !attackingState)
        {
            Turn();
        }
        else if (h < 0 && IsFacingRight && !attackingState)
        {
            Turn();
        };
    }

    private void Turn()
    {
        if (IsFacingRight)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight;
        }
        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight;
        }
    }

    /*void flip()
    {
        leftDirection = !leftDirection;
        float x = transform.localScale.x;
        x *= -1;
        transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
    }*/

    void UpAttack()
    {
        playerAnimator.SetBool("upAttack", true);
    }

    void DownAttack()
    {
        playerAnimator.SetBool("downAttack", true);
    }

    void Attack()
    {
        playerAnimator.SetBool("firstAttack", true);
        playerAnimator.SetBool("secondAttack", false);
    }

    void doubleAttack()
    {
        playerAnimator.SetBool("secondAttack", true);
        playerAnimator.SetBool("firstAttack", false);
    }

    void Jump()
    {
        playerRb.velocity = Vector2.up * 3.2f;
    }

    void Interact()
    {
        RaycastHit2D interactionRaycast2D = Physics2D.Raycast(interactionRayCast.position, transform.right, 0.1f, RayCastLayer);
        Debug.DrawRay(interactionRayCast.position, transform.right * 0.1f, Color.red);

        if(interactionRaycast2D == true)
        {
            currentInteractObject = interactionRaycast2D.collider.gameObject;
        }
        else
        {
            currentInteractObject = null;
        };
    }

    public void finishAttackAnimation()
    {
        playerAnimator.SetBool("firstAttack", false);
    }

    public void finishDoubleAttackAnimation()
    {
        playerAnimator.SetBool("secondAttack", false);
        doubleAtack = false;
        attackingState = false;
    }

    public void finishUpAttackAnimation()
    {
        playerAnimator.SetBool("upAttack", false);
    }

    public void finishDownAttackAnimation()
    {
        playerAnimator.SetBool("downAttack", false);
    }

    public void atk(int atk)
    {
        switch(atk)
        {
            case 0:
                attackingState = false;
                break;

            case 1:
                attackingState = true; 
                break;
        };
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        float originalGravity = playerRb.gravityScale;
        playerRb.gravityScale = 0f;
        playerAnimator.SetTrigger("dash");
        playerRb.velocity = Vector2.zero;
        playerRb.velocity = new Vector2(transform.localScale.x * dashPower, 0f);
        yield return new WaitForSeconds(dashingTime);
        playerRb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;
    }
}