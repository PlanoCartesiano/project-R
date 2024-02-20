using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class essenceScript : MonoBehaviour
{
    private GameDataController gameDataController;

    void Start()
    {
        gameDataController = FindFirstObjectByType(typeof(GameDataController)) as GameDataController;
    }
    public void collect()
    {
        gameDataController.Essence += 1;
        Destroy(this.gameObject);
    }
}
