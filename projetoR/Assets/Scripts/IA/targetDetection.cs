using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetDetection : MonoBehaviour
{
    public static bool playerOnRange = false;

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Player":
                playerOnRange = true;
                break;
        }
    }
}
