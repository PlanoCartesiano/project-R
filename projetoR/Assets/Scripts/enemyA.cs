using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using Random = UnityEngine.Random;

public class enemyA : MonoBehaviour
{
    public int healthPoint;

    public GameObject knockBackBlow;
    public Transform blowPosition;
    private Animator EnemyAnimator;
    private SpriteRenderer EnemyRenderer;
    public bool IsFacingRight, playerOnRight;
    public GameObject loot;
    public bool dead;

    private playerScript Player;
    public float blowCurrentXPosition;
    public float XPosition;


    void Start()
    {
        Player = FindObjectOfType(typeof(playerScript)) as playerScript;
        EnemyAnimator = GetComponent<Animator>();
        EnemyRenderer = GetComponent<SpriteRenderer>();
        blowPosition.localPosition = new Vector3 (blowCurrentXPosition, blowPosition.localPosition.y, 0);

        if (!IsFacingRight)
        {
            float x = transform.localScale.x;
            x *= -1;
            transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
        }
    }

    // Update is called once per frame
    void Update()
    {

        float xPositionPlayer = Player.transform.position.x;
        if (xPositionPlayer < transform.position.x)
        {
            playerOnRight = false;
        }
        else if(xPositionPlayer > transform.position.x)
        {
            playerOnRight = true;
        }

        if(!IsFacingRight && !playerOnRight)
        {
            XPosition = blowCurrentXPosition;
        }
        else if(IsFacingRight && !playerOnRight)
        {
            XPosition = blowCurrentXPosition * -1;
        }
        else if(!IsFacingRight && playerOnRight)
        {
            XPosition = blowCurrentXPosition * -1;
        }
        else if(IsFacingRight && playerOnRight)
        {
            XPosition = blowCurrentXPosition;
        }

        blowPosition.localPosition = new Vector3(XPosition, blowPosition.localPosition.y, 0);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (dead) {  return; }

        switch (collision.tag)
        {
            case "attackCollision":

                attackInfo attackInfoData = collision.GetComponent<attackInfo>();

                float damageTaken = attackInfoData.attackDamage;

                EnemyAnimator.SetTrigger("hit");

                healthPoint -= Mathf.RoundToInt(damageTaken);

                if(healthPoint <= 0)
                {
                    dead = true;
                    EnemyAnimator.SetInteger("idAnimation", 2);
                    StartCoroutine("death");
                }

                GameObject Knockback = Instantiate(knockBackBlow, blowPosition.position, blowPosition.localRotation);
                Destroy(Knockback, 0.02f);

            break;
        }
    }

    IEnumerator death()
    {
        yield return new WaitForSeconds(1);
        int essenceAmout = Random.Range(1, 6);
        for (int essence = 0; essence < essenceAmout; essence++)
        {
            GameObject dropLoot = Instantiate(loot, transform.position, transform.localRotation);
            dropLoot.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-25,25),85));
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
    }
}
