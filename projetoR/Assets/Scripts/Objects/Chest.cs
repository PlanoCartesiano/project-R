using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private GameDataController gameDataController;

    private SpriteRenderer objectSpriteRenderer;
    public Sprite[] currentSprite;
    public bool openedState;

    void Start()
    {
        gameDataController = FindObjectOfType(typeof(GameDataController)) as GameDataController;
        objectSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Interaction()
    {
        openedState = !openedState;

        switch(openedState)
        {
            case true:
                objectSpriteRenderer.sprite = currentSprite[1];

                if(gameDataController == null) //usado para prevenir o erro de perda do objeto durante loading da cena
                {
                   gameDataController = FindObjectOfType(typeof(GameDataController)) as GameDataController;
                }
                gameDataController.test += 1;
                break;

            case false:
                objectSpriteRenderer.sprite = currentSprite[0];
                break;
        }
    }

}
