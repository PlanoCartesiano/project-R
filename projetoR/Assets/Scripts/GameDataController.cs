using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameDataController : MonoBehaviour
{
    private fadeEffect fadeEffect;
    public int test;
    public int Essence;
    public TextMeshProUGUI essenceTxt;

    void Start()
    {
        fadeEffect = FindObjectOfType(typeof(fadeEffect)) as fadeEffect;
        fadeEffect.fadeOut();
    }

    void Update()
    {
        essenceTxt.text = Essence.ToString();
    }
}
