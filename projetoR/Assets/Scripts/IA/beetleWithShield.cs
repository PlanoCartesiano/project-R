using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beetleWithShield : MonoBehaviour
{
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

    [Header("Loot Spoliage")]
    public GameObject loot;

    [SerializeField] private StudioEventEmitter[] emitterSFX;
    private playerScript playerScript;
    private AnimationState currentAnimation;
    private Rigidbody2D enemyRb;
    private Animator enemyAnimator;
    public bool IsFacingRight, playerOnShieldSide;
    public LayerMask playerLayer;
    public bool isProtected;
    public AnimationClip hitProtectedClip;
    //public ShieldCheck ShieldCheck;

    void Start()
    {
        playerScript = FindFirstObjectByType(typeof(playerScript)) as playerScript;
        enemyRb = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (dead)
        {
            return;
        }

        float xPositionPlayer = playerScript.transform.position.x;
        if (xPositionPlayer < transform.position.x)
        {
            playerOnShieldSide = true;
        }
        else if (xPositionPlayer > transform.position.x)
        {
            playerOnShieldSide = false;
        }

        if (playerOnShieldSide)
        {
            isProtected = true;
        }
        else if (!playerOnShieldSide)
        {
            isProtected = false;
        }

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (dead) { return; }

        switch (collision.tag)
        {
            case "attackCollision":

                if (isProtected)
                {
                    emitterSFX[0].Play();
                    enemyAnimator.SetTrigger("hitProtected");
                    break;
                }

                enemyAnimator.SetTrigger("hit");

                attackInfo attackInfoData = collision.GetComponent<attackInfo>();

                float damageTaken = attackInfoData.attackDamage;


                healthPoint -= Mathf.RoundToInt(damageTaken);

                if (healthPoint <= 0)
                {
                    dead = true;
                    StopAllCoroutines();
                    StartCoroutine("death");
                }

                break;

            case "SlingshotBullet":
                Destroy(collision.gameObject);
                Destroy(this.gameObject);
                break;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {

        if (dead) { return; }

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
