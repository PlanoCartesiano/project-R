using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Doors : MonoBehaviour
{
    private fadeEffect fadeEffect;

    public Transform tPlayer;

    public  Transform   destiny;

    public bool darkness;
    public Material light2D, default2D;

    // Start is called before the first frame update
    void Start()
    {
        fadeEffect = FindFirstObjectByType(typeof(fadeEffect)) as fadeEffect;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public  void Interaction()
    {
        StartCoroutine("triggerDoor");
    }

    IEnumerator triggerDoor()
    {
        fadeEffect.fadeIn();
        yield return new WaitWhile(() => fadeEffect.blackout.color.a < 0.9f);
        tPlayer.position = destiny.position;
        fadeEffect.fadeOut();
    }
}
