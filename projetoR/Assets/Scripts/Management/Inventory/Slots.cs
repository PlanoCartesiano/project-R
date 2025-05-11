using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slots : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
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
        if(slotObject != null)
        {
            itemInfo.slotObject = slotObject;
            itemInfo.slotID = slotID;
            slotObject.SendMessage("Equip", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (slotObject != null)
        {
            itemInfo.loadItemInfo();
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        itemInfo.unloadItemInfo();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slotObject != null)
        {
            itemInfo.loadItemInfo();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        itemInfo.unloadItemInfo();
    }
}
