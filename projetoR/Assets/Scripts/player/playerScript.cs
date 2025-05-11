using FMOD.Studio;
using Unity.Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class playerScript : MonoBehaviour
{
    public static playerScript Instance { get; private set; }
    private GameDataController gameDataController;
    [SerializeField] private GameObject cameraFollowGameObject;
    private cameraFollowObject cameraFollowObject;
    private HitEffect hitEffect;
    private float _fallSpeedYDampingChangeThreshold;
    private Animator playerAnimator;
    private string currentAnimationState;
    private Rigidbody2D playerRb;
    private float h, v;
    public Collider2D standingCollider, crounchingCollider;
    private TrailRenderer trailRenderer;

    [Header("Health Settings")]
    public int maximumHealth;
    public int currentHealth;
    public bool isHealing;
    public bool isDead = false;

    [Header("Slingshot variables")]
    public GameObject slingshotRock;
    public Transform slingshot;
    private bool isShooting;
    public float slingshotRockSpeed;
    [SerializeField] private bool hasCharge;
    [SerializeField] private GameObject chargedRock;
    [SerializeField] private float timeToCharge;
    private float chargeTime;

    [Header("Jump Buffer and Coyote Time")]
    private float coyoteTime = 0.05f;
    private float coyoteTimeCounter;
    private float jumpBufferTime = 0.2f;
    [SerializeField] private float jumpBufferCounter;

    [Header("Wall Slide & Wall Jump")]
    private bool isWallSliding;
    private float wallSlidingSpeed = 0.8f;
    private bool isWallJumping;
    private float wallJumpingDirection;
    private float MaxJumpingTime = 0.3f, MinJumpingTime = 0.1f;
    //private float wallJumpingCounter;
    private float wallJumpingTime;
    private Vector2 wallJumpingPower = new Vector2(1.2f, 3.2f);
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    [Header("Ladder Settings")]
    [SerializeField] private bool climbing;
    [SerializeField] private float climbingSpeed;
    public LayerMask ladderMask;
    public float checkRadius = 0.06f;

    [Header("EdgeDetection Settings")]
    public LayerMask edgeDetectionLayer;
    public Transform outerLeftLine;
    public Transform innerLeftLine;
    public Transform innerRightLine;
    public Transform outerRightLine;

    [Header("Parry Settings")]
    public Transform parryCheck;
    public bool isCollidingEnemyAttack;
    public LayerMask parryCheckLayer;
    [SerializeField] private float parryCooldown = 1.55f;
    private bool canParry = true;
    private bool isParrying = false;

    [Header("Platform System")]
    [SerializeField] private LayerMask platformLayer;
    private bool isOnPlatform;

    [Header("Camera Effect")]
    [SerializeField] private ScreenShakeProfile profile;
    private CinemachineImpulseSource impulseSource;

    [Header("Sounds Config")]
    private EventInstance footSteps;
    private EventInstance climbingLadder;

    [Header("Dash & Roll Config")]
    private bool canDash = false;

    [Header("Jump & DoubleJump sets")]
    public Transform groundCheck;
    public LayerMask ground;
    public bool isJumping;
    public float counterJump = 0.25f;
    public bool doubleJump;

    public Transform interactionRayCast;
    public LayerMask RayCastLayer;
    public GameObject currentInteractObject;
    public bool attackingState;
    public float speed;
    public bool IsFacingRight = true;
    private float direction = 1f;
    public int idAnimation;
    public bool Grounded;
    public int combo;
    public bool doubleAtack, lockAtack = false;
    public bool inTransition;

    private enum State
    {
        Normal,
        Rolling,
        Dead,
    }

    private enum AnimationState
    {
        idle,
        run,
        JumpFall,
        crounch,
        slingshot,
        wall_sliding,
        first_atack,
        second_atack,
        air_upper_atack,
        air_down_atack,
        dodgeRoll,
        climbingLadders,
        healing,
        hit,
        death,
        chargingSlingshot,
        readyChargedSlingshot,
        missParry,
        parry,
        dash,
    }

    private float rollSpeed;
    [SerializeField] private State state;

    public GameObject interactionButtonAlert;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        state = State.Normal;
        gameDataController = FindFirstObjectByType(typeof(GameDataController)) as GameDataController;
        cameraFollowObject = cameraFollowGameObject.GetComponent<cameraFollowObject>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        hitEffect = GetComponent<HitEffect>();
        playerRb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        trailRenderer = GetComponent<TrailRenderer>();

        footSteps = AudioManager.instance.CreateInstance(FMODEvents.instance.footSteps);
        climbingLadder = AudioManager.instance.CreateInstance(FMODEvents.instance.climbingLadder);

        maximumHealth = gameDataController.maximumHealth;
        currentHealth = maximumHealth;

        _fallSpeedYDampingChangeThreshold = cameraManager.instance._fallSpeedYDampingChangeThreshold;
    }

    void Update()
    {

        TurnCheck();

        if(gameDataController.currentState != GameState.RUN)
        {
            return;
        }

        switch (state)
        {
            case State.Normal:

                if(Grounded && !Input.GetButton("Jump"))
                {
                    doubleJump = false;
                }

                if (doubleJump && Input.GetButtonDown("Jump") && !isWallSliding && !isWallJumping)
                {
                    DoubleJump();
                }

                if (currentHealth <= 0)
                {
                    state = State.Dead;
                }

                if (isWallSliding)
                {
                    EndAllAttackState();
                }

                if (Grounded && !attackingState && !isShooting && !isHealing && !climbing && !isParrying && !inTransition)
                {
                    if(h != 0)
                    {
                        ChangeAnimationState(AnimationState.run.ToString());
                    }
                    else if(h == 0)
                    {
                        ChangeAnimationState(AnimationState.idle.ToString());
                    }
                }
                else if(!Grounded && !isShooting && !climbing)
                {
                    ChangeAnimationState(AnimationState.JumpFall.ToString());
                }

                if(counterJump == 0.25f)
                {
                    isJumping = false;
                }

                WallSlide();
                WallJump();

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

                if (Input.GetButtonDown("Fire1") && v > 0 && !isShooting && !isParrying && currentInteractObject == null)
                {
                    UpAttack();
                };

                if (Input.GetButtonDown("Fire1") && v < 0 && !Grounded && !isShooting)
                {
                    DownAttack();
                };

                if (Input.GetButtonDown("Fire1") && v == 0 && !lockAtack && !isShooting && !isParrying && currentInteractObject == null)
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

                if (Input.GetButtonDown("Fire2") && currentHealth < maximumHealth && !isParrying && !inTransition)
                {
                    StartCoroutine(Healing());
                }

                if (Input.GetButtonDown("Left Bumper") && canParry && Grounded)
                {
                    StartCoroutine(Parry());
                };

                if (Grounded)
                {
                    coyoteTimeCounter = coyoteTime;
                }
                else
                {
                    coyoteTimeCounter -= Time.deltaTime;
                }

                if(isOnPlatform && Input.GetButtonDown("Jump") && v < 0f)
                {
                    StartCoroutine(Drop());
                }
                else if (Input.GetButtonDown("Jump") && Grounded)
                {
                    jumpBufferCounter = jumpBufferTime;
                    doubleJump = !doubleJump;
                    canDash = true;
                    //emitterSFX[1].Play();
                }
                else
                {
                    jumpBufferCounter -= Time.deltaTime;
                }

                if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f && !isShooting && !isParrying)
                {
                    isJumping = true;
                    jumpBufferCounter = 0f;
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

                if (Input.GetButtonDown("Fire3") && Grounded && !isShooting && !attackingState && h != 0 && !isParrying)
                {
                    rollSpeed = 8f;
                    state = State.Rolling;
                    AudioManager.instance.PlayOneShotSound(FMODEvents.instance.Roll, this.transform.position);
                    playerAnimator.SetTrigger("dash");
                    //StartCoroutine(Dash());
                }else if (Input.GetButtonDown("Fire3") && !Grounded && !isShooting && !attackingState && h != 0 && !isParrying && canDash)
                {
                    canDash = false;
                    rollSpeed = 8f;
                    state = State.Rolling;
                    //AudioManager.instance.PlayOneShotSound(FMODEvents.instance.Roll, this.transform.position); dash sound here!
                    ChangeAnimationState(AnimationState.dash.ToString());
                    trailRenderer.emitting = true;
                }


                if (Input.GetButtonDown("Slingshot"))
                {
                    //ChangeAnimationState(AnimationState.slingshot.ToString());
                }
                if (!hasCharge) return;

                if (Input.GetButton("Slingshot"))
                {
                    chargeTime += Time.deltaTime;
                    if(chargeTime <= timeToCharge)
                    {
                        ChangeAnimationState(AnimationState.chargingSlingshot.ToString());
                    }else if (chargeTime > timeToCharge)
                    {
                        ChangeAnimationState(AnimationState.readyChargedSlingshot.ToString());
                    }
                }
                if (Input.GetButtonUp("Slingshot"))
                {
                    if (chargeTime >= timeToCharge)
                    {
                        isShooting = false;
                        ChangeAnimationState(AnimationState.idle.ToString());
                        ChargedShoot();
                    }
                    else
                    {
                        ChangeAnimationState(AnimationState.slingshot.ToString());
                    }
                    chargeTime = 0f;
                }

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
                    trailRenderer.emitting = false;
                }
                break;

            case State.Dead:
                StopAllCoroutines();
                EndAllAttackState();
                ChangeAnimationState(AnimationState.death.ToString());
                h = 0;
                isDead = true;
                break;
        }

        if(attackingState && Grounded)
        {
            h = 0;
        }else if(isShooting && Grounded)
        {
            h = 0;
        }else if (isParrying)
        {
            h = 0;
        }else if (isHealing)
        {
            h = 0;
        }
        else if (inTransition)
        {
            h = 0;
        }

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
        playerAnimator.SetBool("wallSliding", isWallSliding);
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

                    if (counterJump > 0f)
                    {
                        playerRb.velocity = Vector2.up * 2f;
                    }
                    else
                    {
                        isJumping = false;
                    }
                }

                if (playerRb.velocity.y > 0)
                {
                    EdgeDetection();
                }

                if (h > 0 || h < 0)
                {
                    TurnCheck();
                }

                Grounded = Physics2D.OverlapCircle(groundCheck.position, 0.02f, ground);
                isOnPlatform = Physics2D.OverlapCircle(groundCheck.position, 0.02f, platformLayer);

                if (isWallJumping)
                {
                    playerRb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, playerRb.velocity.y);
                }
                else
                {
                    playerRb.velocity = new Vector2(h * speed, playerRb.velocity.y);
                }

                isCollidingEnemyAttack = Physics2D.OverlapBox(parryCheck.position, new Vector2(0.2f, 0.25f), 0, parryCheckLayer);

                Interact();
                ClimbLadder();
                break;

            case State.Rolling:
                StartCoroutine(Invulnerable());
                if (Grounded)
                {
                    ChangeAnimationState(AnimationState.dodgeRoll.ToString());
                    playerRb.velocity = new Vector2(h * rollSpeed, -2f); // Criar um método único para chamar esse método dentro de um sprite (Event dentro do sprite de Roll)
                }else if (!Grounded)
                {
                    ChangeAnimationState(AnimationState.dash.ToString());
                    playerRb.velocity = new Vector2(h * rollSpeed, 0f);
                }
                break;
        }

        UpdateSound();

    }

    private void TurnCheck()
    {
        if (h > 0 && !IsFacingRight && !attackingState && !isWallJumping)
        {
            Turn();
        }
        else if (h < 0 && IsFacingRight && !attackingState && !isWallJumping)
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
            slingshotRockSpeed *= -1;
            IsFacingRight = !IsFacingRight;
            direction *= -1;
            //cameraFollowObject.CallTurn();
        }
        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            slingshotRockSpeed *= -1;
            IsFacingRight = !IsFacingRight;
            direction *= -1;
            //cameraFollowObject.CallTurn();
        }
    }

    void UpAttack()
    {
        playerAnimator.SetBool("upAttack", true);
        AudioManager.instance.PlayOneShotSound(FMODEvents.instance.attackSlash, this.transform.position);
    }

    void DownAttack()
    {
        playerAnimator.SetBool("downAttack", true);
        AudioManager.instance.PlayOneShotSound(FMODEvents.instance.attackSlash, this.transform.position);
    }

    void Attack()
    {
        playerAnimator.SetBool("firstAttack", true);
        playerAnimator.SetBool("secondAttack", false);
        AudioManager.instance.PlayOneShotSound(FMODEvents.instance.attackSlash, this.transform.position);
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

    void DoubleJump()
    {
        playerRb.velocity = new Vector2(playerRb.velocity.x, 0);
        playerRb.velocity = Vector2.up * 3.2f;
        doubleJump = false;
    }

    void WallJump()
    {
        if (isWallSliding && Input.GetButtonDown("Jump"))
        {
            wallJumpingDirection = IsFacingRight ? -1 : 1;
            wallJumpingTime = 0f;
            doubleJump = true;
            isWallJumping = true;
            playerRb.velocity = new Vector2(playerRb.velocity.x, wallJumpingPower.y);
            StartCoroutine(WallJumping());
        }
    }

    bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.1f, wallLayer);
    }

    void WallSlide()
    {
        if(IsWalled() && !Grounded && h != 0f && !climbing && playerRb.velocity.y < 0)
        {
            canDash = true;
            isWallSliding = true;
            playerRb.velocity = new Vector2(playerRb.velocity.x, Mathf.Clamp(playerRb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    bool OnLadder()
    {
        return standingCollider.IsTouchingLayers(ladderMask);
    }
    void ClimbLadder()
    {

        bool up = Physics2D.OverlapCircle(transform.position, checkRadius, ladderMask);
        bool down = Physics2D.OverlapCircle(transform.position + new Vector3(0, -0.2f), checkRadius, ladderMask);

        if(v != 0 && OnLadder())
        {
            climbing = true;
        }
        else
        {
            climbing = false;
        }

        if (climbing)
        {
            if(!up && v >= 0)
            {
                FinishClimbing();
                return;
            }

            if (!down && v <= 0)
            {
                FinishClimbing();
                return;
            }

            ChangeAnimationState(AnimationState.climbingLadders.ToString());
            playerAnimator.SetFloat("climbingSpeed", v);
            playerRb.velocity = new Vector2(0, v * climbingSpeed);

            if (Input.GetButtonDown("Jump"))
            {
                FinishClimbing();

                int direction = Convert.ToInt32(IsFacingRight);
                if (h != 0)
                    direction = h > 0 ? 1 : -1;

                playerRb.AddForce(new Vector2(6 * direction, 2), ForceMode2D.Impulse);
            }
        }
    }

    void EdgeDetection()
    {
        RaycastHit2D edgeDetection1 = Physics2D.Raycast(outerLeftLine.position, transform.up, 0.1f, edgeDetectionLayer);
        Debug.DrawRay(outerLeftLine.position, transform.up * 0.1f, Color.blue);

        RaycastHit2D edgeDetection2 = Physics2D.Raycast(innerLeftLine.position, transform.up, 0.1f, edgeDetectionLayer);
        Debug.DrawRay(innerLeftLine.position, transform.up * 0.1f, Color.blue);

        RaycastHit2D edgeDetection3 = Physics2D.Raycast(innerRightLine.position, transform.up, 0.1f, edgeDetectionLayer);
        Debug.DrawRay(innerRightLine.position, transform.up * 0.1f, Color.blue);

        RaycastHit2D edgeDetection4 = Physics2D.Raycast(outerRightLine.position, transform.up, 0.1f, edgeDetectionLayer);
        Debug.DrawRay(outerRightLine.position, transform.up * 0.1f, Color.blue);

        if (outerRightLine && !innerRightLine && !innerLeftLine && !outerLeftLine)
        {
            transform.position = new Vector2(-5f, transform.position.y);
        }
        else if(outerLeftLine && !innerLeftLine && innerRightLine && !outerRightLine)
        {
            transform.position = new Vector2(-5f, transform.position.y);
        }
    }

    void FinishClimbing()
    {
        climbing = false;
    }

    void Shoot()
    {
        GameObject temp = Instantiate(slingshotRock);
        temp.transform.position = slingshot.position;
        temp.GetComponent<Rigidbody2D>().velocity = new Vector2(slingshotRockSpeed, 0f);
        Destroy(temp.gameObject, 1.5f);
    }

    void ChargedShoot()
    {
        GameObject temp = Instantiate(chargedRock);
        temp.transform.position = slingshot.position;
        temp.GetComponent<Rigidbody2D>().velocity = new Vector2(slingshotRockSpeed, 0f);
        Destroy(temp.gameObject, 1.5f);
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

    void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState)
        {
            return;
        }

        if (currentAnimationState == "slingshot" && newState == "chargingSlingshot")
        {
            return;
        }

        playerAnimator.Play(newState);
        currentAnimationState = newState;
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

    public void EndAllAttackState()
    {
        attackingState = false;
        isShooting = false;
        finishAttackAnimation();
        finishDoubleAttackAnimation();
        finishUpAttackAnimation();
        finishDownAttackAnimation();
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

    public void shoot(int shoot)
    {
        switch (shoot)
        {
            case 0:
                isShooting = false;
                attackingState = false;
                break;

            case 1:
                isShooting = true;
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
        if (attackingState)
        {
            EndAllAttackState();
        }

        hitEffect.HitBlinkEffect();
        playerAnimator.SetTrigger("hit");
        //CameraShakeManager.instance.CameraShake(impulseSource);
        CameraShakeManager.instance.ScreenShakeFromProfile(profile, impulseSource);
        currentHealth -= 1;
    }

    public void Death()
    {
        EndAllAttackState();
        hitDamage();
        Debug.Log("Rafaela morreu");
    }

    private IEnumerator WallJumping()
    {
        while (wallJumpingTime < MaxJumpingTime && Input.GetButton("Jump") || wallJumpingTime < MinJumpingTime) 
        {
            wallJumpingTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        isWallJumping = false;
        wallJumpingTime = 0f;
    }

    private IEnumerator Invulnerable()
    {
        Physics2D.IgnoreLayerCollision(7, 14, true);

        yield return new WaitForSeconds(0.2f);

        Physics2D.IgnoreLayerCollision(7, 14, false);
    }

    private IEnumerator Healing()
    {
        isHealing = true;
        //emitterSFX[3].Play();
        ChangeAnimationState(AnimationState.healing.ToString());
        currentHealth += 1;

        yield return new WaitForSeconds(0.6f);

        isHealing = false;
    }

    private IEnumerator Drop()
    {
        Physics2D.IgnoreLayerCollision(13, 14, true);

        yield return new WaitForSeconds(0.2f);

        Physics2D.IgnoreLayerCollision(13, 14, false);
    }

    private IEnumerator Parry()
    {

        isParrying = true;
        canParry = false;

        ChangeAnimationState(AnimationState.missParry.ToString());
        AudioManager.instance.PlayOneShotSound(FMODEvents.instance.missParry, this.transform.position);

        if (isCollidingEnemyAttack)
        {
            ChangeAnimationState(AnimationState.parry.ToString());
            AudioManager.instance.PlayOneShotSound(FMODEvents.instance.parryStrike, this.transform.position);

            yield return new WaitForSeconds(0.8f);

            isParrying = false;
            canParry = true;

            ChangeAnimationState(AnimationState.idle.ToString());

            yield break;
        }

        yield return new WaitForSeconds(parryCooldown);

        ChangeAnimationState(AnimationState.idle.ToString());

        isParrying = false;
        canParry = true;

    }

    private void UpdateSound()
    {

        if (playerRb.velocity.x != 0 && Grounded)
        {
            PLAYBACK_STATE playbackState;
            footSteps.getPlaybackState(out playbackState);

            if(playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                footSteps.start();
            }
        }
        else
        {
            footSteps.stop(STOP_MODE.IMMEDIATE);
        }

        if (playerRb.velocity.y != 0 && climbing)
        {
            PLAYBACK_STATE playbackState;
            climbingLadder.getPlaybackState(out playbackState);

            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                climbingLadder.start();
            }
        }
        else
        {
            climbingLadder.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(transform.position, checkRadius);
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, -0.2f), checkRadius);

        Gizmos.DrawWireCube(parryCheck.position, new Vector3(0.2f, 0.25f));
    }
}