using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slots : MonoBehaviour
{

    private GameDataController gameDataController;
    private itensInformations itemInfo;
    public GameObject slotObject;
    public int slotID;

    void Start()
    {
        gameDataController = FindFirstObjectByType(typeof(GameDataController)) as GameDataController;
        itemInfo = FindFirstObjectByType(typeof(itensInformations)) as itensInformations;
    }

    public void EquipItem()
    {
        //print("Equipei item!");

        if(slotObject != null)
        {
            itemInfo.slotObject = slotObject;
            itemInfo.slotID = slotID;
            slotObject.SendMessage("Equip", SendMessageOptions.DontRequireReceiver);
        }
    }
}
