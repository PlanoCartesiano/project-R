using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Cinemachine;

public class basicEnemy : MonoBehaviour
{
    public enum enemyState
    {
        idle,
        observer,
        dead,
    }
    public enum AnimationState
    {
        idle,
        walking,
        hit,
        death,
    }

    [Header("Enemy Stats")]
    public int healthPoint;
    public bool dead;

    [Header("Knockback settings")]
    public GameObject knockBackBlow;
    public Transform blowPosition;
    public float blowCurrentXPosition;
    public float XPosition;

    [Header("Loot Spoliage")]
    public GameObject loot;

    private playerScript playerScript;
    public enemyState currentEnemyState;
    private AnimationState currentAnimation;
    private Rigidbody2D enemyRb;
    private Animator enemyAnimator;
    private Vector3 dir = Vector3.right;
    public float distanceToTurn;
    public LayerMask obstructionLayer;
    public float moveSpeed;
    public float speed;
    public bool IsFacingRight, playerOnRight;
    public float idleCooldown;
    public float enemyViewDistance;
    public LayerMask playerLayer;
    public bool isTurnRayAtivacted;
    private float distance;
    private CinemachineImpulseSource impulseSource;
    [SerializeField] private ScreenShakeProfile profile;

    void Start()
    {
        playerScript = FindFirstObjectByType(typeof(playerScript)) as playerScript;
        enemyRb = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<Animator>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        blowPosition.localPosition = new Vector3(blowCurrentXPosition, blowPosition.localPosition.y, 0);

        if (!IsFacingRight)
        {
            TurnCheck();
        }
        speed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawRay(transform.position, dir * enemyViewDistance, Color.red);

        if (dead)
        {
            return;
        }

        RaycastHit2D hitPlayer = Physics2D.Raycast(transform.position, dir, enemyViewDistance, playerLayer);

        /*if (hitPlayer && !dead)
        {
            ChangeState(enemyState.attacking);
            Debug.DrawRay(transform.position, dir * enemyViewDistance, Color.red);
        }
        else if (!hitPlayer && currentEnemyState == enemyState.attacking && distance >= 2f && !dead)
        {
            ChangeState(enemyState.observer);
        }*/

        if (currentEnemyState == enemyState.observer)
        {
            RaycastHit2D turnRay = Physics2D.Raycast(transform.position, dir, distanceToTurn, obstructionLayer);
            Debug.DrawRay(transform.position, dir * enemyViewDistance, Color.cyan);

            if (speed == 0 && !dead)
            {
                ChangeAnimationState(AnimationState.idle);
            }else if (speed != 0 && !dead)
            {
                ChangeAnimationState(AnimationState.walking);
            }

            if (turnRay)
            {
                isTurnRayAtivacted = true;
                ChangeState(enemyState.idle);
            }else if (!turnRay)
            {
                isTurnRayAtivacted = false;
            }
        }

        /*if (currentEnemyState == enemyState.attacking && !dead)
        {
            distance = Vector3.Distance(transform.position, playerScript.transform.position);
        }*/

        enemyRb.velocity = new Vector2(speed, enemyRb.velocity.y); //lembrar do fixedUpdate timeDeltatime

        if (speed == 0 && !dead)
        {
            ChangeAnimationState(AnimationState.idle);
        }
        else if (speed != 0 && !dead)
        {
            ChangeAnimationState(AnimationState.walking);
        }

        float xPositionPlayer = playerScript.transform.position.x;
        if (xPositionPlayer < transform.position.x)
        {
            playerOnRight = false;
        }
        else if (xPositionPlayer > transform.position.x)
        {
            playerOnRight = true;
        }

        if (!IsFacingRight && !playerOnRight)
        {
            XPosition = blowCurrentXPosition;
        }
        else if (IsFacingRight && !playerOnRight)
        {
            XPosition = blowCurrentXPosition * -1;
        }
        else if (!IsFacingRight && playerOnRight)
        {
            XPosition = blowCurrentXPosition * -1;
        }
        else if (IsFacingRight && playerOnRight)
        {
            XPosition = blowCurrentXPosition;
        }

        blowPosition.localPosition = new Vector3(XPosition, blowPosition.localPosition.y, 0);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (dead) { return; }

        switch (collision.tag)
        {
            case "attackCollision":

                attackInfo attackInfoData = collision.GetComponent<attackInfo>();

                float damageTaken = attackInfoData.attackDamage;


                enemyAnimator.SetTrigger("hit");
                //CameraShakeManager.instance.CameraShake(impulseSource);
                CameraShakeManager.instance.ScreenShakeFromProfile(profile, impulseSource);
                //ChangeAnimationState(AnimationState.hit);

                healthPoint -= Mathf.RoundToInt(damageTaken);

                if (healthPoint <= 0)
                {
                    dead = true;
                    enemyAnimator.SetBool("dead", true);
                    StopAllCoroutines();
                    StartCoroutine("death");
                }
               
                if(knockBackBlow == null) { return; }
                else
                {
                    GameObject Knockback = Instantiate(knockBackBlow, blowPosition.position, blowPosition.localRotation);
                    Destroy(Knockback, 0.02f);
                }

                break;

            case "SlingshotBullet":
                Destroy(collision.gameObject);
                Destroy(this.gameObject);
                break;

            case "Player":
                collision.gameObject.SendMessage("hitDamage", SendMessageOptions.DontRequireReceiver);
                break;
        }
    }

    /*public void OnTriggerEnter2D(Collision2D collision)
    {

        if (dead) { return; }

        switch (collision.gameObject.tag)
        {
            case "Player":

                collision.gameObject.SendMessage("hitDamage", SendMessageOptions.DontRequireReceiver);

                break;
        }
    }*/

    void ChangeState(enemyState newState)
    {
        if (currentEnemyState == newState)
        {
            return;
        }

        currentEnemyState = newState;
        switch(newState)
        {
            case enemyState.idle:
                speed = 0f;
                StartCoroutine("idle");
                break;

            case enemyState.observer:
                speed = moveSpeed;
                break;
        }
    }

    void ChangeAnimationState(AnimationState newState)
    {
        if (currentAnimation == newState)
        {
            return;
        }

        enemyAnimator.Play(newState.ToString());
        currentAnimation = newState;
    }

    private void TurnCheck()
    {
        if (!IsFacingRight)
        {
            Turn();
        }
        else if (IsFacingRight)
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
            moveSpeed *= -1f;
            dir = Vector3.left;
            IsFacingRight = !IsFacingRight;
        }
        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            moveSpeed *= -1f;
            dir = Vector3.right;
            IsFacingRight = !IsFacingRight;
        }
    }

    IEnumerator idle()
    {
        if (currentEnemyState == enemyState.idle)
        {
            yield return null;
        }
        yield return new WaitForSeconds(idleCooldown);
        TurnCheck();
        ChangeState(enemyState.observer);
    }

    IEnumerator death()
    {
        ChangeAnimationState(AnimationState.death);
        yield return new WaitForSeconds(1);
        int essenceAmout = Random.Range(1, 6);
        for (int essence = 0; essence < essenceAmout; essence++)
        {
            GameObject dropLoot = Instantiate(loot, transform.position, transform.localRotation);
            dropLoot.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-25, 25), 85));
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
    }
}
