using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;

public class playerScript : MonoBehaviour
{
    private GameDataController gameDataController;
    [SerializeField] private GameObject cameraFollowGameObject;
    private cameraFollowObject cameraFollowObject;
    private float _fallSpeedYDampingChangeThreshold;
    private Animator playerAnimator;
    private Rigidbody2D playerRb;
    private float h, v;
    public Collider2D standingCollider, crounchingCollider;

    [Header("Health Settings")]
    public int maximumHealth;
    public int currentHealth;

    public Transform interactionRayCast;
    public LayerMask RayCastLayer;
    public GameObject currentInteractObject;
    public Transform groundCheck;
    public LayerMask ground;
    public bool attackingState;
    public float speed;
    public bool isJumping;
    public float counterJump = 0.25f;
    public bool IsFacingRight = true;
    public int idAnimation;
    public bool Grounded;
    public int combo;
    public bool doubleAtack, lockAtack = false;

    private enum State
    {
        Normal,
        Rolling,
    }

    private float rollSpeed;
    private State state;

    public GameObject interactionButtonAlert;

    void Start()
    {
        state = State.Normal;
        gameDataController = FindFirstObjectByType(typeof(GameDataController)) as GameDataController;
        cameraFollowObject = cameraFollowGameObject.GetComponent<cameraFollowObject>();
        playerRb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();

        maximumHealth = gameDataController.maximumHealth;
        currentHealth = maximumHealth;

        _fallSpeedYDampingChangeThreshold = cameraManager.instance._fallSpeedYDampingChangeThreshold;
    }

    void Update()
    {

        if(gameDataController.currentState != GameState.RUN)
        {
            return;
        }

        switch (state)
        {
            case State.Normal:

                /*if (playerRb.velocity.y < _fallSpeedYDampingChangeThreshold && !cameraManager.instance.IsLerpingYDamping && !cameraManager.instance.LerpedFromPlayerFalling)
                {
                    cameraManager.instance.LerpYDamping(true);
                }

                if (playerRb.velocity.y >= 0f && !cameraManager.instance.IsLerpingYDamping && cameraManager.instance.LerpedFromPlayerFalling)
                {
                    cameraManager.instance.LerpedFromPlayerFalling = false;

                    cameraManager.instance.LerpYDamping(false);
                }*/

                h = Input.GetAxisRaw("Horizontal");

                v = Input.GetAxisRaw("Vertical");

                if (v < 0)
                {
                    idAnimation = 2;
                    if (Grounded)
                    {
                        h = 0;
                    }
                }
                else if (h != 0)
                {
                    idAnimation = 1;
                }
                else
                {
                    idAnimation = 0;
                };

                if (Input.GetButtonDown("Fire1") && v > 0 && currentInteractObject == null)
                {
                    UpAttack();
                };

                if (Input.GetButtonDown("Fire1") && v < 0 && !Grounded)
                {
                    DownAttack();
                };

                if (Input.GetButtonDown("Fire1") && v == 0 && !lockAtack && currentInteractObject == null)
                {
                    Attack();

                    if (doubleAtack)
                    {
                        doubleAttack();
                    };
                };

                if (Input.GetButtonDown("Fire1") && v >= 0 && !lockAtack && currentInteractObject != null)
                {
                    if (currentInteractObject.tag == "Door")
                    {
                        currentInteractObject.GetComponent<Doors>().tPlayer = this.transform;
                    }
                    currentInteractObject.SendMessage("Interaction", SendMessageOptions.DontRequireReceiver);
                };

                if (Input.GetButtonDown("Jump") && v >= 0 && Grounded) // tirei o !isDashing e o canDash
                {
                    isJumping = true;
                }
                if (Input.GetButton("Jump"))
                {
                    counterJump -= Time.deltaTime;
                }
                if (Input.GetButtonUp("Jump"))
                {
                    isJumping = false;
                    counterJump = 0.25f;
                };

                if (Input.GetButtonDown("Fire3") && Grounded && !attackingState && h != 0) // tirei o && canDash
                {
                    playerAnimator.SetTrigger("dash");
                    rollSpeed = 8f;
                    state = State.Rolling;
                    //StartCoroutine(Dash());
                };
                break;

            case State.Rolling:
                float rollSpeedDropMultiplier = 10f;
                rollSpeed -= rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;

                float rollSpeedMinimum = 0.5f;
                if (rollSpeed < rollSpeedMinimum)
                {
                    state = State.Normal;
                    playerRb.velocity = new Vector2(h * speed, playerRb.velocity.y);
                    isJumping = false;
                }
                break;
        }


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
        if(gameDataController.currentState != GameState.RUN)
        {
            return;
        }

        switch (state)
        {
            case State.Normal:

                if (isJumping)
                {
                    if (counterJump > 0)
                    {
                        playerRb.velocity = Vector2.up * 2f;
                    }
                    else
                    {
                        isJumping = false;
                    }
                }

                if (h > 0 || h < 0)
                {
                    TurnCheck();
                }

                Grounded = Physics2D.OverlapCircle(groundCheck.position, 0.02f, ground);
                playerRb.velocity = new Vector2(h * speed, playerRb.velocity.y);


                Interact();
                break;

            case State.Rolling:
                playerRb.velocity = new Vector2(h * rollSpeed, 0f);
                break;
        }
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
            cameraFollowObject.CallTurn();
        }
        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            IsFacingRight = !IsFacingRight;
            cameraFollowObject.CallTurn();
        }
    }

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
            interactionButtonAlert.SetActive(true);
        }
        else
        {
            currentInteractObject = null;
            interactionButtonAlert.SetActive(false);
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

    public void OnTriggerEnter2D(Collider2D collision)
    {
        switch(collision.gameObject.tag)
        {
            case "collectible":
                collision.gameObject.SendMessage("collect", SendMessageOptions.DontRequireReceiver);
            break;
        }
    }

    public void hitDamage()
    {
        playerAnimator.SetTrigger("hit");
        currentHealth -= 1;
    }
}