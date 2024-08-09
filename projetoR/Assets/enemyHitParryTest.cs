using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHitParryTest : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "parryCollision":
                StartCoroutine(Stunned());
                print("Meu ataque foi revertido! Estou atordoado!");
                break;
        }
    }

    public IEnumerator Stunned()
    {
        spriteRenderer.color = Color.blue;
        yield return new WaitForSeconds(2f);
        spriteRenderer.color = Color.red;
    }
}
