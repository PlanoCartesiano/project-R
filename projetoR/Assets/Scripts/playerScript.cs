using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerScript : MonoBehaviour
{
    private Animator playerAnimator;
    private Rigidbody2D playerRb;
    private float h, v;
    public Collider2D standingCollider, crounchingCollider;

    public Transform groundCheck;
    public bool attackingState;
    public float speed;
    public float jumpForce;
    public bool leftDirection;
    public int idAnimation;
    public bool Grounded;

    // dash system variables
    public bool canDash = true;
    private bool isDashing;
    private float dashPower = 4f;
    private float dashingTime = 0.2f;
    private float dashCoolDown = 0.8f;

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
        }

        v = Input.GetAxisRaw("Vertical");
        h = Input.GetAxisRaw("Horizontal");

        if (h > 0 && leftDirection && !attackingState)
        {
            flip();
        }
        else if(h < 0 && !leftDirection && !attackingState)
        {
            flip();
        };

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

        if (Input.GetKeyDown(KeyCode.X) && v >= 0 && !attackingState && canDash)
        {
            Attack();
        }else if (Input.GetKeyDown(KeyCode.Z) && v >= 0 && Grounded && !isDashing && canDash)
        {
            Jump();
        }else if(Input.GetKeyDown(KeyCode.C) && v >= 0 && Grounded && canDash && !attackingState)
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

        Grounded = Physics2D.OverlapCircle(groundCheck.position, 0.02f);
        playerRb.velocity = new Vector2(h * speed, playerRb.velocity.y);
    }

    void flip()
    {
        leftDirection = !leftDirection;
        float x = transform.localScale.x;
        x *= -1;
        transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
    }

    void Attack()
    {
        playerAnimator.SetTrigger("atack");
        //return;
    }

    void Jump()
    {
        playerRb.velocity = Vector2.up * 4;
        //playerRb.AddForce(new Vector2(0f, jumpForce));
        //return;
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

    IEnumerator Dash()
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