using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class fadeEffect : MonoBehaviour
{
    public GameObject blackoutPanel;
    public Image blackout;
    public Color[]  transitionColor;
    public float step;

    public void fadeIn()
    {
        blackoutPanel.SetActive(true);
        StartCoroutine("fadeI");
    }

    public void fadeOut()
    {
        StartCoroutine("fadeO");
    }

    IEnumerator fadeI()
    {
        for(float i = 0; i <=1; i+= step)
        {
            blackout.color = Color.Lerp(transitionColor[0], transitionColor[1], i);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator fadeO()
    {
        for (float i = 0; i <= 1; i += step)
        {
            blackout.color = Color.Lerp(transitionColor[1], transitionColor[0], i);
            yield return new WaitForEndOfFrame();
        }

        blackoutPanel.SetActive(false);
    }
}
