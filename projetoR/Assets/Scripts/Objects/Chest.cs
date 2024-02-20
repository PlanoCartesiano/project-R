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
        gameDataController = FindFirstObjectByType(typeof(GameDataController)) as GameDataController;
        objectSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Interaction()
    {
        if (!openedState)
        {
            openedState = true;
            objectSpriteRenderer.sprite = currentSprite[1];
            GetComponent<Collider2D>().enabled = false;
        }
    }

}
