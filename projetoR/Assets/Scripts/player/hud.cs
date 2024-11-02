using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class hud : MonoBehaviour
{
    private playerScript player;
    public Image[] hpHearts;
    public Sprite empty, full;

    void Start()
    {
        player = FindFirstObjectByType(typeof(playerScript)) as playerScript;
    }

    void Update()
    {
        healthController();
    }

    void healthController()
    {
        foreach (Image img in hpHearts)
        {
            img.enabled = true;
        }

        if (player.currentHealth == 5)
        {

        }else if (player.currentHealth == 4)
        {
            hpHearts[4].enabled = false;
        }else if (player.currentHealth == 3)
        {
            hpHearts[4].enabled = false;
            hpHearts[3].enabled = false;
        }else if (player.currentHealth == 2)
        {
            hpHearts[4].enabled = false;
            hpHearts[3].enabled = false;
            hpHearts[2].enabled = false;
        }else if ( player.currentHealth == 1)
        {
            hpHearts[4].enabled = false;
            hpHearts[3].enabled = false;
            hpHearts[2].enabled = false;
            hpHearts[1].enabled = false;
        }else if (player.currentHealth <= 0)
        {
            hpHearts[4].enabled = false;
            hpHearts[3].enabled = false;
            hpHearts[2].enabled = false;
            hpHearts[1].enabled = false;
            hpHearts[0].enabled = false;
        }
    }
}
