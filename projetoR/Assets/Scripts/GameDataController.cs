using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameDataController : MonoBehaviour
{
    public int test;
    public int Essence;
    public TextMeshProUGUI essenceTxt;

    void Start()
    {
        
    }

    void Update()
    {
        essenceTxt.text = Essence.ToString();
    }
}
