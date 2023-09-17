using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollowTarget : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.08f;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        };

        Vector3 startedPosition = new Vector3(target.position.x, target.position.y, -1f);
        Vector3 smoothPosition = Vector3.Lerp(transform.position, startedPosition, smoothSpeed);
        transform.position = smoothPosition;
    }
}
