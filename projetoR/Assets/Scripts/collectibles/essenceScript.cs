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
        AudioManager.instance.PlayOneShotSound(FMODEvents.instance.coinCollected, this.transform.position);
        Destroy(this.gameObject);
    }
}