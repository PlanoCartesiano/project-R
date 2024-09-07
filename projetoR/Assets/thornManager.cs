using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thornManager : MonoBehaviour
{

    public Transform spawnPointOnRightPosition;
    public Transform spawnPointOnLeftPosition;
    private fadeEffect fadeEffect;
    private bool playerOnRight;

    void Start()
    {
        fadeEffect = FindFirstObjectByType(typeof(fadeEffect)) as fadeEffect;
        spawnPointOnRightPosition = transform.GetChild(4);
        spawnPointOnLeftPosition = transform.GetChild(5);
    }

    void Update()
    {
        float xPositionPlayer = playerScript.Instance.transform.position.x;
        if (xPositionPlayer < transform.position.x)
        {
            playerOnRight = false;
        }
        else if (xPositionPlayer > transform.position.x)
        {
            playerOnRight = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(playerScript.Instance.isDead)
        {
            return;
        }

        if(collision.tag == "Player" && playerScript.Instance.currentHealth > 1)
        {
            collision.gameObject.SendMessage("hitDamage", SendMessageOptions.DontRequireReceiver);

            StartCoroutine(RespawnPlayer());
        }
        else if(collision.tag == "Player" && playerScript.Instance.currentHealth == 1)
        {
            collision.gameObject.SendMessage("Death", SendMessageOptions.DontRequireReceiver);
        }
    }

    private IEnumerator RespawnPlayer()
    {

        playerScript.Instance.inTransition = true;

        fadeEffect.fadeIn();

        yield return new WaitForSeconds(2.2f);

        if (playerOnRight)
        {
            playerScript.Instance.transform.position = spawnPointOnRightPosition.position;
        }
        else if (!playerOnRight)
        {
            playerScript.Instance.transform.position = spawnPointOnLeftPosition.position;
        }

        fadeEffect.fadeOut();

        playerScript.Instance.inTransition = false;
    }
}
