using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slingshotBulletScript : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "SceneObject")
        {
            Destroy(this.gameObject);
        };
    }
}
