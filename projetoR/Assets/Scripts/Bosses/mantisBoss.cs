using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class mantisBoss : MonoBehaviour
{

    public static mantisBoss Instance;

    public Rigidbody2D mantisRb;
    public SpriteRenderer mantisRender;
    public Animator mantisAnimator;

    public Transform mantisGroundCheck;
    public float groundCheckX = 0.5f;
    public float groundCheckY = 0.2f;
    public LayerMask ground;

    public bool isFacingRight;


    public Transform attackTransform;
    public Vector2 attackArea;
    public float attackTimer;
    public float attackRange;
    public bool attacking;
    public float attackCountdown;
    public bool damagedPlayer = false;

    [Header("Boss Status")]
    public int healthPoint = 300;
    private Color originalColor;

    [Header("jumpAttack")]
    public bool bounceJumpAttack;
    public int bounceCount;
    public int bounces = 0;
    public float rotationToTarget;

    [Header("Special Dive Attack")]
    public Vector2 moveToPosition;
    public bool specialDiveAttack;
    public GameObject divingCollider;
    public GameObject thornPillar;

    public float speed = 0.8f;
    public float runSpeed;

    int hitCounter;
    bool stunned, canStun;
    bool dead;

    [SerializeField] private State state;
    private string currentAnimationState;

    private enum State
    {
        stage1,
        stage2,
        stage3,
        stage4,
    }

    private enum AnimationState
    {
        intro,
        idle,
        walk,
        jump,
        stunned,
        atack,
        circleAtack,
        specialAtack,
        death,
    }

    public void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        mantisRb = GetComponent<Rigidbody2D>();
        mantisRender = GetComponentInChildren<SpriteRenderer>();
        originalColor = mantisRender.color;
        mantisAnimator = GetComponentInChildren<Animator>();
        state = State.stage1;
        dead = false;
    }

    void Update()
    {

        if(healthPoint <= 0 && !dead)
        {
            Death();
        }

        if (!attacking)
        {
            attackCountdown -= Time.deltaTime;
        }

        if (stunned)
        {
            mantisRb.velocity = Vector2.zero;
        }

        //Debug.Log(Vector2.Distance(playerScript.Instance.transform.position, mantisRb.position));

        UpdateEnemyState();
        Flip();
    }

    void FixedUpdate()
    {

    }

    void UpdateEnemyState()
    {
        if(playerScript.Instance != null)
        {
            switch (state)
            {
                case State.stage1:
                    canStun = true;
                    break;

                case State.stage2:
                    canStun = true;
                    break;

                case State.stage3:
                    canStun = false;
                    break;

                case State.stage4:
                    canStun = false;
                    break;
            }
        }
    }

    public void AttackController()
    {
        if(state == State.stage1)
        {
            if(Vector2.Distance(playerScript.Instance.transform.position, mantisRb.position) <= attackRange)
            {
                StartCoroutine(Slash());
            }
            else
            {
                //StartCoroutine(Lunge());
                //DiveJump();
                BounceAttack();
            }
        }
    }

    public void ResetAllAttacks()
    {
        attacking = false;
        StopCoroutine(Slash());
        StopCoroutine(Lunge());
        specialDiveAttack = false;
        bounceJumpAttack = false;
    }

    void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState)
        {
            return;
        }

        mantisAnimator.Play(newState);
        currentAnimationState = newState;
    }

    public void Flip()
    {
        if (playerScript.Instance.transform.position.x < transform.position.x && transform.localScale.x > 0)
        {
            transform.eulerAngles = new Vector2(transform.eulerAngles.x, 0);
            isFacingRight = false;
        }
        else
        {
            transform.eulerAngles = new Vector2(transform.eulerAngles.x, 180);
            isFacingRight = true;
        }
    }

    public IEnumerator Stunned()
    {
        stunned = true;
        hitCounter = 0;
        mantisAnimator.SetBool("Stunned", true);

        yield return new WaitForSeconds(4f);

        mantisAnimator.SetBool("Stunned", false);
        stunned = false;
    }

    IEnumerator Slash()
    {
        attacking = true;
        mantisRb.velocity = Vector2.zero;
        mantisAnimator.SetTrigger("Attack");

        yield return new WaitForSeconds(1f);

        ResetAllAttacks();
    }

    IEnumerator Lunge()
    {
        Flip();
        attacking = true;

        mantisAnimator.SetBool("Lunge", true);

        yield return new WaitForSeconds(1f);

        mantisAnimator.SetBool("Lunge", false);

        damagedPlayer = false;

        ResetAllAttacks();
    }

    IEnumerator ChangeColorWhenHit()
    {
        mantisRender.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        mantisRender.color = originalColor;
    }

    #region Stage 2
    public void DiveJump()
    {
        attacking = true;
        moveToPosition = new Vector2(playerScript.Instance.transform.position.x, playerScript.Instance.transform.position.y + 1.2f);
        specialDiveAttack = true;
        mantisAnimator.SetBool("Jump", true);
    }

    public void Dive()
    {
        mantisAnimator.SetBool("Special", true);
        mantisAnimator.SetBool("Jump", false);
    }

    public void InstantiateThornPillars()
    {
        Vector2 impactPoint = mantisGroundCheck.transform.position;
        float spawnDistance = 0.5f;

        for(int i = 0; i < 6; i++)
        {
            Vector2 thornSpawnPointToRight = impactPoint + new Vector2(spawnDistance, -0.15f);
            Vector2 thornSpawnPointToLeft = impactPoint - new Vector2(spawnDistance, 0.15f);
            Instantiate(thornPillar, thornSpawnPointToRight, Quaternion.Euler(0, 0, 0));
            Instantiate(thornPillar, thornSpawnPointToLeft, Quaternion.Euler(0, 0, 0));

            spawnDistance += 0.5f;
        }
        ResetAllAttacks();
    }

    public void BounceAttack()
    {
        attacking = true;
        bounceCount = Random.Range(2, 3);
        BounceJump();
    }

    public void CheckBounce()
    {
        if(bounces < bounceCount - 1)
        {
            bounces++;
            BounceJump();
        }
        else
        {
            bounces = 0;
            mantisAnimator.Play("walk");
        }
    }

    public void BounceJump()
    {
        mantisRb.velocity = Vector2.zero; // GravityScale 0
        moveToPosition = new Vector2(playerScript.Instance.transform.position.x, playerScript.Instance.transform.position.y + 1.2f);
        bounceJumpAttack = true;
        mantisAnimator.SetTrigger("Bend");
    }

    public void CalculateTargetAngle()
    {
        Vector3 directionToTarget = (playerScript.Instance.transform.position - transform.position).normalized;

        float angleOfTarget = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

        rotationToTarget = angleOfTarget;
    }

    public void GetHit()
    {
        if(!stunned)
        {
            if (canStun)
            {
                hitCounter++;
                if(hitCounter > 15)
                {
                    ResetAllAttacks();
                    StartCoroutine(Stunned());
                }
            }
        }
        else
        {
            StopCoroutine(Stunned());
            mantisAnimator.SetBool("Stunned", false);
            stunned = false;
        }

        if (healthPoint <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        ResetAllAttacks();
        dead = true;
        mantisRb.velocity = new Vector2(mantisRb.velocity.x, -25f);
        mantisAnimator.SetTrigger("Die");
    }

    public void DestroyAfterDead()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<playerScript>() != null && (specialDiveAttack || bounceJumpAttack)) 
        {
            playerScript.Instance.hitDamage();
        }

        if(collision.tag == "attackCollision")
        {
            attackInfo attackInfoData = collision.GetComponent<attackInfo>();

            float damageTaken = attackInfoData.attackDamage;

            healthPoint -= Mathf.RoundToInt(damageTaken);

            StartCoroutine(ChangeColorWhenHit());

            GetHit();
        }
    }

    #endregion

    public bool Grounded()
    {
        if(Physics2D.Raycast(mantisGroundCheck.position, Vector2.down, groundCheckY, ground)
            || Physics2D.Raycast(mantisGroundCheck.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, ground)
            || Physics2D.Raycast(mantisGroundCheck.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, ground))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(attackTransform.position, attackArea);

    }
}
