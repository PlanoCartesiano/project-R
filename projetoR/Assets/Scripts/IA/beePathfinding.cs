using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.Cinemachine;

public class beePathfinding : MonoBehaviour
{
    public enum enemyState
    {
        idle,
        observer,
        attacking,
    }
    public enum AnimationState
    {
        flying,
        attacking,
        hit,
        death,
    }

    private playerScript playerScript;
    public bool playerOnRight;
    public Transform target;
    public float speed;
    public float nextWaypointDistance = 3f;
    private Animator enemyAnimator;
    private AnimationState currentAnimation;
    //[SerializeField] private targetDetection targetDetection;
    //private bool playerOnRange = false;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D enemyRb;

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

    void Start()
    {
        seeker = GetComponent<Seeker>();
        enemyRb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        enemyAnimator = GetComponent<Animator>();
        playerScript = FindFirstObjectByType(typeof(playerScript)) as playerScript;
        //targetDetection = GetComponent<targetDetection>();

        InvokeRepeating("UpdatePath", 0f, .5f);
    }

    void Update()
    {

        if (!dead)
        {
            ChangeAnimationState(AnimationState.flying);
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

        if (enemyRb.velocity.x <= -0.01f && !playerOnRight)
        {
            XPosition = blowCurrentXPosition;
        }
        else if (enemyRb.velocity.x >= 0.01f && !playerOnRight)
        {
            XPosition = blowCurrentXPosition * -1;
        }
        else if (enemyRb.velocity.x <= -0.01f && playerOnRight)
        {
            XPosition = blowCurrentXPosition * -1;
        }
        else if (enemyRb.velocity.x >= 0.01f && playerOnRight)
        {
            XPosition = blowCurrentXPosition;
        }

        blowPosition.localPosition = new Vector3(XPosition, blowPosition.localPosition.y, 0);

    }

    void UpdatePath()
    {
        if (seeker.IsDone() && targetDetection.playerOnRange == true && dead == false)
        {
            seeker.StartPath(enemyRb.position, target.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void FixedUpdate()
    {
        /*if(target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }*/

        if(path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - enemyRb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        enemyRb.AddForce(force);

        float distance = Vector2.Distance(enemyRb.position, path.vectorPath[currentWaypoint]);

        if(distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if(enemyRb.velocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(-1f, 1f, 0f);
        }else if(enemyRb.velocity.x <= -0.01f)
        {
            transform.localScale = new Vector3(1f, 1f, 0f);
        }

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

                healthPoint -= Mathf.RoundToInt(damageTaken);

                if (healthPoint <= 0)
                {
                    enemyAnimator.SetBool("dead", true);
                    enemyRb.gravityScale = 1f;
                    StartCoroutine("death");
                    dead = true;
                }

                GameObject Knockback = Instantiate(knockBackBlow, blowPosition.position, blowPosition.localRotation);
                Destroy(Knockback, 0.02f);

                break;

            case "SlingshotBullet":
                Destroy(collision.gameObject);
                Destroy(this.gameObject);
                break;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Player":

                collision.gameObject.SendMessage("hitDamage", SendMessageOptions.DontRequireReceiver);

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

    IEnumerator death()
    {
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
