using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollowObject : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    private float flipYRotationTime = 0.5f;

    private Coroutine turnCoroutine;
    private playerScript player;
    private bool isFacingRight = true;

    void Start()
    {
        player = playerTransform.gameObject.GetComponent<playerScript>();
        isFacingRight = player.IsFacingRight;
    }

    void Update()
    {
        transform.position = playerTransform.position;
    }

    void FixedUpdate()
    {
        if(playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    public void CallTurn()
    {
        //turnCoroutine = StartCoroutine(FlipYLerp());
        //LeanTween.rotateY(gameObject, DetermineEndRotation(), flipYRotationTime).setEaseInOutSine();
    }

    private IEnumerator FlipYLerp()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotationAmout = DetermineEndRotation();
        float yRotation = 0f;

        float elapsedTime = 0f;
        while (elapsedTime < flipYRotationTime)
        {
            elapsedTime += Time.deltaTime;

            yRotation = Mathf.Lerp(startRotation, endRotationAmout, (elapsedTime / flipYRotationTime));
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

            yield return null;
        };
    }

    private float DetermineEndRotation()
    {
        isFacingRight = !isFacingRight;
        if (isFacingRight)
        {
            return 0f;
        }
        else
        {
            return 180f;
        }
    }
}
